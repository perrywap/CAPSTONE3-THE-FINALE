using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeMerchCard : MerchantCard
{
    public GameObject card;
    public override void InitializeCard()
    {
        card = PersistentData.Instance.upgradeCards[Random.Range(0, PersistentData.Instance.upgradeCards.Count)];
        UpgradeCard upgrade = card.GetComponent<UpgradeCard>();
        upgrade.isClickable = true;
        upgrade.price = 50;
        upgrade.cardType = CardType.MERCHANT;

        GameObject cardGO = Instantiate(card, content.transform);
        cardGO.transform.SetParent(content);
    }    
}
