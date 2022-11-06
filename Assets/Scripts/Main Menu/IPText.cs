using System.Collections;
using System.Threading;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Networking;
using System.Net.Sockets;
using System.Net;

namespace MainMenu
{
    public class IPText : MonoBehaviour
    {
        public TMP_Text Text;

        string ipText = "";
        bool set = false;

        void Start()
        {
            new Thread(GetIP).Start();
        }

        void GetIP()
        {
            string public_ip = new System.Net.WebClient().DownloadString(NetworkSettings.PUBLIC_IP_SOURCE);
            string private_ip = "Not found";

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    private_ip = ip.ToString();
                    break;
                }
            }

            ipText = $"Public IP: {public_ip}\nPrivate IP: {private_ip}";
        }

        private void Update()
        {
            if (!set && ipText != string.Empty)
            {
                Text.text = ipText;
                set = true;
            }
        }
    }

}