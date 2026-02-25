using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HpBar : MonoBehaviour
{
    [SerializeField] private Image hpBar;
    [SerializeField] private bool showHpBar;
    [SerializeField] private float popUpTime;

    private float currentHealth;
    private float maxHealth;
    private Coroutine coroutine;


    private void Start()
    {
        if(this.GetComponentInParent<Unit>() != null)
        {
            maxHealth = this.GetComponentInParent<Unit>().Hp;
        }

        if(this.GetComponentInParent<Tower>() != null)
        {
            maxHealth = this.GetComponentInParent<Tower>().Hp;
        }

        if(this.GetComponent<PortalCoreHPBased>() != null)
        {
            maxHealth = this.GetComponentInParent<PortalCoreHPBased>().Hp;
        }
    }

    private void Update()
    {
        if(showHpBar)
        {
            if (this.GetComponentInParent<Unit>() != null)
            {
                currentHealth = this.GetComponentInParent<Unit>().Hp;
            }

            if (this.GetComponentInParent<Tower>() != null)
            {
                currentHealth = this.GetComponentInParent<Tower>().Hp;
            }
            if (this.GetComponent<PortalCoreHPBased>() != null)
            {
                currentHealth = this.GetComponentInParent<PortalCoreHPBased>().Hp;
            }

            hpBar.fillAmount = currentHealth / maxHealth;
        }

        hpBar.gameObject.SetActive(showHpBar ? true : false);
    }

    public void PopHpBar()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            coroutine = null;
        }

        coroutine = StartCoroutine(Show());
    }

    private IEnumerator Show()
    {
        showHpBar = true;
        yield return new WaitForSeconds(popUpTime);
        showHpBar = false;
    }
}
