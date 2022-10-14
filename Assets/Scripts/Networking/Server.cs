using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

#nullable enable
/// <summary>
/// The game server that primarily acts as a relay between clients
/// </summary>
public class Server
{
    private bool running = true;

    public ServerGameData gameData;
    private string serverPassword;

    #region Threads

    private Thread acceptClientThread;
    private Thread recieveThread;
    private Thread sendThread;

    #endregion Threads

    private ConcurrentQueue<Tuple<int, byte[]>> recieveQueue = new ConcurrentQueue<Tuple<int, byte[]>>();
    private ConcurrentQueue<Tuple<int, byte[]>> sendQueue = new ConcurrentQueue<Tuple<int, byte[]>>();

    #region Cooldowns

    /// <summary>
    /// Time between sending messages
    /// </summary>
    private const int SEND_COOLDOWN = 2;

    /// <summary>
    /// How long to wait before rechecking an empty queue
    /// </summary>
    private const int RECEIVE_COOLDOWN = 2;

    /// <summary>
    /// Time between trying to add a client again
    /// </summary>
    private const int CLIENT_ADD_COOLDOWN = 5;

    /// <summary>
    /// Time between trying to remove a client again
    /// </summary>
    private const int CLIENT_REMOVE_COOLDOWN = 5;

    #endregion Cooldowns

    public bool AcceptingClients = false;
    public int ClientsConnected { get; private set; } = 0;

    private ConcurrentDictionary<int, ServerPlayerData> PlayerData = new ConcurrentDictionary<int, ServerPlayerData>();
    private int PlayerIDCounter = 0;

    private InternalServerPacketHandler internalPackerHandler;

    public Server(ServerGameData gameData, string serverPassword)
    {
        serverPassword = Util.RemoveInvisibleChars(serverPassword);

        acceptClientThread = new Thread(AcceptClients);
        recieveThread = new Thread(ReceiveLoop);
        sendThread = new Thread(SendLoop);

        this.gameData = gameData;
        this.serverPassword = serverPassword;

        internalPackerHandler = new InternalServerPacketHandler(this);
    }

    /// <summary>
    /// Starts the server
    /// </summary>
    public void Start()
    {
        AcceptingClients = true;
        acceptClientThread.Start();
        recieveThread.Start();
        sendThread.Start();
    }

    /// <summary>
    /// Adds a player to the server and begins listening for messages
    /// </summary>
    /// <param name="handler">Socket used to communicate with client</param>
    /// <param name="name"></param>
    /// <param name="team"></param>
    /// <param name="playerInTeam"></param>
    /// <returns></returns>
    private Tuple<bool, int> TryAddPlayer(Socket handler, string name, int team, int playerInTeam)
    {
        ServerPlayerData player_data = new ServerPlayerData(handler, PlayerIDCounter, name, team, playerInTeam);
        bool player_added = PlayerData.TryAdd(PlayerIDCounter, player_data);
        PlayerIDCounter++;

        if (!player_added) return new Tuple<bool, int>(false, -1);

        handler.BeginReceive(
                   player_data.Buffer,
                   0,
                   1024,
                   0,
                   new AsyncCallback(ReadCallback),
                   player_data
               );

        SendToAll(ServerOtherClientInfoPacket.Build(player_data.PlayerID, player_data.Name, player_data.Team, player_data.PlayerInTeam));

        // OnPlayersChange.Invoke();

        return new Tuple<bool, int>(player_added, PlayerIDCounter - 1);
    }

    /// <summary>
    /// Disconnects a player from the server
    /// </summary>
    /// <param name="player">Player ID</param>
    public bool TryRemovePlayer(int player, string? reason = null)
    {
        if (PlayerData.ContainsKey(player))
        {
            ServerPlayerData playerData;
            bool removed = PlayerData.TryRemove(player, out playerData);

            if (!removed) return false;

            playerData.ShutdownSocket();

            // OnPlayersChange.Invoke();

            SendToAll(ServerInformOfClientDisconnectPacket.Build(playerData.PlayerID));

            return true;
        }
        else return false;
    }

    public bool ValidateTeams()
    {
        return true;
    }

    public bool StartGame()
    {
        if (!ValidateTeams()) return false;

        SendToAll(StartGamePacket.Build());
        return true;
    }

    /// <summary>
    /// Loop to accept new clients
    /// </summary>
    private void AcceptClients()
    {
        IPAddress ipAddress = IPAddress.Any;

        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, NetworkSettings.PORT);

        Socket? listener = null;

        try
        {
            listener = new Socket(
                ipAddress.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp
            );

            listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);

            listener.Bind(localEndPoint);

            listener.Listen(100);

            while (true)
            {
                // Recieve connection data
                Socket handler = listener.Accept();

                byte[] rec_bytes = new byte[1024];
                int total_rec = 0;

                while (total_rec < 4)
                {
                    byte[] partial_bytes = new byte[1024];
                    int bytesRec = handler.Receive(rec_bytes);

                    total_rec += bytesRec;

                    Tuple<byte[], int> cleared = ArrayExtensions.ClearEmpty(rec_bytes);
                    rec_bytes = cleared.Item1;
                    total_rec -= cleared.Item2;

                    ArrayExtensions.Merge(
                        rec_bytes,
                        ArrayExtensions.Slice(partial_bytes, 0, bytesRec),
                        total_rec
                    );
                }

                int packet_len = PacketBuilder.GetPacketLength(
                    ArrayExtensions.Slice(rec_bytes, 0, 4)
                );

                while (total_rec < packet_len)
                {
                    byte[] partial_bytes = new byte[1024];
                    int bytesRec = handler.Receive(partial_bytes);

                    total_rec += bytesRec;
                    ArrayExtensions.Merge(
                        rec_bytes,
                        ArrayExtensions.Slice(partial_bytes, 0, bytesRec),
                        total_rec
                    );
                }

                // Decode connection data
                ClientConnectRequestPacket initPacket = new ClientConnectRequestPacket(
                    PacketBuilder.Decode(ArrayExtensions.Slice(rec_bytes, 0, packet_len))
                );

                // Not accepting clients
                if (!AcceptingClients)
                {
                    handler.Send(
                        ServerKickPacket.Build("Server is not accepting clients at this time")
                    );
                    continue;
                }

                // Password incorrect
                if (serverPassword != "" && initPacket.Password != serverPassword)
                {
                    handler.Send(
                        ServerKickPacket.Build("Wrong Password: '" + initPacket.Password + "'")
                    );
                    continue;
                }

                // Version mismatch
                if (initPacket.Version != NetworkSettings.VERSION)
                {
                    handler.Send(
                        ServerKickPacket.Build(
                            "Wrong Version:\nServer: "
                                + NetworkSettings.VERSION.ToString()
                                + "| Client (You): "
                                + initPacket.Version
                        )
                    );
                    continue;
                }

                

                int playerID;
                while (true)
                {
                    Tuple<bool, int> add_result = TryAddPlayer(handler, initPacket.Name, -1, -1);
                    if (add_result.Item1) { playerID = add_result.Item2; break; }
                    Thread.Sleep(CLIENT_ADD_COOLDOWN);
                }
                SendMessage(playerID, ServerConnectAcceptPacket.Build(playerID));

                foreach (ServerPlayerData player_data in PlayerData.Values)
                {
                    SendMessage(playerID, ServerOtherClientInfoPacket.Build(player_data.PlayerID, player_data.Name, player_data.Team, player_data.PlayerInTeam));
                }
            }
        }
        catch (ThreadAbortException)
        {
            try { listener?.Shutdown(SocketShutdown.Both); } catch (Exception e) { Debug.LogError(e); }
            try { listener?.Close(); } catch (Exception e) { Debug.LogError(e); }
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    /// <summary>
    /// Called when a message is recieved from a client
    /// </summary>
    /// <param name="ar"></param>
    private void ReadCallback(IAsyncResult ar)
    {
        if (!running) { return; }
        string content = string.Empty;

        ServerPlayerData CurrentPlayer = (ServerPlayerData)ar.AsyncState;
        Socket handler = CurrentPlayer.Handler;

        int bytesRead = handler.EndReceive(ar);

        if (bytesRead > 0)
        {
            ArrayExtensions.Merge(
                CurrentPlayer.LongBuffer,
                CurrentPlayer.Buffer,
                CurrentPlayer.LongBufferSize
            );
            CurrentPlayer.LongBufferSize += bytesRead;

        ReprocessBuffer:

            if (
                CurrentPlayer.CurrentPacketLength == -1
                && CurrentPlayer.LongBufferSize >= PacketBuilder.PacketLenLen
            )
            {
                CurrentPlayer.CurrentPacketLength = PacketBuilder.GetPacketLength(
                    CurrentPlayer.LongBuffer
                );
            }

            if (
                CurrentPlayer.CurrentPacketLength != -1
                && CurrentPlayer.LongBufferSize >= CurrentPlayer.CurrentPacketLength
            )
            {
                recieveQueue.Enqueue(
                    new Tuple<int, byte[]>(
                        CurrentPlayer.PlayerID,
                        ArrayExtensions.Slice(
                            CurrentPlayer.LongBuffer,
                            0,
                            CurrentPlayer.CurrentPacketLength
                        )
                    )
                );
                byte[] new_buffer = new byte[1024];
                ArrayExtensions.Merge(
                    new_buffer,
                    ArrayExtensions.Slice(
                        CurrentPlayer.LongBuffer,
                        CurrentPlayer.CurrentPacketLength,
                        1024
                    ),
                    0
                );
                CurrentPlayer.LongBuffer = new_buffer;
                CurrentPlayer.LongBufferSize -= CurrentPlayer.CurrentPacketLength;
                CurrentPlayer.CurrentPacketLength = -1;
                if (CurrentPlayer.LongBufferSize > 0)
                {
                    // Recieved more than one message
                    goto ReprocessBuffer;
                }
            }
        }
        // Ready to recieve again
        handler.BeginReceive(
                CurrentPlayer.Buffer,
                0,
                1024,
                0,
                new AsyncCallback(ReadCallback),
                CurrentPlayer
            );
    }

    /// <summary>
    /// Processes recieved messages
    /// </summary>
    private void ReceiveLoop()
    {
        try
        {
            while (true)
            {
                if (recieveQueue.IsEmpty)
                {
                    Thread.Sleep(RECEIVE_COOLDOWN);
                    continue;
                } // Nothing recieved

                Tuple<int, byte[]> content;
                if (!recieveQueue.TryDequeue(out content))
                {
                    // Dequeue failed
                    continue;
                }
                if (!PlayerData.ContainsKey(content.Item1))
                {
                    // Player no longer exists
                    continue;
                }

                try
                {
                    Packet packet = PacketBuilder.Decode(content.Item2, content.Item1);
                    bool handled = internalPackerHandler.TryHandlePacket(packet);

                    if (!handled)
                    {
                        // Pass to game manager
                    }
                }
                catch (PacketDecodeError e)
                {
                    Debug.LogError(e);
                    while (!TryRemovePlayer(content.Item1, "Fatal packet handling error")) Thread.Sleep(CLIENT_REMOVE_COOLDOWN);
                }
            }
        }
        catch (ThreadAbortException) { }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    /// <summary>
    /// Sends a message
    /// </summary>
    /// <param name="playerID">Player to send to</param>
    /// <param name="payload">Data to send</param>
    public void SendMessage(int playerID, byte[] payload)
    {
        sendQueue.Enqueue(new Tuple<int, byte[]>(playerID, payload));
    }

    /// <summary>
    /// Sends a message to all connected players
    /// </summary>
    /// <param name="payload"></param>
    public void SendToAll(byte[] payload)
    {
        foreach (int player_id in PlayerData.Keys)
        {
            SendMessage(player_id, payload);
        }
    }

    /// <summary>
    /// Sends queued messages
    /// </summary>
    private void SendLoop()
    {
        try
        {
            while (true)
            {
                if (!sendQueue.IsEmpty)
                {
                    SendMessageFromSendQueue();
                }

                Thread.Sleep(SEND_COOLDOWN);
            }
        }
        catch (ThreadAbortException) { }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
    }

    /// <summary>
    /// Sends the first message from the send queue
    /// </summary>
    private void SendMessageFromSendQueue()
    {
        Tuple<int, byte[]> to_send;
        if (sendQueue.TryDequeue(out to_send))
        {
            if (!PlayerData.ContainsKey(to_send.Item1))
            {
                return;
            }
            try
            {
                PlayerData[to_send.Item1].Handler.Send(to_send.Item2);
            }
            catch (SocketException se)
            {
                Debug.LogError(se);
                TryRemovePlayer(to_send.Item1);
            }
        }
    }

    /// <summary>
    /// Sends all remaining messages from the send queue.
    /// </summary>
    /// <exception cref="ThreadStateException">Throws ThreadStateException if send loop is running</exception>
    private void FlushSendQueue()
    {
        if (sendThread.ThreadState == ThreadState.Running) throw new ThreadStateException("Send queue cannot be flushed while send loop is running!");

        while (!sendQueue.IsEmpty) SendMessageFromSendQueue();
    }

    /// <summary>
    /// Shuts down the server
    /// </summary>
    public void Shutdown()
    {
        running = false;
        acceptClientThread.Abort();
        recieveThread.Abort();
        sendThread.Abort();
        FlushSendQueue();
    }
}