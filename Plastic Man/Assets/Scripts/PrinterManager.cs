using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PrinterManager : MonoBehaviour
{
    [SerializeField] private Transform printedSlot;
    [SerializeField] private Transform moduleSlot;
    [SerializeField] private Transform filamentSlot;

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
