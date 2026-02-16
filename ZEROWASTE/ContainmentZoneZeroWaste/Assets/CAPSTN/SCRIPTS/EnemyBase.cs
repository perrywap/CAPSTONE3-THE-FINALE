using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private int rewardAmount = 10;
    [SerializeField] private int coreDamage = 10;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float riverDamagePerSecond = 0.1f;

    private bool isDead;

    public float MoveSpeed => moveSpeed;
    public bool IsDead => isDead;
    public float RiverDamagePerSecond => riverDamagePerSecond;

    public int CoreDamage => coreDamage;

    void Start()
    {

    }

    void Update()
    {
 
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        GameManager.Instance.enemies.Remove(gameObject);
        Destroy(gameObject);
    }
}
