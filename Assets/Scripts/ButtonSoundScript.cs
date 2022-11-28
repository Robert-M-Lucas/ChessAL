using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSoundScript : MonoBehaviour
{
    public void OnClick() => SoundMananger.GetInstance().PlayOnClickSound();
}
