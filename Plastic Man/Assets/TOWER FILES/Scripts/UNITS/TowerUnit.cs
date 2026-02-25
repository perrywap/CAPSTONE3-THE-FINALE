using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerUnit : MonoBehaviour
{
    #region VARIABLES

    [Header("Tower Stats")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    [SerializeField] private float attackDamage = 20f;
    [SerializeField] private float fireRate = 1f;
    [SerializeField] private float attackRange = 3f;

    [SerializeField] private bool canAttack = true;
    //[SerializeField] private bool isAttacking = false;

    [Header("Combat")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform currentTarget;
    [SerializeField] private List<GameObject> targets = new List<GameObject>();

    private float fireCooldown;

    #endregion

    private void Start()
    {
        currentHealth = maxHealth;
        fireCooldown = fireRate;
    }

    private void Update()
    {
        if (targets.Count > 0 && canAttack)
        {
            currentTarget = targets[0].transform;

            fireCooldown -= Time.deltaTime;
            if (fireCooldown <= 0f)
            {
                Attack();
                fireCooldown = fireRate;
            }
        }
    }

    public virtual void Attack()
    {
        if (currentTarget == null) return;

        Unit unit = currentTarget.GetComponent<Unit>();
        if (unit != null)
        {
            unit.TakeDamage(attackDamage); 
        }

    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Unit")) 
        {
            targets.Add(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Unit"))
        {
            targets.Remove(collision.gameObject);
        }
    }

    public float CurrentHealth => currentHealth;
}
