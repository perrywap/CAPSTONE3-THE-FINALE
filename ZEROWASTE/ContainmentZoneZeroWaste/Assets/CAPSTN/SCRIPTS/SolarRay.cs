using System.Collections;
using UnityEngine;

public class SolarRay : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float duration;

    public void Fired()
    {
        animator.SetBool("isFiring", true);
        StartCoroutine(RayDuration());
    }

    public void DestroyRay()
    {
        Destroy(gameObject);
    }

    private IEnumerator RayDuration()
    {
        CameraShake.Instance.Shake(duration);
        yield return new WaitForSeconds(duration);
        animator.SetBool("isFiring", false);
        animator.SetBool("fireEnd", true);
    }
}
