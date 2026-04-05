using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class PickUps : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public static bool IsDraggingAnyPickup { get; private set; }
    public static bool HasCompletedDragTutorial { get; private set; } = false;

    [SerializeField] private GameObject weaponPrefab;

    [Header("Tutorial Settings")]
    [SerializeField] private bool _isTutorialItem = false;
    [SerializeField] private GameObject _tutorialPrompt;
    [SerializeField] private UnityEvent _onTutorialEquipped;

    private Camera cam;
    private Vector3 offset;
    private float zDepth;
    private Vector3 originalPosition;

    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    private bool _isCurrentlyDraggingThis = false;

    public GameObject WeaponPrefab => weaponPrefab;

    private void Awake()
    {
        HasCompletedDragTutorial = false;
        IsDraggingAnyPickup = false;

        cam = Camera.main;
        originalPosition = transform.position;
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();

        if (_isTutorialItem && _tutorialPrompt != null)
        {
            _tutorialPrompt.SetActive(false);
        }
    }

    private void Update()
    {
        if (_isTutorialItem && col != null && !HasCompletedDragTutorial)
        {
            if (!NPCDialogue.HasStartedFirstConversation)
            {
                col.enabled = false;
            }
            else if (!_isCurrentlyDraggingThis)
            {
                col.enabled = true;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (_isTutorialItem && !NPCDialogue.HasStartedFirstConversation) return;

        if (_isTutorialItem && !HasCompletedDragTutorial && _tutorialPrompt != null)
        {
            _tutorialPrompt.SetActive(true);
        }
    }

    public void OnPointerClick(PointerEventData eventData) { }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_isTutorialItem && !NPCDialogue.HasStartedFirstConversation) return;

        if (cam == null) return;

        IsDraggingAnyPickup = true;
        _isCurrentlyDraggingThis = true;

        originalPosition = transform.position;
        zDepth = cam.WorldToScreenPoint(transform.position).z;
        offset = transform.position - GetMouseWorld(eventData);

        if (spriteRenderer != null) spriteRenderer.enabled = false;
        if (col != null) col.enabled = false;

        if (DragIconManager.Instance != null && spriteRenderer != null)
            DragIconManager.Instance.Show(spriteRenderer.sprite);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (cam == null) return;
        transform.position = GetMouseWorld(eventData) + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        IsDraggingAnyPickup = false;
        _isCurrentlyDraggingThis = false;

        if (DragIconManager.Instance != null) DragIconManager.Instance.Hide();

        if (eventData.pointerEnter != null)
        {
            WeaponSlot slot = eventData.pointerEnter.GetComponentInParent<WeaponSlot>();

            if (slot != null)
            {
                if (_isTutorialItem && !HasCompletedDragTutorial)
                {
                    if (_tutorialPrompt != null) _tutorialPrompt.SetActive(false);
                    HasCompletedDragTutorial = true;

                    if (NPCDialogue.IsTalking)
                    {
                        NPCDialogue.TutorialWeaponEquippedDuringDialogue = true;
                        NPCDialogue.PendingDoorEvent = _onTutorialEquipped;
                    }
                    else
                    {
                        _onTutorialEquipped?.Invoke();
                    }
                }

                slot.TrySetWeapon(this);
                return;
            }
        }

        transform.position = originalPosition;
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (col != null) col.enabled = true;
    }

    private void OnDisable()
    {
        IsDraggingAnyPickup = false;
        _isCurrentlyDraggingThis = false;

        if (DragIconManager.Instance != null) DragIconManager.Instance.Hide();

        if (_isTutorialItem && !HasCompletedDragTutorial && _tutorialPrompt != null)
        {
            _tutorialPrompt.SetActive(false);
        }
    }

    public void RestorePickup()
    {
        transform.position = originalPosition;
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        if (col != null) col.enabled = true;
    }

    private Vector3 GetMouseWorld(PointerEventData eventData)
    {
        Vector3 screen = new Vector3(eventData.position.x, eventData.position.y, zDepth);
        return cam.ScreenToWorldPoint(screen);
    }
}