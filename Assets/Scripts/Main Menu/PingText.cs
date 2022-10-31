using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MainMenu
{
    public class PingText : MonoBehaviour
    {
        public TMP_Text Text;
        public ChessManager ChessManager;

        private float time = 0f;

        private int ping = -1;

        public void PingUpdate(int ping)
        {
            this.ping = ping;
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.time - time > 1f)
            {
                time = Time.time;
                Text.text = "Ping: _ms";
                ChessManager.GetPing(PingUpdate);
            }

            if (ping != -1)
            {
                Text.text = $"Ping: {ping}ms";
                ping = -1;
            }
        }
    }

}
