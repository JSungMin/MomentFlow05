using UnityEngine;
using System.Collections;

/// <summary>
/// Sound player prepack for demonstration CSV dialog system features
/// </summary>
public class SoundLoader : MonoBehaviour
{
    public static SoundLoader Instance;                                             // Singleton

    public AudioSource audioSource;                                                 // Link to sound source

    private string soundFolder = "Sounds/";                                         // Folder for sound load

    void Awake()
    {
        Instance = this;                                                            // Make singleton Instance
    }

    void Start()
    {
        if (audioSource == null)
        {
            Debug.LogError("Wrong default settings");
            return;
        }
    }

    /// <summary>
    /// Play sound by it name
    /// </summary>
    /// <param name="soundName"> Name of sound </param>
    public void PlaySound(string soundName)
    {
        if (soundName == null)
        {
            Debug.Log("Wrong input data");
            return;
        }
        AudioClip newClip = Resources.Load<AudioClip>(soundFolder + soundName);     // Try to load sound by its name
        if (newClip == null)
        {
            Debug.LogError("Can not load such sound");
            return;
        }
        audioSource.clip = newClip;                                                 // Place sound to audio source
        audioSource.Play();                                                         // Play it
    }
}
