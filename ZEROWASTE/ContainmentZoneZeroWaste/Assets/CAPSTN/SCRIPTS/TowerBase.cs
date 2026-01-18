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
    [SerializeField] private GameObject attackRangeGO;
    [SerializeField] private Transform turretRotationPoint;
    [SerializeField] private LayerMask enemyMask;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firingPoint;

    [Header("Attributes")]
    [SerializeField] private float attackRange = 5f;
    [SerializeField] private float rotationSpeed = 250f;
    [SerializeField] private float bps = 1f; // Bullets Per Second

    private Transform target;
    private float timeUntilFire;

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
            timeUntilFire += Time.deltaTime;

            if(timeUntilFire >= 1f /  bps)
            {
                Shoot();
                timeUntilFire = 0f;
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
        float angle = Mathf.Atan2(target.position.y - transform.position.y, target.position.x - transform.position.x) * Mathf.Rad2Deg - 90f;

        Quaternion targetRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));
        turretRotationPoint.rotation = Quaternion.RotateTowards(turretRotationPoint.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    private bool CheckTargetIsInRange()
    {
        return Vector2.Distance(target.position, transform.position) <= attackRange;
    }

    private void UpdateAttackRangeVisual()
    {
        float diameter = attackRange / 2f;

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
    }
    #endregion

    #region MOUSE EVENTS
    private void OnDrawGizmosSelected()
    {
        Handles.color = Color.cyan;
        Handles.DrawWireDisc(transform.position, transform.forward, attackRange);
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
        Debug.Log("Clickerd");
    }
    #endregion
}
