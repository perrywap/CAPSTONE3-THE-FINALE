using System;
using System.Collections;
using UnityEngine;

public class FrostTowerAnimator : MonoBehaviour
{
    [Header("Idle Animation")]
    [SerializeField] private Sprite[] idleFrames;
    [SerializeField] private float idleFrameRate = 0.15f;

    [Header("Attack Animation")]
    [SerializeField] private Sprite[] attackFrames;
    [SerializeField] private float attackFrameRate = 0.25f;

    public Action OnCastFrameReached;

    private SpriteRenderer spriteRenderer;
    private Coroutine animationCoroutine;
    private string currentState = "";

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void PlayIdle()
    {
        if (currentState == "Idle") return;
        currentState = "Idle";

        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(LoopAnimation(idleFrames, idleFrameRate, isAttack: false));
    }

    public void PlayAttack()
    {
        if (currentState == "Attack") return;
        currentState = "Attack";

        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(LoopAnimation(attackFrames, attackFrameRate, isAttack: true));
    }

    private IEnumerator LoopAnimation(Sprite[] frames, float frameRate, bool isAttack)
    {
        int index = 0;
        while (true)
        {
            if (frames.Length == 0) yield break;

            spriteRenderer.sprite = frames[index];

            if (isAttack && index == 0)
            {
                OnCastFrameReached?.Invoke();
            }

            index = (index + 1) % frames.Length;
            yield return new WaitForSeconds(frameRate);
        }
    }
}
