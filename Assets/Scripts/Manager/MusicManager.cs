using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    [SerializeField] [Range(0, 100)] private int _volume = 50;
    public int Volume {
        get {
            return _volume;
        }
        set {
            if(value > 100) {
                value = 100;
            }
            else if(value < 0) {
                value = 0;
            }
            _volume = value;
            audioSrc.volume = value;
        }
    }

    public AudioSource audioSrc;

    [Header ("Music Clips")]
    public AudioClip introQueueTrack;
    public AudioClip loopTrack;

    private LerpClass fadeLerp = new LerpClass(1f);
    private float volFrom, volTo;

    void Start()
    {
        if(introQueueTrack != null) {
            audioSrc.clip = introQueueTrack;
            volFrom = 0f;
            volTo = (float)Volume;
            fadeLerp.ActivateLerp();
            audioSrc.Play();
        }
    }

    private void Update() {
        if (fadeLerp.IsLerping)
        {
            float val = fadeLerp.LerpValues(volFrom, volTo);
            Volume = Mathf.RoundToInt (val);
        }
        if(audioSrc.clip == introQueueTrack && audioSrc.isPlaying == false) {
            audioSrc.clip = loopTrack;
            audioSrc.Play();
            audioSrc.loop = true;
        }
        audioSrc.volume = Volume/100f;
    }
}
