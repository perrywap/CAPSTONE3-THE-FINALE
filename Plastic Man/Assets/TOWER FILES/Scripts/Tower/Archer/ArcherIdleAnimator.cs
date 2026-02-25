using System.Collections;
using UnityEngine;

public class ArcherIdleAnimator : MonoBehaviour
{
    [SerializeField] private Sprite[] idleFrames;
    [SerializeField] private Sprite baseArcherSprite;
    [SerializeField] private float frameRate = 0.2f;

    private SpriteRenderer spriteRenderer;
    private Coroutine animationCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void StartIdle()
    {
        if (animationCoroutine == null && idleFrames.Length > 0)
        {
            animationCoroutine = StartCoroutine(PlayIdleAnimation());
        }
    }

    public void StopIdle()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }

        if (baseArcherSprite != null)
        {
            spriteRenderer.sprite = baseArcherSprite;
        }
    }

    private IEnumerator PlayIdleAnimation()
    {
        int index = 0;
        while (true)
        {
            spriteRenderer.sprite = idleFrames[index];
            index = (index + 1) % idleFrames.Length;
            yield return new WaitForSeconds(frameRate);
        }
    }
}
