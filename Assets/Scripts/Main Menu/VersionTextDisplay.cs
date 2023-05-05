using UnityEngine;
using TMPro;
using Networking;

namespace MainMenu
{
    /// <summary>
    /// Shows the current application and networking version on the main menu
    /// </summary>
    public class VersionTextDisplay : MonoBehaviour
    {
        public TMP_Text VersionText;

        void Start()
        {
            VersionText.text = $"V{Application.version} - N{NetworkSettings.VERSION}";
        }
    }
}