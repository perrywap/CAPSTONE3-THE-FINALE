using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HpBarComponent : MonoBehaviour
{
    [SerializeField] private Image hpBar;
    [SerializeField] private float currentHp;
    [SerializeField] private float maxHp;

    public float CurrentHp { get { return currentHp; } }
    public float MaxtHp { get { return maxHp; } }

    private void Start()
    {
        // HIDE HP BAR
        this.gameObject.SetActive(false);

        currentHp = maxHp;
    }

    private void Update()
    {        
        hpBar.fillAmount = currentHp / maxHp; 
    }

    public void TakeDamage(float amount)
    {
        currentHp -= amount;

        StartCoroutine(ShowHpBar());

        if (currentHp <= 0)
        {
            currentHp = 0;

            transform.parent.gameObject.GetComponent<EnemyBase>().Die();
        }
    }

    private IEnumerator ShowHpBar()
    {
        this.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        this.gameObject.SetActive(false);
    }
}
