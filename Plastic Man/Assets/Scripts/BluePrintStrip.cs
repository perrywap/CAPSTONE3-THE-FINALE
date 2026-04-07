using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BluePrintStrip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private TextMeshProUGUI filCost1;
    [SerializeField] private TextMeshProUGUI filCost2;
    [SerializeField] private Image filIcon1;
    [SerializeField] private Image filIcon2;

    [Header("Blueprint Data")]
    [SerializeField] private ModuleData data;

    [Header("Description Body")]
    [SerializeField] private string descText;

    [Header("References")]
    [SerializeField] private Sprite empty;
    [SerializeField] private Sprite acrylicIcon;
    [SerializeField] private Sprite polyEthylIcon;
    [SerializeField] private Sprite polyCarbIcon;

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowDetails();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        description.text = " ";
        filCost1.text = " ";
        filCost2.text = " ";
        filIcon1.sprite = empty;
        filIcon2.sprite = empty;
    }

    private void ShowDetails()
    {
        description.text = descText;

        if (data == null || data.FilamentCosts == null || data.FilamentCosts.Count == 0)
        {
            filCost1.text = " ";
            filCost2.text = " ";
            filIcon1.sprite = empty;
            filIcon2.sprite = empty;
            return;
        }

        filCost1.text = data.FilamentCosts[0].amount.ToString();
        filIcon1.sprite = GetPlasticSprite(data.FilamentCosts[0].plasticType);

        if (data.FilamentCosts.Count > 1)
        {
            filCost2.text = data.FilamentCosts[1].amount.ToString();
            filIcon2.sprite = GetPlasticSprite(data.FilamentCosts[1].plasticType);
        }
        else
        {
            filCost2.text = " ";
            filIcon2.sprite = empty;
        }
    }

    private Sprite GetPlasticSprite(PlasticType plasticType)
    {
        switch (plasticType)
        {
            case PlasticType.Acrylic:
                return acrylicIcon;

            case PlasticType.Polyethylene:
                return polyEthylIcon;

            case PlasticType.Polycarbonate:
                return polyCarbIcon;

            default:
                return empty;
        }
    }
}