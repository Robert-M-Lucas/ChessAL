using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Class that makes text's font size tied to another font asset
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class InheritFontSize : MonoBehaviour
{
    [SerializeField] private TMP_Text InheritFrom;

    private TMP_Text _text = null;

    // Copy and update font size

    private void OnEnable()
    {
        if (_text is null) _text = GetComponent<TMP_Text>();
        _text.enableAutoSizing = false;
    }

    private void Start()
    {
        if (_text is null) _text = GetComponent<TMP_Text>();
        _text.enableAutoSizing = false;
    }

    private void OnValidate()
    {
        _text = GetComponent<TMP_Text>();
        _text.fontSize = InheritFrom.fontSize;
    }

    void Update()
    {
        if (_text.fontSize != InheritFrom.fontSize)
        {
            _text.fontSize = InheritFrom.fontSize;
        }
    }
}
