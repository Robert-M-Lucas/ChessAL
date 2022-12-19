using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

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
            bool part1 = s.Poll(1000, SelectMode.SelectRead);
            bool part2 = (s.Available == 0);
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
            IPGlobalProperties ipProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] ipEndPoints = ipProperties.GetActiveTcpListeners();


            foreach (IPEndPoint endPoint in ipEndPoints)
            {
                if (endPoint.Port == port) // Port found
                {
                    return true;
                }
            }
            return false;
        }
    }
}