using System.Threading;
using UnityEngine;
using TMPro;
using Networking;
using System.Net.Sockets;
using System.Net;
using System;

namespace MainMenu
{
    /// <summary>
    /// Finds public and private IP and displays it on a font asset
    /// </summary>
    public class IPText : MonoBehaviour
    {
        public TMP_Text Text;

        private string ipText = "";
        private bool set = false;

        private void Start()
        {
            // Start looking for IP on thread
            new Thread(GetIP).Start();
        }

        private void GetIP()
        {
            string public_ip;
            try
            {
                public_ip = new WebClient().DownloadString(NetworkSettings.PUBLIC_IP_SOURCE); // Download IP from source
                if (public_ip.Length > 20) throw new FormatException("Recieved public IP incorrectly formatted");
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                public_ip = "Not found";
            }

            var private_ip = "Not found";

            // Get local IP
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    private_ip = ip.ToString();
                    break;
                }
            }

            ipText = $"Public IP: {public_ip}\nLocal IP: {private_ip}";
        }

        private void Update()
        {
            // When IP found
            if (!set && ipText != string.Empty)
            {
                Text.text = ipText;
                set = true;
            }
        }
    }

}