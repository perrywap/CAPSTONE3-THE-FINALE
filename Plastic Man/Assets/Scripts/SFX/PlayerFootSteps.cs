using UnityEngine;
using System.Collections;

public class PlayerFootSteps : MonoBehaviour
{
    public AudioClip[] footStepSfx;
    [SerializeField] private float footStepsVolume = 0.005f;
    private Coroutine footstepRoutine;

    public bool isMoving;

    void Update()
    {
        if (isMoving && footstepRoutine == null)
        {
            footstepRoutine = StartCoroutine(PlayFootsteps());
        }

        if (!isMoving && footstepRoutine != null)
        {
            StopCoroutine(footstepRoutine);
            footstepRoutine = null;
        }
    }

    IEnumerator PlayFootsteps()
    {
        while (true)
        {
            AudioClip clip = GetRandomClip();

            if (clip != null)
            {
                SfxManager.instance.PlaySFX(clip, footStepsVolume);
            }

            yield return new WaitForSeconds(Random.Range(0.15f, 0.25f));
        }
    }
    AudioClip GetRandomClip()
    {
        if (footStepSfx.Length == 0) return null;
        return footStepSfx[Random.Range(0, footStepSfx.Length)];
    }
}