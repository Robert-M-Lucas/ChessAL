using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Networking;

public class VersionTextDisplay : MonoBehaviour
{
    public TMP_Text VersionText;
    
    void Start()
    {
        VersionText.text = $"{Application.version} - {NetworkSettings.VERSION}";    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
