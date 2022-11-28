using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMananger : MonoBehaviour
{
    private static SoundMananger instance = null;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] PieceMoveSounds;
    [SerializeField] private AudioClip OnClickSound;

    [Header("References")]
    [SerializeField] private GameObject Soundling; // Audio source

    private void Awake()
    {
        // Singleton setup
        if (instance is not null) DestroyImmediate(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Singleton
    public static SoundMananger GetInstance() => instance;

    /// <summary>
    /// Plays UI click sound
    /// </summary>
    public void PlayOnClickSound() => PlaySound(OnClickSound);

    
    int _previous = -1;
    /// <summary>
    /// Plays random piece move sound
    /// </summary>
    public void PlayPieceMoveSound()
    {
        int r = Random.Range(0, PieceMoveSounds.Length - 1);

        // Ensure same sound isn't played twice in a row
        if (r == _previous)
        {
            r += 1;
            if (r >= PieceMoveSounds.Length) { r = 0; }
        }
        _previous = r;
        PlaySound(PieceMoveSounds[r]);
    }

    /// <summary>
    /// Plays a sound
    /// </summary>
    /// <param name="clip"></param>
    public void PlaySound(AudioClip clip)
    {
        // Instantiate a 'soundling' to play sound so multiple sounds can play at once
        GameObject soundling = Instantiate(Soundling);

        // soundling.transform.SetParent(Camera.main.gameObject.transform);
        // soundling.transform.localPosition = Vector3.zero;

        soundling.transform.position = Camera.main.transform.position;

        AudioSource audioSource = soundling.GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.Play();

        // Allow soundling to play across scenes
        DontDestroyOnLoad(soundling);

        // Delete the soundling once it is no longer needed
        StartCoroutine(DeleteSoundling(soundling, clip.length));
    }

    /// <summary>
    /// Deletes the soundling after a delay
    /// </summary>
    /// <param name="soundling"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator DeleteSoundling(GameObject soundling, float delay)
    {
        yield return new WaitForSeconds(delay + 1);
        Destroy(soundling);
    }
}
