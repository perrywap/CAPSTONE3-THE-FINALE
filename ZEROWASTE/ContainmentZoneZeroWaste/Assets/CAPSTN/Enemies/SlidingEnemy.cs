using UnityEngine;

public class SlidingEnemy : MonoBehaviour

{
    private EnemyBase enemy;
    private HpBarComponent targetHp;
    private float attackCooldown;

    void Awake()
    {
        enemy = GetComponent<EnemyBase>();
    }

    void Update()
    {
        if (enemy.IsDead || targetHp == null)
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
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Turret"))
        {
            targetHp = null;
        }
    }
}
