using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace MainMenu
{
    /// <summary>
    /// Script attatched to every item in the save selector dropdown
    /// Stores the item's save path
    /// </summary>
    public class SaveSelectorItem : MonoBehaviour
    {
        [SerializeField] private SaveSelector SaveSelector;

         public string Filename;

        [SerializeField] private Button Button;
        public TMP_Text Text;

        /*
        public Action OnClick = () => Debug.LogWarning("No action set");
        */

        public void OnClickCall() => SaveSelector.SelectFile(Filename);
    }
}

