using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    [Header("Cursor Textures")]
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D crosshairCursor;
    [SerializeField] private Texture2D pressedCursor;

    [Header("Hotspots")]
    [SerializeField] private Vector2 defaultHotspot;
    [SerializeField] private Vector2 crosshairHotspot;
    [SerializeField] private Vector2 pressedHotspot;

    private Camera cam;
    private PointerEventData pointerData;
    private GraphicRaycaster[] raycasters;

    private bool isOverUI;
    private bool isOverWorld;
    private bool isPressed;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (EventSystem.current != null)
            pointerData = new PointerEventData(EventSystem.current);

        if (crosshairCursor != null)
            crosshairHotspot = new Vector2(crosshairCursor.width / 2f, crosshairCursor.height / 2f);

        SetCursor(defaultCursor, defaultHotspot);
    }

    private void Update()
    {
        RefreshReferences();
        DetectTargets();
        ApplyCursor();
    }

    private void RefreshReferences()
    {
        if (cam == null)
            cam = Camera.main ?? FindFirstObjectByType<Camera>();

        if (EventSystem.current != null && pointerData == null)
            pointerData = new PointerEventData(EventSystem.current);

        raycasters = FindObjectsByType<GraphicRaycaster>(FindObjectsSortMode.None);
    }

    private void DetectTargets()
    {
        isOverUI = false;
        isOverWorld = false;
        isPressed = Mouse.current != null && Mouse.current.leftButton.isPressed;

        if (Mouse.current == null || pointerData == null)
            return;

        if (PickUps.IsDraggingAnyPickup)
        {
            isOverUI = true;
            return;
        }

        pointerData.position = Mouse.current.position.ReadValue();

        List<RaycastResult> uiResults = new List<RaycastResult>();

        for (int i = 0; i < raycasters.Length; i++)
        {
            if (raycasters[i] == null || !raycasters[i].isActiveAndEnabled)
                continue;

            List<RaycastResult> tempResults = new List<RaycastResult>();
            raycasters[i].Raycast(pointerData, tempResults);

            if (tempResults.Count > 0)
                uiResults.AddRange(tempResults);
        }

        if (uiResults.Count > 0)
        {
            isOverUI = true;
            return;
        }

        if (cam == null)
            return;

        Vector2 mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        RaycastHit2D[] hits = Physics2D.RaycastAll(mousePos, Vector2.zero);

        bool foundPickup = false;
        bool foundOtherWorld = false;

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider == null)
                continue;

            if (hits[i].collider.GetComponentInParent<PickUps>() != null)
            {
                foundPickup = true;
            }
            else
            {
                foundOtherWorld = true;
            }
        }

        if (foundPickup)
        {
            isOverUI = true;
            return;
        }

        if (foundOtherWorld)
        {
            isOverWorld = true;
        }
    }

    private void ApplyCursor()
    {
        if (isOverUI)
        {
            if (isPressed)
                SetCursor(pressedCursor, pressedHotspot);
            else
                SetCursor(defaultCursor, defaultHotspot);

            return;
        }

        if (isOverWorld)
        {
            SetCursor(crosshairCursor, crosshairHotspot);
            return;
        }

        SetCursor(defaultCursor, defaultHotspot);
    }

    private void SetCursor(Texture2D texture, Vector2 hotspot)
    {
        if (texture == null)
            return;

        Cursor.SetCursor(texture, hotspot, CursorMode.Auto);
    }
}