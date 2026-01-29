using UnityEngine;

public class SlidingEnemy : MonoBehaviour
{
    private EnemyBase enemy;
    private HpBarComponent targetHp;
    private WaypointMovement waypointMovement;

    private float attackCooldown;
    private bool isAttacking;

    void Awake()
    {
        enemy = GetComponent<EnemyBase>();
        waypointMovement = GetComponent<WaypointMovement>();
    }

    void Update()
    {
        if (enemy.IsDead || !isAttacking || targetHp == null)
            return;

        attackCooldown -= Time.deltaTime;

        if (attackCooldown <= 0f)
        {
            targetHp.TakeDamage(enemy.attackDamage);
            attackCooldown = 1f / enemy.attackRate;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Turret"))
        {
            targetHp = collision.gameObject.GetComponentInChildren<HpBarComponent>();

            if (targetHp != null)
            {
                isAttacking = true;

                if (waypointMovement != null)
                    waypointMovement.enabled = false;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Turret"))
        {
            targetHp = null;
            isAttacking = false;

            if (waypointMovement != null)
                waypointMovement.enabled = true;
        }
    }
}
