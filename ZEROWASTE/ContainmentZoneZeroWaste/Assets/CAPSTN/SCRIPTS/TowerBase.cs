using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerBase : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    #region VARIABLES
    [Header("References")]
    [SerializeField] protected GameObject attackRangeGO;
    [SerializeField] protected Transform turretRotationPoint;
    [SerializeField] protected LayerMask enemyMask;
    [SerializeField] protected GameObject bulletPrefab;
    [SerializeField] protected Transform firingPoint;

    [Header("Attributes")]
    [SerializeField] protected float attackRange = 5f;
    [SerializeField] protected float rotationSpeed = 250f;
    [SerializeField] protected float attackSpeed = 1f; 
    [SerializeField] protected float damage = 10f;

    [Header("Sprite Direction")]
    [SerializeField] private SpriteRenderer turretSprite;
    [SerializeField] private Sprite[] directionSprites;

    protected Transform target;
    protected float attackCD;

    public float AttackRange { get { return attackRange; } }

    #endregion

    #region UNITY METHODS
    private void Start()
    {
        UpdateAttackRangeVisual();
    }

    private void Update()
    {
        if (target == null)
        {
            FindTarget();
            return;
        }

        RotateTowardsTarget();
        
        if (!CheckTargetIsInRange())
        {
            target = null;
        }
        else
        {
            attackCD += Time.deltaTime;

            if(attackCD >= 1f /  attackSpeed)
            {
                Shoot();
                attackCD = 0f;
            }
        }
    }
    #endregion

    #region METHODS
    private void FindTarget()
    {
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, attackRange, (Vector2)transform.position, 0f, enemyMask);

        if (hits.Length > 0)
        {
            target = hits[0].transform;
        }
    }

    private void RotateTowardsTarget()
    {
        Vector2 dir = target.position - transform.position;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        angle = (angle + 360f) % 360f;

        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle - 90f);
        turretRotationPoint.rotation = Quaternion.RotateTowards(
            turretRotationPoint.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        UpdateTurretSprite(angle);
    }

    private void UpdateTurretSprite(float angle)
    {
        int index = Mathf.RoundToInt(angle / 45f) % 8;
        turretSprite.sprite = directionSprites[index];
    }

    private bool CheckTargetIsInRange()
    {
        if (target == null) return false; 
        return Vector2.Distance(target.position, transform.position) <= attackRange;
    }

    private void UpdateAttackRangeVisual()
    {
        float diameter = attackRange * 2f; 

        attackRangeGO.transform.localScale = new Vector3(
            diameter,
            diameter,
            1f
        );
    }

    virtual public void Shoot()
    {
        GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
        Bullet bulletScript = bulletObj.GetComponent<Bullet>();
        
        bulletScript.SetTarget(target);
        bulletScript.SetDamage(damage);
    }
    #endregion

    #region MOUSE EVENTS
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;

        Vector3 tiltedNormal = Quaternion.Euler(45f, 0f, 0f) * Vector3.forward;
        Handles.DrawWireDisc(transform.position, tiltedNormal, attackRange);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        attackRangeGO.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        attackRangeGO.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked Tower");
    }
    #endregion
}