using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimManager : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Unit unit;
    [SerializeField] private UnitState state;
    [SerializeField] private bool isWalking, isSeeking, isAttacking, isDead;

    private void Start()
    {
        animator = GetComponent<Animator>();
        unit = GetComponent<Unit>();
    }

    private void Update()
    {
        state = unit.State;

        isWalking = (state == UnitState.WALKING) ? true : false;
        isSeeking = (state == UnitState.SEEKING) ? true : false;
        isAttacking = (state == UnitState.ATTACKING) ? true : false;
        isDead = (state == UnitState.DEAD) ? true : false;

        animator.SetBool("isWalking", isWalking);
        animator.SetBool("isAttacking", isAttacking);
        
        if(isDead)
            animator.SetTrigger("Die");
    }
}
