using UnityEngine;

public class IllegalDumper : MonoBehaviour
{
    [Header("Dumper Stats")]
    [SerializeField] private int clicksToRemove = 5;
    [SerializeField] private float throwInterval = 3f;

    [Header("Prefabs")]
    [SerializeField] private GameObject scrapPrefab;
    [SerializeField] private GameObject enemyPrefab;

    [Header("Throw Chances")]
    [Range(0, 1f)] public float scrapChance = 0.6f;
    [Range(0, 1f)] public float enemyChance = 0.4f;

    [Header("River Damage")]
    [SerializeField] private int waterDamage = 5;

    private int currentClicks;

    private void Start()
    {
        currentClicks = clicksToRemove;
        InvokeRepeating(nameof(ThrowGarbage), 1f, throwInterval);
    }

    void ThrowGarbage()
    {
        float roll = Random.value;

        if (roll <= scrapChance)
        {
            Instantiate(scrapPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        }

        RiverManager.Instance.DamageRiver(waterDamage);
    }

    private void OnMouseDown()
    {
        currentClicks--;

        if (currentClicks <= 0)
        {
            Destroy(gameObject);
        }
    }
}
