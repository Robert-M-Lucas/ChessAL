using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;
using ThreadState = System.Threading.ThreadState;

#nullable enable
public class Client
{
    # region Server Connection
    private Socket? handler = null;

    public string IP;
    public string Password;
    public string PlayerName;
    Action<string?> connectionStatusCallback;

    private byte[] serverLongBuffer = new byte[1024];
    private byte[] serverBuffer = new byte[1024];
    private int serverLongBufferSize = 0;
    private int serverCurrentPacketLength = -1;
    #endregion

    #region Threads

    private Thread connectionThread;
    private Thread receiveThread;
    private Thread sendThread;

    #endregion

    private ConcurrentQueue<byte[]> ContentQueue = new ConcurrentQueue<byte[]>();
    private ConcurrentQueue<byte[]> SendQueue = new ConcurrentQueue<byte[]>();

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

    public ConcurrentDictionary<int, ClientPlayerData> PlayerData = new ConcurrentDictionary<int, ClientPlayerData>();

    private InternalClientPacketHandler internalPacketHandler;

    #region Actions 

    public Action OnPlayersChange = () => { };
    private Action onConnect = () => { };

    #endregion

    #region Ping
    public Action<int>? pingResponseAction = null;
    public Stopwatch PingTimer = new Stopwatch();
    #endregion

    /// <summary>
    /// 
    /// </summary>
    /// <param name="IP">IP String</param>
    /// <param name="password">Server password (can be left empty)</param>
    /// <param name="playerName">Client name</param>
    /// <param name="connectionStatusCallback">Action called when connection succeeds or fails.
    /// String will be null when successful or give a reason for failure.</param>
    public Client(string IP, string password, string playerName, Action<string?> connectionStatusCallback)
    {
        this.IP = IP;
        Password = password;
        PlayerName = playerName;
        this.connectionStatusCallback = connectionStatusCallback;

        connectionThread = new Thread(StartConnecting);
        receiveThread = new Thread(ReceiveLoop);
        sendThread = new Thread(SendLoop);

        internalPacketHandler = new InternalClientPacketHandler(this);
    }

    /// <summary>
    /// Start connecting to the server
    /// </summary>
    public void Connect() { connectionThread.Start(); }

    /// <summary>
    /// Starts connecting (threaded)
    /// </summary>
    private void StartConnecting()
    {
        try
        {
            IPAddress HostIpA;
            try { HostIpA = IPAddress.Parse(IP); } catch (FormatException) { connectionStatusCallback("IP incorrectly formatted"); return; }
            IPEndPoint RemoteEP = new IPEndPoint(HostIpA, NetworkSettings.PORT);

            handler = new Socket(HostIpA.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try { handler.Connect(RemoteEP); } catch (SocketException) { connectionStatusCallback("Server refused connection"); return; }

            handler.Send(ClientConnectRequestPacket.Build(PlayerName, NetworkSettings.VERSION, Password));

            handler.BeginReceive(serverBuffer, 0, 1024, 0, new AsyncCallback(ReadCallback), null);

            // Successful connection
            connectionStatusCallback(null);

            receiveThread = new Thread(ReceiveLoop);
            receiveThread.Start();
            sendThread = new Thread(SendLoop);
            sendThread.Start();
        }
        catch (Exception e)
        {
            connectionStatusCallback(e.ToString());
        }
    }

    /// <summary>
    /// Get the current server ping
    /// </summary>
    /// <param name="pingCallback">Called when ping is calculated</param>
    public void GetPing(Action<int> pingCallback)
    {
        if (pingResponseAction is not null) return; // Already fetching ping
        pingResponseAction = pingCallback;
        SendMessage(ClientPingPacket.Build());
    }

    /// <summary>
    /// Adds a player or updates them if they already exist
    /// </summary>
    /// <param name="playerID"></param>
    /// <param name="name">Player name</param>
    /// <param name="team">Player's team</param>
    /// <param name="playerInTeam">Player's place in team</param>
    /// <returns></returns>
    public bool AddOrUpdatePlayer(int playerID, string name, int team = -1, int playerInTeam = -1)
    {
        ClientPlayerData player_data = new ClientPlayerData(playerID, name, team, playerInTeam);
        bool player_added;

        if (!PlayerData.ContainsKey(playerID)) player_added = PlayerData.TryAdd(playerID, player_data);
        else
        {
            ClientPlayerData current_data;
            bool success = PlayerData.TryGetValue(playerID, out current_data);
            if (!success) return false;

            player_added = PlayerData.TryUpdate(playerID, player_data, current_data);
        }

        if (!player_added) return false;

        OnPlayersChange.Invoke();

        return true;
    }

    /// <summary>
    /// Adds a client to the player list
    /// </summary>
    /// <param name="player">Player ID</param>
    public bool TryRemovePlayer(int player, string? reason = null)
    {
        if (PlayerData.ContainsKey(player))
        {
            bool removed = PlayerData.TryRemove(player, out _);

            if (!removed) return false;

            return true;
        }
        else return false;
    }

    /// <summary>
    /// Called when a message is recieved from the server
    /// </summary>
    /// <param name="ar"></param>
    private void ReadCallback(IAsyncResult ar)
    {
        if (handler is null) throw new NullReferenceException();

        string content = string.Empty;

        int bytesRead = handler.EndReceive(ar);

        if (bytesRead > 0)
        {
            ArrayExtensions.Merge(serverLongBuffer, serverBuffer, serverLongBufferSize);
            serverLongBufferSize += bytesRead;

        ReprocessBuffer:

            if (
                serverCurrentPacketLength == -1
                && serverLongBufferSize >= PacketBuilder.PacketLenLen
            )
            {
                serverCurrentPacketLength = PacketBuilder.GetPacketLength(serverLongBuffer);
            }

            if (
                serverCurrentPacketLength != -1
                && serverLongBufferSize >= serverCurrentPacketLength
            )
            {
                ContentQueue.Enqueue(
                    ArrayExtensions.Slice(serverLongBuffer, 0, serverCurrentPacketLength)
                );
                byte[] new_buffer = new byte[1024];
                ArrayExtensions.Merge(
                    new_buffer,
                    ArrayExtensions.Slice(serverLongBuffer, serverCurrentPacketLength, 1024),
                    0
                );
                serverLongBuffer = new_buffer;
                serverLongBufferSize -= serverCurrentPacketLength;
                serverCurrentPacketLength = -1;
                if (serverLongBufferSize > 0)
                {
                    goto ReprocessBuffer;
                }
            }
        }

        handler.BeginReceive(serverBuffer, 0, 1024, 0, new AsyncCallback(ReadCallback), null); // Listen again
    }

    void ReceiveLoop()
    {
        try
        {
            while (true)
            {
                if (ContentQueue.IsEmpty)
                {
                    Thread.Sleep(RECEIVE_COOLDOWN);
                    continue;
                } // Nothing recieved

                byte[] content;
                if (!ContentQueue.TryDequeue(out content))
                {
                    continue;
                }

                try
                { 
                    Packet packet = PacketBuilder.Decode(content);
                    bool handled = internalPacketHandler.TryHandlePacket(packet);

                    if (!handled)
                    {

                    }
                }
                catch (PacketDecodeError)
                {

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
    /// <param name="payload">Data to send</param>
    public void SendMessage(byte[] payload)
    {
        SendQueue.Enqueue(payload);
    }

    /// <summary>
    /// Sends queued messages
    /// </summary>
    private void SendLoop()
    {
        if (handler is null) throw new NullReferenceException();

        try
        {
            while (true)
            {
                if (!SendQueue.IsEmpty)
                {
                    byte[] to_send;
                    if (SendQueue.TryDequeue(out to_send))
                    {
                        handler.Send(to_send);
                    }
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
    /// Disconnect client from server
    /// </summary>
    /// <param name="reason"></param>
    public void Disconnect(string reason)
    {
        Shutdown();
    }

    /// <summary>
    /// Shutdown client
    /// </summary>
    public void Shutdown()
    {
        if (connectionThread.ThreadState == ThreadState.Running) connectionThread.Abort();
        receiveThread.Abort();
        sendThread.Abort();
        handler?.Shutdown(SocketShutdown.Both);
        handler?.Close();
    }
}