using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class InheritFontSize : MonoBehaviour
{
    public TMP_Text InheritFrom;

    private TMP_Text _text = null;

    private void OnEnable()
    {
        if (_text is null) _text = GetComponent<TMP_Text>();
        _text.enableAutoSizing = false;
    }

    private void Start()
    {
        _text = GetComponent<TMP_Text>();
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
