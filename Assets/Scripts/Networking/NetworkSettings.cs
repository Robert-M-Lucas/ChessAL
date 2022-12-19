using System.Net.Sockets;

namespace Networking
{
    /// <summary>
    /// Contains network configuration
    /// </summary>
    public static class NetworkSettings
    {
        public const int PORT = 8108;
        public const string VERSION = "0.2.5";
        public const string PUBLIC_IP_SOURCE = "https://ipinfo.io/ip";

        /// <summary>
        /// Configures socket settings for usage
        /// </summary>
        /// <param name="s"></param>
        public static void ConfigureSocket(Socket s)
        {
            s.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            s.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
        }
    }
}