using UnityEngine;

public class CityCore : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        EnemyBase enemy = col.GetComponent<EnemyBase>();

        if (enemy != null)
        {
            GameManager.Instance.TakeDamage(enemy.CoreDamage);
        }
    }
}


