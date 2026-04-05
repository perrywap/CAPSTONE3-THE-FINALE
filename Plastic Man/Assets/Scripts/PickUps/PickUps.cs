using UnityEngine;
using UnityEngine.EventSystems;

public class PickUps : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static bool IsDraggingAnyPickup { get; private set; }

    [SerializeField] private GameObject weaponPrefab;

    private Camera cam;
    private Vector3 offset;
    private float zDepth;
    private Vector3 originalPosition;

    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    public GameObject WeaponPrefab => weaponPrefab;

    private void Awake()
    {
        cam = Camera.main;
        originalPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (cam == null)
            return;

        IsDraggingAnyPickup = true;
        originalPosition = transform.position;
        zDepth = cam.WorldToScreenPoint(transform.position).z;
        offset = transform.position - GetMouseWorld(eventData);

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (col != null)
            col.enabled = false;

        if (DragIconManager.Instance != null && spriteRenderer != null)
            DragIconManager.Instance.Show(spriteRenderer.sprite);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (cam == null)
            return;

        transform.position = GetMouseWorld(eventData) + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsDraggingAnyPickup = false;

        if (DragIconManager.Instance != null)
            DragIconManager.Instance.Hide();

        if (eventData.pointerEnter != null)
        {
            WeaponSlot slot = eventData.pointerEnter.GetComponentInParent<WeaponSlot>();

            if (slot != null)
            {
                slot.TrySetWeapon(this);
                return;
            }
        }

        transform.position = originalPosition;

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        if (col != null)
            col.enabled = true;
    }

    private void OnDisable()
    {
        IsDraggingAnyPickup = false;

        if (DragIconManager.Instance != null)
            DragIconManager.Instance.Hide();
    }

    public void RestorePickup()
    {
        transform.position = originalPosition;

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        if (col != null)
            col.enabled = true;
    }

    private Vector3 GetMouseWorld(PointerEventData eventData)
    {
        Vector3 screen = new Vector3(eventData.position.x, eventData.position.y, zDepth);
        return cam.ScreenToWorldPoint(screen);
    }
}