using UnityEngine;

public class TowerGhost : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TowerData towerData;
    [SerializeField] private GameObject attackRangePreview;

    [Header("Attributes")]
    [SerializeField] private Color occupiedCol;
    [SerializeField] private Color unoccupiedCol;

    private SpriteRenderer ghostSprite;
    private GameObject towerToSpawn;

    private void Start()
    {
        ghostSprite = GetComponent<SpriteRenderer>();
        ghostSprite.sprite = towerData.towerGhost;
    }

    private void Update()
    {
        
    }


}
