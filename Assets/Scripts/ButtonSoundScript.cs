using UnityEngine;

/// <summary>
/// Plays sound when button is pressed
/// </summary>
public class ButtonSoundScript : MonoBehaviour
{
    public void OnClick() => SoundManager.GetInstance().PlayOnClickSound();
}
