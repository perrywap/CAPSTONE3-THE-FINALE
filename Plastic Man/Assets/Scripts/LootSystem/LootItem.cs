using UnityEngine;

public class LootItem : MonoBehaviour
{
    public enum PlasticType { Polyethylene, Acrylic, Polycarbonate }

    [Header("Item Info")]
    public PlasticType type;
    [SerializeField] private int _value = 1;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player touched loot. Checking Inventory...");

            if (LootInventory.Instance != null)
            {
                LootInventory.Instance.AddPlastic(type, _value);
                Debug.Log("Success: Inventory found and updated.");
            }
            else
            {
              
                Debug.LogError("FAIL: Inventory.Instance is NULL!");
            }

            Destroy(gameObject);
        }
    }
}