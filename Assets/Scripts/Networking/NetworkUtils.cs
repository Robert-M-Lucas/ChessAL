using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Networking
{
    public static class NetworkingUtils
    {
        /// <summary>
        /// Polls a socket to see if it is still connected. Used for kicking clients that didn't leave gracefully
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool SocketConnected(Socket s)
        {
            var part1 = s.Poll(1000, SelectMode.SelectRead);
            var part2 = (s.Available == 0);
            if (part1 && part2)
                return false;
            else
                return true;
        }

        /// <summary>
        /// Checks if port is available for server hosting
        /// </summary>
        /// <param name="port"></param>
        /// <returns></returns>
        public static bool PortInUse(int port)
        {
            var ip_properties = IPGlobalProperties.GetIPGlobalProperties();
            var ip_end_points = ip_properties.GetActiveTcpListeners();


            return ip_end_points.Any(endPoint => endPoint.Port == port);
        }
    }
}