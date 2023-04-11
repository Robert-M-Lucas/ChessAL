using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance = null;

    [FormerlySerializedAs("PieceMoveSounds")]
    [Header("Audio Clips")]
    [SerializeField] private AudioClip[] pieceMoveSounds;
    [FormerlySerializedAs("OnClickSound")] [SerializeField] private AudioClip onClickSound;

    [FormerlySerializedAs("Soundling")]
    [Header("References")]
    [SerializeField] private GameObject soundling; // Audio source

    private void Awake()
    {
        // Singleton setup
        if (instance is not null) DestroyImmediate(gameObject);
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Persist between scenes
        }
    }

    // Singleton
    public static SoundManager GetInstance() => instance;

    /// <summary>
    /// Plays UI click sound
    /// </summary>
    public void PlayOnClickSound() => PlaySound(onClickSound);

    
    int previous = -1;
    /// <summary>
    /// Plays random piece move sound
    /// </summary>
    public void PlayPieceMoveSound()
    {
        // Select random sound index
        var r = Random.Range(0, pieceMoveSounds.Length - 1);

        // Ensure same sound isn't played twice in a row
        if (r == previous)
        {
            r += 1;
            if (r >= pieceMoveSounds.Length) { r = 0; }
        }

        previous = r;

        PlaySound(pieceMoveSounds[r]);
    }

    /// <summary>
    /// Plays a sound
    /// </summary>
    /// <param name="clip"></param>
    public void PlaySound(AudioClip clip)
    {
        // Instantiate a 'soundling' to play sound so multiple sounds can play at once
        var new_soundling = Instantiate(this.soundling);

        // Set soundling to camera position to create stereo audio at proper volume
        new_soundling.transform.position = Camera.main!.transform.position;

        // Set and play sound
        var audio_source = new_soundling.GetComponent<AudioSource>();
        audio_source.clip = clip;
        audio_source.Play();

        // Allow soundling to play across scenes
        DontDestroyOnLoad(new_soundling);

        // Delete the soundling once it is no longer needed
        StartCoroutine(DeleteSoundling(new_soundling, clip.length));
    }

    /// <summary>
    /// Deletes the soundling after a delay
    /// </summary>
    /// <param name="passedSoundling"></param>
    /// <param name="delay"></param>
    /// <returns></returns>
    private static IEnumerator DeleteSoundling(GameObject passedSoundling, float delay)
    {
        yield return new WaitForSeconds(delay + 1);
        Destroy(passedSoundling);
    }
}
