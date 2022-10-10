using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

public class Server
{
    private ServerGameData gameData;
    private string serverPassword;

    private Thread acceptClientThread;
    private Thread recieveThread;
    private Thread sendThread;

    private ConcurrentQueue<Tuple<int, byte[]>> recieveQueue = new ConcurrentQueue<Tuple<int, byte[]>>();
    private ConcurrentQueue<Tuple<int, byte[]>> sendQueue = new ConcurrentQueue<Tuple<int, byte[]>>();

    public bool AcceptingClients = false;
    public int ClientsConnected { get; private set; } = 0;

    public Server(ServerGameData gameData, string serverPassword)
    {
        acceptClientThread = new Thread(AcceptClients);
        recieveThread = new Thread(RecieveLoop);
        sendThread = new Thread(SendLoop);

        this.gameData = gameData;
        this.serverPassword = serverPassword;
    }

    /// <summary>
    /// Starts the server
    /// </summary>
    public void Start()
    {
        Debug.Log("Server starting");
        AcceptingClients = true;
        acceptClientThread.Start();
        recieveThread.Start();
        sendThread.Start();
    }

    private void AcceptClients()
    {
        IPAddress ipAddress = IPAddress.Any;

        IPEndPoint localEndPoint = new IPEndPoint(ipAddress, NetworkSettings.PORT);

        Socket listener;

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
                        ServerKickPacket.Build(0, "Server is not accepting clients at this time")
                    );
                    continue;
                }

                // Password incorrect
                if (serverPassword != "" && initPacket.Password != serverPassword)
                {
                    handler.Send(
                        ServerKickPacket.Build(0, "Wrong Password: '" + initPacket.Password + "'")
                    );
                    continue;
                }

                // Version mismatch
                if (initPacket.Version != NetworkSettings.VERSION)
                {
                    handler.Send(
                        ServerKickPacket.Build(
                            0,
                            "Wrong Version:\nServer: "
                                + NetworkSettings.VERSION.ToString()
                                + "| Client (You): "
                                + initPacket.Version
                        )
                    );
                    continue;
                }

                ServerPlayer player = new ServerPlayer();
                player.Name = initPacket.Name;

                AddPlayer(player);

                SendMessage(player.ID, ServerConnectAcceptPacket.Build(0, player.ID));

                // Begin recieving communications from client
                handler.BeginReceive(
                    player.Buffer,
                    0,
                    1024,
                    0,
                    new AsyncCallback(ReadCallback),
                    player
                );
            }
        }
        catch (ThreadAbortException) { }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    private void ReadCallback(IAsyncResult ar)
    {
        string content = string.Empty;

        ServerPlayer CurrentPlayer = (ServerPlayer)ar.AsyncState;
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
                        CurrentPlayer.ID,
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

    private void RecieveLoop()
    {
    }

    public void SendMessage(int ID, byte[] message)
    {
        sendQueue.Enqueue(new Tuple<int, byte[]>(ID, message));
    }

    private void SendLoop()
    {
    }
}