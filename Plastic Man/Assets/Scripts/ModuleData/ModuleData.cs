using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct FilamentCost
{
    public PlasticType plasticType;
    public int amount;
}

[CreateAssetMenu(fileName = "ModuleData", menuName = "Scriptable Objects/ModuleData")]
public class ModuleData : ScriptableObject
{
    [Header("Module Info")]
    [SerializeField] private GameObject printedPrefab;
    [SerializeField] private Sprite moduleIcon;

    [Header("Filament Cost")]
    [SerializeField] private List<FilamentCost> filamentCosts = new List<FilamentCost>();

    public GameObject PrintedPrefab => printedPrefab;
    public Sprite bpSprite => moduleIcon;
    public List<FilamentCost> FilamentCosts => filamentCosts;
}