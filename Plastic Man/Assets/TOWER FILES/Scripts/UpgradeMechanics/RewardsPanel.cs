using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardsPanel : MonoBehaviour
{
    public static RewardsPanel Instance { get; private set; }

    [Header("GOLD REWARDS")]
    [SerializeField] private int goldRewardAmount;
    [SerializeField] private Text goldTxt;

    [Header("REWARD CARDS")]
    [SerializeField] private GameObject rewardsPanelGO;
    [SerializeField] private List<UpgradeCard> upgradeCards = new List<UpgradeCard>();
    [SerializeField] private int numberOfRewards = 3;


    private List<GameObject> cards = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        goldTxt.text = $"+{goldRewardAmount.ToString()}";

        rewardsPanelGO.SetActive(false);
        PersistentData.Instance.gold += goldRewardAmount;        
    }

    public void ShowRewards()
    {
        rewardsPanelGO.SetActive(true);

        cards = PersistentData.Instance.upgradeCards;

        for (int i = 0; i < numberOfRewards; i++)
        {
            GameObject cardGO = cards[Random.Range(0, cards.Count)];
            UpgradeCard upgrade = cardGO.GetComponent<UpgradeCard>();
            upgrade.isClickable = true;
            upgrade.cardType = CardType.REWARD;

            GameObject card = Instantiate(cardGO);
            card.transform.SetParent(rewardsPanelGO.transform);
        }

        upgradeCards.AddRange(GetComponentsInChildren<UpgradeCard>());
    }

    public void RewardPicked()
    {
        for (int i = 0; i < upgradeCards.Count; i++)
        {
            upgradeCards[i].gameObject.SetActive(upgradeCards[i].isPicked ? true : false);
        }
    }
}
