using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D defaultCursor;
    [SerializeField] private Texture2D clickedCursor;
    [SerializeField] private Vector2 cursorHotspot = Vector2.zero; // Adjust as needed

    void Start()
    {
        // Set the initial cursor
        Cursor.SetCursor(defaultCursor, cursorHotspot, CursorMode.Auto);
    }

    void Update()
    {
        // Check for left mouse button press
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.SetCursor(clickedCursor, cursorHotspot, CursorMode.Auto);
        }
        // Check for left mouse button release
        else if (Input.GetMouseButtonUp(0))
        {
            Cursor.SetCursor(defaultCursor, cursorHotspot, CursorMode.Auto);
        }
    }
}
