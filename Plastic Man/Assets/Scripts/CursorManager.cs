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
    private GraphicRaycaster raycaster;
    private PointerEventData pointerData;

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

        pointerData = new PointerEventData(EventSystem.current);
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

        if (raycaster == null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
                raycaster = canvas.GetComponent<GraphicRaycaster>();
        }
    }

    private void DetectTargets()
    {
        isOverUI = false;
        isOverWorld = false;
        isPressed = Mouse.current != null && Mouse.current.leftButton.isPressed;

        if (EventSystem.current != null && raycaster != null && Mouse.current != null)
        {
            pointerData.position = Mouse.current.position.ReadValue();

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);

            if (results.Count > 0)
                isOverUI = true;
        }

        if (!isOverUI && cam != null && Mouse.current != null)
        {
            Vector2 mousePos = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null)
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
        if (texture == null) return;
        Cursor.SetCursor(texture, hotspot, CursorMode.Auto);
    }
}