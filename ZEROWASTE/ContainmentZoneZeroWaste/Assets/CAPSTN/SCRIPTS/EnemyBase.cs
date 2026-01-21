using UnityEngine;

public enum EnemyType
{
    Normal,
    Fast,
    Tank
}

public class EnemyBase : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private EnemyType enemyType;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int rewardAmount = 10;

    [Header("Attack")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackRate = 1f;
    [SerializeField] private float attackRange = 0.75f;

    private float attackCooldown;
    private bool isDead;

    private Transform target;
    private HpBarComponent targetHp;

    public float MoveSpeed => moveSpeed;
    public bool IsDead => isDead;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Base")?.transform;

        if (target != null)
            targetHp = target.GetComponentInChildren<HpBarComponent>();
    }

    void Update()
    {
        if (isDead || target == null || targetHp == null) return;

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance > attackRange)
        {
            Move();
        }
        else
        {
            Attack();
        }
    }

    void Move()
    {
        Vector2 direction = (target.position - transform.position).normalized;
        transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);
    }

    void Attack()
    {
        attackCooldown -= Time.deltaTime;

        if (attackCooldown <= 0f)
        {
            targetHp.TakeDamage(attackDamage);
            attackCooldown = 1f / attackRate;
        }
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        GameManager.Instance._enemies.Remove(gameObject);
        //GameManager.Instance.AddCurrency(rewardAmount);

        Destroy(gameObject);
    }
}
