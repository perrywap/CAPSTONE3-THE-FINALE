using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;
    [SerializeField] private SpriteRenderer mapRenderer;

    [Header("Zoom Settings")]
    [Range(0.1f, 1f)]
    [SerializeField] private float zoomStep = 0.5f;
    [Range(1f, 2f)]
    [SerializeField] private float minCamSize =2f;
    [Range(4f, 14f)]
    [SerializeField] private float maxCamSize = 6f;

    private float minMapX, minMapY, maxMapX, maxMapY;
    private Vector3 dragOrigin;

    private void Awake()
    {
        Bounds bounds = mapRenderer.bounds;

        minMapX = bounds.min.x;
        maxMapX = bounds.max.x;
        minMapY = bounds.min.y;
        maxMapY = bounds.max.y;
    }

    private void Update()
    {
        PanCamera();
        HandleScrollZoom();
    }

    private void PanCamera2()
    {
        if (Mouse.current == null) return;

        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = cam.transform.position.y;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            dragOrigin = cam.ScreenToWorldPoint(mousePos);
        }

        if (Mouse.current.leftButton.isPressed)
        {
            Vector3 currentPos = cam.ScreenToWorldPoint(mousePos);
            Vector3 difference = dragOrigin - currentPos;

            cam.transform.position = ClampCamera(cam.transform.position + difference);
        }
    }

    private void PanCamera()
    {
        if (Mouse.current == null) return;

        // Ignore clicks on UI
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = cam.transform.position.y;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            dragOrigin = cam.ScreenToWorldPoint(mousePos);
        }

        if (Mouse.current.leftButton.isPressed)
        {
            Vector3 currentPos = cam.ScreenToWorldPoint(mousePos);
            Vector3 difference = dragOrigin - currentPos;

            cam.transform.position = ClampCamera(cam.transform.position + difference);
        }
    }

    private void HandleScrollZoom()
    {
        if (Mouse.current == null) return;

        float scrollValue = Mouse.current.scroll.ReadValue().y;

        if (scrollValue > 0f) ZoomIn();
        else if (scrollValue < 0f) ZoomOut();
    }

    public void ZoomIn()
    {
        cam.orthographicSize = Mathf.Clamp(
            cam.orthographicSize - zoomStep,
            minCamSize,
            maxCamSize
        );

        cam.transform.position = ClampCamera(cam.transform.position);
    }

    public void ZoomOut()
    {
        cam.orthographicSize = Mathf.Clamp(
            cam.orthographicSize + zoomStep,
            minCamSize,
            maxCamSize
        );

        cam.transform.position = ClampCamera(cam.transform.position);
    }

    private Vector3 ClampCamera(Vector3 targetPosition)
    {
        float camHeight = cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        float minX = minMapX + camWidth;
        float maxX = maxMapX - camWidth;
        float minY = minMapY + camHeight;
        float maxY = maxMapY - camHeight;

        float newX = Mathf.Clamp(targetPosition.x, minX, maxX);
        float newY = Mathf.Clamp(targetPosition.y, minY, maxY);

        return new Vector3(newX, newY, targetPosition.z);
    }
}
