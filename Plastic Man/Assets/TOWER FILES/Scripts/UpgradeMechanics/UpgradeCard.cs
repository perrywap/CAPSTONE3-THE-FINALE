using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CardType
{
    REWARD,
    CHEST,
    MERCHANT
}

public class UpgradeCard : MonoBehaviour
{
    public int index;
    public Image cardImage;

    public bool isClickable;
    public bool isPicked;

    public CardType cardType;

    public int price;

    private void Start()
    {
        if (cardType == CardType.CHEST)
            return;

        index = Random.Range(0, PersistentData.Instance.unitDatas.Count);
        cardImage.sprite = PersistentData.Instance.unitDatas[index].unitSprite;
    }

    public virtual void Activate(int dataIndex)
    {
        
    }

    public void BuyCard()
    {
        if (PersistentData.Instance.gold >= price)
        {
            Debug.Log("Bought Item");
            PersistentData.Instance.gold -= price;
            isClickable = false;
            Activate(index);
            this.gameObject.GetComponentInParent<MerchantCard>().OnCardClicked();
        }
        else
        {
            Debug.Log("NOT ENOUGH GOLD");
        }
    }

    public virtual void OnCardClicked()
    {
        if (!isClickable)
            return;

        if(cardType == CardType.REWARD)
        {
            if (isPicked)
                return;

            isPicked = true;

            RewardsPanel.Instance.RewardPicked();
            HudManager.Instance.FadeOut();
            Activate(index);
        }
        

        if (cardType == CardType.MERCHANT)
        {
            BuyCard();
        }
    }
}