using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chest : MonoBehaviour
{
    [SerializeField] private GameObject rewardCard;
    [SerializeField] private Transform chestParent;

    private void Start()
    {
        rewardCard = PersistentData.Instance.upgradeCards[Random.Range(0, PersistentData.Instance.upgradeCards.Count)];
        UpgradeCard upgrade = rewardCard.GetComponent<UpgradeCard>();
        upgrade.cardType = CardType.CHEST;
        upgrade.isClickable = false;

        GameObject rewardGO = Instantiate(rewardCard);
        rewardGO.transform.SetParent(chestParent);              

        for (int i = 0; i <  PersistentData.Instance.unitDatas.Count; i++)
        {
            upgrade.Activate(i);
        } 
    }
}