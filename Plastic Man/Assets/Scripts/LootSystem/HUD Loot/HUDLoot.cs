using UnityEngine;
using TMPro;

public class HudLoot : MonoBehaviour
{
    [Header("UI Text References")]
    [SerializeField] private TextMeshProUGUI _peText;
    [SerializeField] private TextMeshProUGUI _acText;
    [SerializeField] private TextMeshProUGUI _pcText;

    private void Update()
    {
        if (LootInventory.Instance != null)
        {
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        _peText.text = LootInventory.Instance.polyethyleneCount.ToString();
        _acText.text = LootInventory.Instance.acrylicCount.ToString();
        _pcText.text = LootInventory.Instance.polycarbonateCount.ToString();
    }
}