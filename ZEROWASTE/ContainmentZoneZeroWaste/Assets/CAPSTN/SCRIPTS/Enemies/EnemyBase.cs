using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private int rewardAmount = 10;
    [SerializeField] private int coreDamage = 10;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float riverDamagePerSecond = 0.1f;

    [Header("Drop Settings")]
    [SerializeField] private GameObject dropPrefab;
    [SerializeField] private float dropDistance = 0.6f;

    private bool isDead;

    public float MoveSpeed => moveSpeed;
    public bool IsDead => isDead;
    public float RiverDamagePerSecond => riverDamagePerSecond;
    public int CoreDamage => coreDamage;

    public virtual void Die()
    {
        if (isDead) return;
        isDead = true;

        DropReward();

        GameManager.Instance.enemies.Remove(gameObject);
        Destroy(gameObject);
    }

    private void DropReward()
    {
        if (dropPrefab == null) return;

        Vector2 offset = Random.insideUnitCircle.normalized * dropDistance;
        Vector3 spawnPos = transform.position;

        GameObject drop = Instantiate(dropPrefab, spawnPos, Quaternion.identity);

        ScrapLoot loot = drop.GetComponent<ScrapLoot>();
        if (loot != null)
        {
            loot.Init(spawnPos + (Vector3)offset, rewardAmount);
        }
    }
}
