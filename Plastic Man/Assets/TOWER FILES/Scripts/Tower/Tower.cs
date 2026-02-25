using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("Tower Stats")]
    [SerializeField] private float hp;
    [SerializeField] protected float range = 3f;
    [SerializeField] protected float damage = 1f;
    [SerializeField] protected float fireRate = 1f;

    [Header("Broken Tower")]
    [SerializeField] private GameObject brokenTowerPrefab;
    [SerializeField] private Transform brokenSpawnPoint;

    [Header("VFX")]
    [SerializeField] private GameObject deathVFXPrefab;

    protected float fireCooldown = 0f;
    public List<Unit> targetsInRange = new List<Unit>();

    public Unit currentTarget;

    public float Hp { get { return hp; } }
    public bool IsDying { get; private set; } = false;

    protected virtual void Update()
    {
        fireCooldown -= Time.deltaTime;
        RemoveNullTargets();

        if (currentTarget == null || Vector2.Distance(transform.position, currentTarget.transform.position) > range + 0.1f)
        {
            currentTarget = GetNearestTarget();
        }

        if (currentTarget != null && fireCooldown <= 0f)
        {
            Attack(currentTarget);
            fireCooldown = 1f / fireRate;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        Unit unit = other.GetComponent<Unit>();
        if (unit != null && !targetsInRange.Contains(unit))
        {
            targetsInRange.Add(unit);
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        Unit unit = other.GetComponent<Unit>();
        if (unit != null)
        {
            float dist = Vector2.Distance(transform.position, unit.transform.position);
            if (dist > range + 0.1f)
            {
                targetsInRange.Remove(unit);

                if (unit == currentTarget)
                {
                    currentTarget = null;
                }
            }
        }
    }

    protected void RemoveNullTargets()
    {
        targetsInRange.RemoveAll(u => u == null);
    }

    protected Unit GetNearestTarget()
    {
        Unit closest = null;
        float shortestDist = Mathf.Infinity;

        foreach (Unit u in targetsInRange)
        {
            float dist = Vector2.Distance(transform.position, u.transform.position);
            if (dist < shortestDist)
            {
                shortestDist = dist;
                closest = u;
            }
        }

        return closest;
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;

        if (hp <= 0f)
        {
            hp = 0f;
            Die();
        }
        this.gameObject.GetComponentInChildren<HpBar>().PopHpBar();
    }

    private void Die()
    {
        IsDying = true;
        StartCoroutine(DelayedDestroy());
    }

    private IEnumerator DelayedDestroy()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        yield return new WaitForSeconds(0.5f);

        Vector3 spawnPosition = brokenSpawnPoint != null ? brokenSpawnPoint.position : transform.position;

        if (deathVFXPrefab != null)
        {
            Instantiate(deathVFXPrefab, spawnPosition, Quaternion.identity);
        }

        if (brokenTowerPrefab != null)
        {
            Instantiate(brokenTowerPrefab, spawnPosition, Quaternion.identity);
        }

        Destroy(transform.root.gameObject);
    }

    public virtual void Attack(Unit target)
    {

    }

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
