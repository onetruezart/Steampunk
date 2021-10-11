using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundMusicAudioSourse;

    public static AudioController instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void SetMusic(string name)
    {
        AudioClip music = Resources.Load<AudioClip>("Music/" + name);
        if (music != null)
        {
            backgroundMusicAudioSourse.Stop();
            backgroundMusicAudioSourse.clip = music;
            backgroundMusicAudioSourse.Play();
        }
    }

}
