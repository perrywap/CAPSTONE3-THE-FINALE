using UnityEngine;
using UnityEngine.InputSystem;

public class TowerGhost : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TowerData towerData;
    [SerializeField] private GameObject attackRangePreview;

    [Header("Placement Settings")]
    [SerializeField] private LayerMask nodeLayer; // Set to "Node" layer
    [SerializeField] private LayerMask pathLayer; // NEW: Set to "Water" or "Path" layer

    [Header("Attributes")]
    [SerializeField] private Color defaultCol;
    [SerializeField] private Color occupiedCol;
    [SerializeField] private Color unoccupiedCol;

    private SpriteRenderer ghostSprite;

    public TowerData GhostData { get { return towerData; } set { towerData = value; } }

    private void Start()
    {
        ghostSprite = GetComponent<SpriteRenderer>();
        if (towerData != null)
        {
            ghostSprite.sprite = towerData.towerGhost;
        }
        
        // Hide attack range for traps (Plastic Catcher), show for towers
        if (attackRangePreview != null)
        {
            bool isTower = towerData.placementType == PlacementType.Node;
            attackRangePreview.SetActive(isTower);
        }
    }

    private void Update()
    {
        FollowMouse();

        // Switch logic based on what we are building
        if (towerData.placementType == PlacementType.Node)
        {
            CheckForNodes();
        }
        else if (towerData.placementType == PlacementType.Path)
        {
            CheckForPath();
        }

        if(Mouse.current.rightButton.wasPressedThisFrame)
            OnCancelBuildClicked();
    }

    private void OnCancelBuildClicked()
    {
        PlayerController.Instance.isBuilding = false;
        Destroy(gameObject);
    }

    // --- LOGIC FOR STANDARD TOWERS (NODES) ---
    private void CheckForNodes()
    {
        TowerNode hoveredNode = GetHoveredNode();

        if (hoveredNode == null)
        {
            SetColor(defaultCol);
            return;
        }
        else
        {
            ghostSprite.transform.position = hoveredNode.transform.position;
        }

        if (hoveredNode.isOccupied)
        {
            SetColor(occupiedCol);
        }
        else
        {
            SetColor(unoccupiedCol);

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                hoveredNode.isOccupied = true;
                BuildStructure();
            }
        }
    }

    // --- LOGIC FOR TRAPS (PATH/WATER) ---
    private void CheckForPath()
    {
        // 1. Raycast to find Water/Path
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.zero, 0f, pathLayer);

        // 2. Also check we are NOT hitting a Node (don't overlap towers)
        RaycastHit2D nodeHit = Physics2D.Raycast(transform.position, Vector2.zero, 0f, nodeLayer);

        bool onPath = hit.collider != null;
        bool overlappingNode = nodeHit.collider != null;

        if (onPath && !overlappingNode)
        {
            SetColor(unoccupiedCol);

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                BuildStructure();
            }
        }
        else
        {
            // If we are off-path or hitting a node, show red
            SetColor(occupiedCol);
        }
    }

    private void SetColor(Color col)
    {
        ghostSprite.color = col;
        if (attackRangePreview != null) 
            attackRangePreview.GetComponent<SpriteRenderer>().color = col;
    }

    private void FollowMouse()
    {
        if (Mouse.current == null) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );

        mouseWorldPos.z = 0f;
        transform.position = mouseWorldPos;
    }

    private TowerNode GetHoveredNode()
    {
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position,
            Vector2.zero,
            0f,
            nodeLayer
        );

        if (hit.collider == null)
            return null;

        return hit.collider.GetComponent<TowerNode>();
    }

    private void BuildStructure()
    {
        Instantiate(towerData.towerPrefab, transform.position,  Quaternion.identity);
        OnCancelBuildClicked();
    }
}