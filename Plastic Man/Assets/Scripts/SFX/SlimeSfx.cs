using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SlimeSfx : MonoBehaviour
{
    [Header("Audio Settings")]
    [SerializeField] private AudioClip slimeLoopSfx;
    [SerializeField] private float volume = 0.5f;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = slimeLoopSfx;
        audioSource.volume = volume;
        audioSource.loop = true;  
        audioSource.playOnAwake = false;
    }

    void OnEnable()
    {
        if (slimeLoopSfx != null)
            audioSource.Play();
    }

    void OnDisable()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
    }
    public void StopLoop()
    {
        if (audioSource.isPlaying)
            audioSource.Stop();
    }
}