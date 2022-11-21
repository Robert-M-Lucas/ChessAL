using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;

namespace MainMenu
{
    /// <summary>
    /// Creates a dropdown for selecting save files
    /// </summary>
    public class SaveSelector : MonoBehaviour
    {
        // Filenames over this length will be truncated
        const int MAX_FILENAME_LENGTH = 20;

        public Canvas Canvas;

        public GameObject Selector;
        public GameObject SelectorListParent;

        public SaveSelectorItem SaveSelectorItemPrefab;

        public GameObject SelectButton;
        public TMP_Text SelectedText;

        public string SelectedFile { get; private set; } = string.Empty;

        private bool showingSelector = false;

        /// <summary>
        /// Resizes the list items to the current screen size
        /// </summary>
        public void UpdateItemScale()
        {
            float height = RectTransformUtility.PixelAdjustRect(SelectButton.GetComponent<RectTransform>(), Canvas).height;
            RectTransform rect = SaveSelectorItemPrefab.GetComponent<RectTransform>();
            rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
        }

        /// <summary>
        /// Shows and hides the list of saves
        /// </summary>
        public void ExpandOrCollapse()
        {
            showingSelector = !showingSelector;
            Selector.SetActive(showingSelector);

            if (showingSelector) UpdateSavesList();
        }

        /// <summary>
        /// Called when a file is selected
        /// </summary>
        /// <param name="file"></param>
        public void SelectFile(string file)
        {
            string shortened_filename = Path.GetFileNameWithoutExtension(file); ;
            if (shortened_filename.Length > MAX_FILENAME_LENGTH)
            {
                shortened_filename = shortened_filename[..(MAX_FILENAME_LENGTH - 3)] + "...";
            }
            SelectedText.text = shortened_filename;

            SelectedFile = file;

            ExpandOrCollapse();
        }

        /// <summary>
        /// Updates the list of saves
        /// </summary>
        public void UpdateSavesList()
        {
            string[] save_files = SaveSystem.ListAllSaveFiles();
            string[] save_file_names = new string[save_files.Length];
            
            for (int i = 0; i < save_files.Length; i++)
            {
                save_file_names[i] = Path.GetFileNameWithoutExtension(save_files[i]);
                if (save_file_names[i].Length > MAX_FILENAME_LENGTH)
                {
                    save_file_names[i] = save_file_names[i][..(MAX_FILENAME_LENGTH-3)] + "...";
                }
            }

            for (int i = 0; i < SelectorListParent.transform.childCount; i++)
            {
                if (SelectorListParent.transform.GetChild(i).gameObject.activeSelf) Destroy(SelectorListParent.transform.GetChild(i).gameObject);
            }

            UpdateItemScale();

            for (int i = 0; i < save_file_names.Length; i++)
            {
                SaveSelectorItemPrefab.Text.text = save_file_names[i];
                SaveSelectorItemPrefab.Filename = save_files[i];
                GameObject new_item = Instantiate(SaveSelectorItemPrefab.gameObject);
                new_item.transform.SetParent(SelectorListParent.transform);
                new_item.SetActive(true);
            }
        }
    }

}
