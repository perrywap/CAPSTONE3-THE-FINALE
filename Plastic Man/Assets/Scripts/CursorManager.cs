//using UnityEngine;
//using UnityEngine.EventSystems;

//public class CursorManager : MonoBehaviour
//{
//    [SerializeField] private Texture2D cursorTexture;

//    private Vector2 cursorHotspot;

//    private void Start()
//    {
//        cursorHotspot = new Vector2(cursorTexture.width /  2, cursorTexture.height / 2);
//        Cursor.SetCursor(cursorTexture, cursorHotspot, CursorMode.Auto);
//    }
//}

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

public class CursorManager : MonoBehaviour
{
    [Header("Cursor Textures")]
    [SerializeField] private Texture2D crosshairCursor;
    [SerializeField] private Texture2D defaultCursor;

    [Header("Cursor Settings")]
    [SerializeField] private Vector2 crosshairHotspot = new Vector2(16, 16);
    [SerializeField] private Vector2 defaultHotspot = Vector2.zero;

    [Header("UI Raycast")]
    [SerializeField] private Canvas canvas; // Assign your main Canvas here
    private GraphicRaycaster raycaster;
    private PointerEventData pointerData;

    private void Awake()
    {
        if (canvas == null)
            canvas = FindObjectOfType<Canvas>();

        raycaster = canvas.GetComponent<GraphicRaycaster>();
        pointerData = new PointerEventData(EventSystem.current);
    }

    private void Start()
    {
        SetCrosshairCursor();
    }

    private void Update()
    {
        HandleCursor();
        //DebugPointerHit();
    }

    private void HandleCursor()
    {
        bool overUI = false;

        if (Pointer.current != null)
        {
            pointerData.position = Pointer.current.position.ReadValue();

            // Raycast UI
            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(pointerData, results);

            if (results.Count > 0)
                overUI = true;
        }

        if (overUI)
            SetDefaultCursor();
        else
            SetCrosshairCursor();
    }

    private void SetCrosshairCursor()
    {
        Cursor.SetCursor(crosshairCursor, crosshairHotspot, CursorMode.Auto);
    }

    private void SetDefaultCursor()
    {
        Cursor.SetCursor(defaultCursor, defaultHotspot, CursorMode.Auto);
    }

    private void DebugPointerHit()
    {
        Vector2 mousePos = Pointer.current.position.ReadValue();

        // First check UI
        pointerData.position = mousePos;
        List<RaycastResult> uiResults = new List<RaycastResult>();
        raycaster.Raycast(pointerData, uiResults);

        if (uiResults.Count > 0)
        {
            Debug.Log("Pointer over UI: " + uiResults[0].gameObject.name);
            return;
        }

        // If not UI, check 2D world
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
        Collider2D hit = Physics2D.OverlapPoint(worldPos);
        if (hit != null)
        {
            Debug.Log("Pointer over World Object: " + hit.gameObject.name);
        }
        else
        {
            Debug.Log("Pointer over nothing");
        }
    }
}