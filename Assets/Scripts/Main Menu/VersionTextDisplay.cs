using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Networking;

namespace MainMenu
{
    public class VersionTextDisplay : MonoBehaviour
    {
        public TMP_Text VersionText;

        void Start()
        {
            VersionText.text = $"{Application.version} - {NetworkSettings.VERSION}";
        }
    }
}