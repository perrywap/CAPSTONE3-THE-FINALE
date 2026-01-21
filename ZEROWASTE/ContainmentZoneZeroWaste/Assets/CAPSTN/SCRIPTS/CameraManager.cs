using UnityEngine;
using UnityEngine.InputSystem;

public class CameraManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera cam;

    [Header("Attributes")]
    [SerializeField] private float offset;

    private Vector3 dragOrigin;

    private void Update()
    {
        PanCamera();
    }

    private void PanCamera()
    {
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            dragOrigin = cam.ScreenToWorldPoint(Mouse.current.position.ReadDefaultValue());
        }

        if(Mouse.current.leftButton.isPressed)
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Mouse.current.position.ReadDefaultValue());
            cam.transform.position += difference;
        }
    }


}
