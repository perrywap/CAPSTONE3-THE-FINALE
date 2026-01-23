using System;
using UnityEditor.Rendering.Universal.ShaderGraph;
using UnityEngine;
using UnityEngine.EventSystems;

public class Scraps : MonoBehaviour, IPointerClickHandler
{
    [Header("Attributes")]
    [SerializeField] private ResourceType resourceType;
    [SerializeField] private int amount;

    public void OnPointerClick(PointerEventData eventData)
    {
        ResourceEntry materialEntry = new ResourceEntry();
        materialEntry.resourceType = resourceType;
        materialEntry.resourceCost = amount;

        GameManager.Instance.AddMaterial(materialEntry);
        Destroy(gameObject);
    }
}
