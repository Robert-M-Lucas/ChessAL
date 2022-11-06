using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace MainMenu
{
    /// <summary>
    /// A replacement for Unity's built-in dropdown
    /// Gives the functionality of a dropdown while being easier to scale with screen sizes
    /// </summary>
    public class Selector : MonoBehaviour
    {
        public List<string> Options;
        public TMP_Text OptionText;
        public string CurrentlyShowing { get { return Options[CurrentlyShowingPos]; } }
        public int CurrentlyShowingPos { get; private set; } = 0;

    private void OnEnable()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            if (Options.Count == 0) Options.Add("Select Option");
            OptionText.text = Options[0];
        }

        void Start()
        {
            OptionText.text = Options[0];
            CurrentlyShowingPos = 0;
        }

        public void Next()
        {
            CurrentlyShowingPos++;
            if (CurrentlyShowingPos >= Options.Count) CurrentlyShowingPos = Options.Count - 1;
            OptionText.text = Options[CurrentlyShowingPos];
        }

        public void Prev()
        {
            CurrentlyShowingPos--;
            if (CurrentlyShowingPos < 0) CurrentlyShowingPos = 0;
            OptionText.text = Options[CurrentlyShowingPos];
        }
    }
}
