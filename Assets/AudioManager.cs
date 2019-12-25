using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    
    public AudioSource musicSource;
    public AudioClip musicClip;

    public GameObject btnMusicOn, btnMusicOff;

    void Awake()
    {
        instance = this;
    }
    
    void Start()
    {
        musicSource.clip = musicClip;
    }

    public void StartMusic()
    {
        musicSource.Play();
        btnMusicOn.SetActive(true);
        btnMusicOff.SetActive(false);
    }
    
    public void UnmuteMusic()
    {
        musicSource.volume = 1;
        btnMusicOn.SetActive(true);
        btnMusicOff.SetActive(false);
    }

    public void MuteMusic()
    {
        musicSource.volume = 0;
        btnMusicOn.SetActive(false);
        btnMusicOff.SetActive(true);
    }
}
