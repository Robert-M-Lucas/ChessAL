using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;

namespace MainMenu
{
    /// <summary>
    /// Script attatched to every item in the save selector dropdown
    /// Stores the item's save path
    /// </summary>
    public class SaveSelectorItem : MonoBehaviour
    {
        [FormerlySerializedAs("SaveSelector")] [SerializeField] private SaveSelector saveSelector;

         public string Filename;

         // ReSharper disable once NotAccessedField.Local
         [FormerlySerializedAs("Button")] [SerializeField] private Button button;
        public TMP_Text Text;

        /*
        public Action OnClick = () => Debug.LogWarning("No action set");
        */

        public void OnClickCall() => saveSelector.SelectFile(Filename);
    }
}

