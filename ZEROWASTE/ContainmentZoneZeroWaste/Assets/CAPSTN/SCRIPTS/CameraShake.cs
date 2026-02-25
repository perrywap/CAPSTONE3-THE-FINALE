using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get;  private set; }

    [SerializeField] private Animator animator;

    private void Awake()
    {
        Instance = this;
        animator = GetComponent<Animator>();
    }

    public void Shake(float duration)
    {
        StartCoroutine(StartShake(duration));
    }

    public IEnumerator StartShake(float duration)
    {
        animator.SetBool("isShaking", true);
        yield return new WaitForSeconds(duration);
        animator.SetBool("isShaking", false);
    }
}
