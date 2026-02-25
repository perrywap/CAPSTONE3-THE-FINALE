using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPanel : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;

    void Start()
    {
        DisplayCards();
    }

    private void DisplayCards()
    {
        if (PersistentData.Instance.unitsOwned == null)
        {
            Debug.LogError("No Units Owned!");
            return;
        }
        for (int i = 0; i < PersistentData.Instance.unitsOwned.Count; i++)
        {
            cardPrefab.GetComponent<Card>().UnitPrefab = PersistentData.Instance.unitsOwned[i];
            GameObject newCard = Instantiate(cardPrefab);
            GameManager.Instance.cardsOnHand.Add(newCard);
            newCard.transform.SetParent(this.gameObject.transform);
        }
    }
}
