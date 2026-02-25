using System;
using System.Collections;
using UnityEngine;

public class MortarTowerAnimator : MonoBehaviour
{
    [Header("Attack Animation")]
    [SerializeField] private Sprite[] attackFrames;
    [SerializeField] private float attackFrameRate = 0.25f;

    public Action OnFireFrameReached;
    private string currentState = "";

    private SpriteRenderer spriteRenderer;
    private Coroutine animationCoroutine;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void PlayAttack()
    {
        if (currentState == "Attack") return;

        currentState = "Attack";
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        animationCoroutine = StartCoroutine(LoopAttackAnimation());
    }

    private IEnumerator LoopAttackAnimation()
    {
        int index = 0;
        while (true)
        {
            if (attackFrames.Length == 0) yield break;

            spriteRenderer.sprite = attackFrames[index];

            if (index == 0)
                OnFireFrameReached?.Invoke();

            index = (index + 1) % attackFrames.Length;
            yield return new WaitForSeconds(attackFrameRate);
        }
    }

    public void StopAnimation()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
        currentState = "";
    }

}
