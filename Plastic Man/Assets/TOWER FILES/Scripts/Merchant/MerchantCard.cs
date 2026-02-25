using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MerchantCard : MonoBehaviour
{
    public Transform content;
    public int price;
    public Text priceTxt;

    // Start is called before the first frame update
    void Start()
    {
        InitializeCard();
        priceTxt.text = price.ToString();
    }

    public virtual void InitializeCard()
    {

    }

    public virtual void OnCardClicked()
    {
        priceTxt.text = "SOLD";
    }
}
