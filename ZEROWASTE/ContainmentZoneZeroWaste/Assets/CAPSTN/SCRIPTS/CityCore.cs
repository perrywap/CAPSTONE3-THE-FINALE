using UnityEngine;

public class CityCore : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D col)
    {
        EnemyBase enemy = col.GetComponent<EnemyBase>();

        if (enemy != null)
        {
            Debug.Log("A garbage reached the city!");
        }
    }
}


