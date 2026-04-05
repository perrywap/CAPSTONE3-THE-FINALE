using UnityEngine;

public enum PlasticType { Polyethylene, Acrylic, Polycarbonate }

public class LootItem : MonoBehaviour
{
    [Header("Item Info")]
    public PlasticType type;
    [SerializeField] private int _value = 1;

    [Header("Audio")]
    [SerializeField] private AudioClip pickupSfx;
    [SerializeField] private float pickupVolume = 0.7f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LootInventory inventory = LootInventory.Instance;

            if (inventory == null)
            {
                inventory = Object.FindFirstObjectByType<LootInventory>();
            }

            if (inventory != null)
            {
                inventory.AddPlastic(type, _value);
                PlayPickupSound();
                Destroy(gameObject);
            }
            else
            {
                Debug.LogError("FAIL: No LootInventory found in scene! Check if it's on a Manager object.");
            }
        }
    }

    private void PlayPickupSound()
    {
        if (pickupSfx == null) return;

        if (SfxManager.instance != null)
        {
            SfxManager.instance.PlaySFX(pickupSfx, pickupVolume);
        }
        else
        {
            AudioSource.PlayClipAtPoint(pickupSfx, transform.position, pickupVolume);
        }
    }
}