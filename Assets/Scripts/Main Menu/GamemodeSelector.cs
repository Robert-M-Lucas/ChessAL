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
    public class GamemodeSelector : MonoBehaviour
    {
        public List<string> Options;
        public TMP_Text OptionText;
        public string CurrentlyShowing { get { return Options[CurrentlyShowingPos]; } }
        public int CurrentlyShowingPos { get; private set; } = 0;


        private void OnEnable()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                OnValidate();
            }
#endif
            Start();
        }

        // Editor utility to autopopulate list if empty
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (Options.Count == 0)
            {
                Options.Add("Select Option");
                OptionText.text = Options[0];
            }
        }
#endif

        void Start()
        {
            OptionText.text = Options[0];
            CurrentlyShowingPos = 0;
        }

        public void Next()
        {
            CurrentlyShowingPos++;
            if (CurrentlyShowingPos >= Options.Count) CurrentlyShowingPos = 0;
            OptionText.text = Options[CurrentlyShowingPos];
        }

        public void Prev()
        {
            CurrentlyShowingPos--;
            if (CurrentlyShowingPos < 0) CurrentlyShowingPos = Options.Count - 1;
            OptionText.text = Options[CurrentlyShowingPos];
        }
    }
}
