using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }

    [SerializeField] private AudioSource src;
    [SerializeField] private AudioClip[] clip;

    private void Awake()
    {
        src.clip = clip[0];
        src.Play();
    }
}
