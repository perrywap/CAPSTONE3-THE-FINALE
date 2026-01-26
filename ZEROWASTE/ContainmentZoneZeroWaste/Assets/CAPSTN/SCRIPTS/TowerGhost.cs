using UnityEngine;
using UnityEngine.InputSystem;

public class TowerGhost : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TowerData towerData;
    [SerializeField] private GameObject attackRangePreview;

    [Header("Attributes")]
    [SerializeField] private Color defaultCol;
    [SerializeField] private Color occupiedCol;
    [SerializeField] private Color unoccupiedCol;

    private SpriteRenderer ghostSprite;

    public TowerData GhostData { get { return towerData; } set { towerData = value; } }

    private void Start()
    {
        ghostSprite = GetComponent<SpriteRenderer>();
        ghostSprite.sprite = towerData.towerGhost;
    }

    private void Update()
    {
        FollowMouse();
        CheckForNodes();

        if(Mouse.current.rightButton.wasPressedThisFrame)
            OnCancelBuildClicked();
    }

    private void OnCancelBuildClicked()
    {
        PlayerController.Instance.isBuilding = false;
        Destroy(gameObject);
    }

    private void CheckForNodes()
    {
        TowerNode hoveredNode = GetHoveredNode();

        if (hoveredNode == null)
        {
            ghostSprite.color = defaultCol;
            attackRangePreview.GetComponent<SpriteRenderer>().color = defaultCol;
            return;
        }
        else
        {
            ghostSprite.transform.position = hoveredNode.transform.position;
        }

        if (hoveredNode.isOccupied)
        {
            ghostSprite.color = occupiedCol;
            attackRangePreview.GetComponent<SpriteRenderer>().color = occupiedCol;
        }
        else
        {
            ghostSprite.color = unoccupiedCol;
            attackRangePreview.GetComponent<SpriteRenderer>().color = unoccupiedCol;

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                hoveredNode.isOccupied = true;
                OnBuildTower();
            }
        }
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
        LayerMask nodeLayer = LayerMask.GetMask("Node");

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

    private void OnBuildTower()
    {
        Instantiate(towerData.towerPrefab, transform.position,  Quaternion.identity);

        OnCancelBuildClicked();
    }
}
