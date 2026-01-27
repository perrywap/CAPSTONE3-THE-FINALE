using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public enum EnemyType
{

}

public class EnemyBase : MonoBehaviour
{
    [Header("Enemy Stats")]
    [SerializeField] private float movespeed;
    [SerializeField] private int rewardAmount;
    [SerializeField] private bool isDead;

    public float MoveSpeed { get { return movespeed; } }
    public bool IsDead {  get { return isDead; } }

    public void Die()
    {
        GameManager.Instance.enemies.Remove(gameObject);
        Destroy(gameObject);
    }
}
