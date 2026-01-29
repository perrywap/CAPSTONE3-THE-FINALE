using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class IllegalDumper : MonoBehaviour, IPointerDownHandler
{
    [Header("References")]
    [SerializeField] private GameObject scrapPrefab;
    [SerializeField] private GameObject[] spawnableEnemies;

    [Header("Attributes")]
    [Range(1, 10)]          [SerializeField] private int clicksToRemove = 3;
    [Range(0.5f, 2.5f)]     [SerializeField] private float throwInterval;

    [Header("Throw Chances")]
    [Range(0, 1f)] public float scrapChance = 0.6f;
    [Range(0, 1f)] public float enemyChance = 0.4f;

    [Header("River Damage")]
    [SerializeField] private float riverDamage = 0.5f;
    [SerializeField] private float damageRate = 0.5f;

    private int currentClicks;

    private void Start()
    {
        currentClicks = clicksToRemove;
        InvokeRepeating(nameof(ThrowGarbage), 1f, throwInterval);
        StartCoroutine(DamageRiver());
    }

    void ThrowGarbage()
    {
        Debug.Log("Dumper throw garbage");
        float roll = Random.value;

        if (roll <= scrapChance)
        {
            if (scrapPrefab == null)
                return;

            Instantiate(scrapPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            if (spawnableEnemies.Length == 0)
                return;

            Instantiate(spawnableEnemies[Random.Range(0, spawnableEnemies.Length - 1)], transform.position, Quaternion.identity);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        currentClicks--;

        if (currentClicks <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    private IEnumerator DamageRiver()
    {
        while(true)
        {
            yield return new WaitForSeconds(damageRate);
            RiverManager.Instance.DamageRiver(riverDamage);
        }
    }
}
