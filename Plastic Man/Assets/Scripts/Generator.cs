using UnityEngine;

public class Generator : MonoBehaviour
{
    [SerializeField] private float health;

    public void TakeDamage(float damage)
    {
        health -= damage;

        if(health <= 0 )
            DestroyGenerator();
    }

    private void DestroyGenerator()
    {
        Destroy(gameObject);
    }


}
