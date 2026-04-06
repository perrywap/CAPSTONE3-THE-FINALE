using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrinterManager : MonoBehaviour
{
    [Header("Slots")]
    [SerializeField] private Transform printedSlot;
    [SerializeField] private SpriteRenderer filamentSlot1;
    [SerializeField] private SpriteRenderer filamentSlot2;

    [Header("Filaments")]
    [SerializeField] private Sprite emptySprite;
    [SerializeField] private Sprite acrylicFilament;
    [SerializeField] private Sprite polyEthylFilament; 
    [SerializeField] private Sprite polycarbFilament;


    [Header("Printed Weapon")]
    [SerializeField] private SpriteRenderer bpSprite;
    [SerializeField] private List<FilamentCost> costs = new List<FilamentCost>();
    [SerializeField] private GameObject weapPrefab;


    private Animator animator;
    private GameObject currentPrintedWeapon;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void Print()
    {
        if (weapPrefab == null)
        {
            Debug.LogWarning("No weapon prefab selected to print.");
            return;
        }

        if (costs == null || costs.Count == 0)
        {
            Debug.LogWarning("No filament costs assigned for this module.");
            return;
        }

        if (LootInventory.Instance == null)
        {
            Debug.LogWarning("LootInventory instance not found.");
            return;
        }

        if (!HasEnoughFilament())
        {
            Debug.Log("Not enough filament to print this module.");
            return;
        }


        if (animator != null)
            animator.SetTrigger("print");

    }

    public void PrintModule()
    {
        ResetSlots();
        ConsumeFilament();
        SpawnPrintedWeapon();
    }

    public void OnBlueprintClicked(ModuleData data)
    {
        if (data == null)
            return;

        bpSprite.sprite = data.bpSprite;
        costs = new List<FilamentCost>(data.FilamentCosts);
        weapPrefab = data.PrintedPrefab;

        if (costs[0].plasticType == PlasticType.Acrylic)
            filamentSlot1.sprite = acrylicFilament;
        else if (costs[0].plasticType == PlasticType.Polyethylene)
            filamentSlot1.sprite = polyEthylFilament;
        else if (costs[0].plasticType == PlasticType.Polycarbonate)
            filamentSlot1.sprite = polycarbFilament;

        if (costs.Count == 2)
        {
            if (costs[1].plasticType == PlasticType.Acrylic)
                filamentSlot2.sprite = acrylicFilament;
            else if (costs[1].plasticType == PlasticType.Polyethylene)
                filamentSlot2.sprite = polyEthylFilament;
            else if (costs[1].plasticType == PlasticType.Polycarbonate)
                filamentSlot2.sprite = polycarbFilament;
        }        
    }

    private void ResetSlots()
    {
        bpSprite.sprite = emptySprite;
        filamentSlot1.sprite = emptySprite;
        filamentSlot2.sprite = emptySprite;
    }

    private bool HasEnoughFilament()
    {
        foreach (FilamentCost cost in costs)
        {
            int currentAmount = GetPlasticAmount(cost.plasticType);

            if (currentAmount < cost.amount)
                return false;
        }

        return true;
    }

    private void ConsumeFilament()
    {
        foreach (FilamentCost cost in costs)
        {
            RemovePlastic(cost.plasticType, cost.amount);
        }
    }

    private void SpawnPrintedWeapon()
    {
        if (currentPrintedWeapon != null)
            Destroy(currentPrintedWeapon);

        currentPrintedWeapon = Instantiate(weapPrefab, printedSlot.position, Quaternion.identity, printedSlot);
    }

    private int GetPlasticAmount(PlasticType type)
    {
        switch (type)
        {
            case PlasticType.Polyethylene:
                return LootInventory.Instance.polyethyleneCount;

            case PlasticType.Acrylic:
                return LootInventory.Instance.acrylicCount;

            case PlasticType.Polycarbonate:
                return LootInventory.Instance.polycarbonateCount;
        }

        return 0;
    }

    private void RemovePlastic(PlasticType type, int amount)
    {
        switch (type)
        {
            case PlasticType.Polyethylene:
                LootInventory.Instance.polyethyleneCount -= amount;
                break;

            case PlasticType.Acrylic:
                LootInventory.Instance.acrylicCount -= amount;
                break;

            case PlasticType.Polycarbonate:
                LootInventory.Instance.polycarbonateCount -= amount;
                break;
        }
    }
}
