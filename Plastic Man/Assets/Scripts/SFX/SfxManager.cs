using System.Collections;
using UnityEngine;

public class SfxManager : MonoBehaviour
{
    public static SfxManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void PlaySFX(AudioClip audioClip, float volume = 1f)
    {
        StartCoroutine(PlaySFXCoroutine(audioClip, volume));
    }

    IEnumerator PlaySFXCoroutine(AudioClip audioClip, float volume = 1f)
    {
        AudioSource audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.Play();

        yield return new WaitForSeconds(audioClip.length);

        Destroy(audioSource);
    }
    public AudioSource PlayLoop(AudioClip clip, float volume = 1f)
    {
        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.loop = true;
        source.Play();

        return source;
    }

    public void StopLoop(AudioSource source)
    {
        if (source != null)
        {
            source.Stop();
            Destroy(source);
        }
    }
}