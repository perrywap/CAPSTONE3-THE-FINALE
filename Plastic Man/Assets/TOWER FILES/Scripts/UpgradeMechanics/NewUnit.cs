using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class NewUnit : UpgradeCard
{
    [SerializeField] private bool generateNotOwnedUnitsOnly;

    private GameObject unitToAdd;

    private void Start()
    {
        if (PersistentData.Instance.units == null)
        {
            Debug.LogError("No Units!");
            return;
        }

        if (generateNotOwnedUnitsOnly)
        {
            index = Random.Range(0, PersistentData.Instance.unitsNotOwned.Count);
            cardImage.sprite = PersistentData.Instance.unitsNotOwned[index].GetComponent<SpriteRenderer>().sprite;
            unitToAdd = PersistentData.Instance.unitsNotOwned[index];
        } 
        else
        {
            index = Random.Range(0, PersistentData.Instance.units.Count);
            cardImage.sprite = PersistentData.Instance.units[index].GetComponent<SpriteRenderer>().sprite;
            unitToAdd = PersistentData.Instance.units[index];
        }        
    }

    public override void Activate(int dataIndex)
    {
        UnitData dataToAdd = unitToAdd.GetComponent<Unit>().Data;
        PersistentData.Instance.unitsOwned.Add(unitToAdd);

        if (!PersistentData.Instance.unitDatas.Contains(dataToAdd))
            PersistentData.Instance.unitDatas.Add(dataToAdd);
    }

    public override void OnCardClicked()
    {
        if (!isClickable)
            return;

        if (cardType == CardType.REWARD)
        {
            if (isPicked)
                return;

            isPicked = true;
            RewardsPanel.Instance.RewardPicked();
            Activate(index);
            
            HudManager.Instance.FadeOut();
        }

        if (cardType == CardType.MERCHANT)
        {
            BuyCard();
        }
    }
}