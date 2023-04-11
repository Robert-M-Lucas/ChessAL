using UnityEngine;
using TMPro;
using UnityEngine.Serialization;

/// <summary>
/// Class that makes text's font size tied to another font asset
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class InheritFontSize : MonoBehaviour
{
    [FormerlySerializedAs("InheritFrom")] [SerializeField] private TMP_Text inheritFrom;

    private TMP_Text text = null;

    // Copy and update font size
    private void OnEnable()
    {
        text ??= GetComponent<TMP_Text>();
        text.enableAutoSizing = false;
    }

    private void Start()
    {
        if (text is null) text = GetComponent<TMP_Text>();
        text.enableAutoSizing = false;
    }

    private void OnValidate()
    {
        text = GetComponent<TMP_Text>();
        text.fontSize = inheritFrom.fontSize;
    }

    void Update()
    {
        // ReSharper disable once CompareOfFloatsByEqualityOperator
        if (text.fontSize != inheritFrom.fontSize)
        {
            text.fontSize = inheritFrom.fontSize;
        }
    }
}
