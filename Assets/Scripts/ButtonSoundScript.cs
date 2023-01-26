using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Plays sound when button is pressed
/// </summary>
public class ButtonSoundScript : MonoBehaviour
{
    public void OnClick() => SoundMananger.GetInstance().PlayOnClickSound();
}
