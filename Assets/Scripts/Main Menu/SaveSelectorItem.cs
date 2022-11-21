using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace MainMenu
{
    public class SaveSelectorItem : MonoBehaviour
    {
        public SaveSelector SaveSelector;

        public string Filename;

        public Button Button;
        public TMP_Text Text;

        public Action OnClick = () => Debug.LogWarning("No action set");

        public void OnClickCall() => SaveSelector.SelectFile(Filename);
    }
}

