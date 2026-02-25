using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class Merchant : MonoBehaviour
{
    [SerializeField] private List<GameObject> merchantCards = new List<GameObject>();
    [SerializeField] private GameObject card;
    [SerializeField] private Transform contentPanel;
    [SerializeField] private int numberOfMerchantCards = 5;
    
    private void Start()
    {
        //ShowItems();
    }

    public void ShowItems()
    {
        for (int i = 0; i < numberOfMerchantCards; i++)
        {
            GameObject merchCard = Instantiate(card);
            merchCard.transform.SetParent(contentPanel);
            merchantCards.Add(merchCard);
        }
    }
}
