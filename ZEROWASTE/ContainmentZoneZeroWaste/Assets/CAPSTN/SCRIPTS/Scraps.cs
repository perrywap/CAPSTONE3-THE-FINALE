using System;
using UnityEditor.Rendering.Universal.ShaderGraph;
using UnityEngine;
using UnityEngine.EventSystems;

public class Scraps : MonoBehaviour
{
    [Header("Attributes")]
    [SerializeField] public ResourceType resourceType;
    [SerializeField] public int amount;

    private void OnMouseDown()
    {
        Pickup();
    }

    private void Pickup()
    {
        Debug.Log("Picked up loot");

        ResourceEntry materialEntry = new ResourceEntry
        {
            resourceType = resourceType,
            resourceCost = amount
        };

        GameManager.Instance.AddMaterial(materialEntry);
        Destroy(gameObject);
    }
}
