using System.Collections;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
	public static SoundManager instance;

    [HideInInspector] public SoundData[] soundLibrary;
    public int maxAudioSources = 5; 

    private AudioSource[] audioSources;

	// flags
	bool playingAudioRoutine = false;

    void Awake()
    {
		if (instance == null)
        	instance = this;
    }

    void Start ()
    {
        InitializeAudioSources ();
        InitializeSoundLibrary ();
    }

	public void PlaySound (string soundName)
	{
        SoundData sound = System.Array.Find(soundLibrary, s => s.soundName == soundName);

        if (sound != null)
        {
            AudioSource availableAudioSource = GetAvailableAudioSource();

            if (availableAudioSource != null)
            {
                // Reproducir el sonido usando el audioClip y el volumen
                availableAudioSource.clip = sound.audioClip;
                availableAudioSource.volume = sound.volume;
                availableAudioSource.Play();
            }
            else
            {
                Debug.LogWarning("All audio sources are busy. Cannot play sound: " + soundName);
            }
        }
        else
        {
            Debug.LogWarning("Sound not found: " + soundName);
        }
	}

    public float GetSoundDuration (string soundName)
    {
        SoundData sound = System.Array.Find(soundLibrary, s => s.soundName == soundName);
        if (sound != null)
        {
            return sound.audioClip.length;
        }
        else
        {
            Debug.LogWarning ("Sound Not Found");    
            return 0;
        }
    }

    public void PlaySoundXTimes (string sound, int _times)
    {
		if (!playingAudioRoutine)
			StartCoroutine (PlaySoundXTimesRoutine (sound, _times));
    }

    void InitializeAudioSources()
    {
        audioSources = new AudioSource[maxAudioSources];

        for (int i = 0; i < maxAudioSources; i++)
        {
            audioSources[i] = gameObject.AddComponent<AudioSource>();
        }
    }

    void InitializeSoundLibrary ()
    {
        soundLibrary = Resources.LoadAll<SoundData>("Sounds");
    }

    AudioSource GetAvailableAudioSource()
    {
        for (int i = 0; i < maxAudioSources; i++)
        {
            if (!audioSources[i].isPlaying)
            {
                return audioSources[i];
            }
        }
        return null; // Returns null if all of the audiosources are busy
    }

	IEnumerator PlaySoundXTimesRoutine (string sound, int _times)
	{
		if (_times < 0)
			yield return 0;

		playingAudioRoutine = true;

		for (int i = 0; i < _times; i++)
		{
			PlaySound (sound);
			yield return new WaitForSeconds (GetSoundDuration(sound));
		}

		playingAudioRoutine = false;
	}
}