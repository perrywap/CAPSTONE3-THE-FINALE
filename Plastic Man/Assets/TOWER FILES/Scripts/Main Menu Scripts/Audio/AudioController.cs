using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource soundSource;
    public AudioMixer audioMixer;

    public void PlayAudio(AudioClip music, AudioClip sound)
    {
        if (sound != null)
        {
            soundSource.PlayOneShot(sound); 
        }

        if (music != null && musicSource.clip != music)
        {
            StartCoroutine(SwitchMusic(music));
        }
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("Volume", volume);
    }

    private IEnumerator SwitchMusic(AudioClip music)
    {
        if (musicSource.clip != null)
        {
            while (musicSource.volume > 0)
            {
                musicSource.volume -= 0.05f;
                yield return new WaitForSeconds(0.05f);
            }
        }
        else
        {
            musicSource.volume = 0;
        }

        musicSource.clip = music;
        musicSource.Play();

        while (musicSource.volume < 1.0f)
        {
            musicSource.volume += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }
    }
}
