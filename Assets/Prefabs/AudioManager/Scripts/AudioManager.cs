using System;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    private float _loopStart = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (musicSource.isPlaying && musicSource.time >= (musicSource.clip.length - 1f))
        {
            musicSource.time = _loopStart;
            musicSource.Play();
        }
    }

    public void PlayMusic(string songName, float loopStart)
    {
        var music = musicSounds.FirstOrDefault(x => x.Name == songName);

        if (music == null) return;

        _loopStart = loopStart;
        musicSource.clip = music.Clip;
        musicSource.Play();
    }

    public void PlaySfx(string sfxName)
    {
        var sfx = sfxSounds.FirstOrDefault(x => x.Name == sfxName);

        if (sfx == null) return;
        
        sfxSource.PlayOneShot(sfx.Clip);
    }

    public void ChangeMusicVolume(float volume)
    {
        musicSource.volume = volume;
    }
    
    public void ChangeSfxVolume(float volume)
    {
        sfxSource.volume = volume;
    }
}

[Serializable]
public class Sound
{
    public string Name;
    public AudioClip Clip;
}