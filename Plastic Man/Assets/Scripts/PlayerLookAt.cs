using UnityEngine;

public class PlayerLookAt : MonoBehaviour
{
    public static PlayerLookAt Instance { get; private set; }
    public float angle;
    public bool isLookingLeft;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        HandleLookDirection();
    }


    public void HandleLookDirection()
    {
        Vector3 mousePosition = GetMouseWorldPosition();
        Vector3 aimDirection = (mousePosition - transform.position).normalized;
        float _angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        isLookingLeft =  _angle > 90f || _angle < -90f;

        angle = _angle < 0 ? _angle + 360f : _angle;
    }

    #region MOUSE WORLD POSITION
    private static Vector3 GetMouseWorldPosition()
    {
        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vec.z = 0f;
        return vec;
    }

    private static Vector3 GetMouseWorldPositionWithZ()
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
    }

    private static Vector3 GetMouseWorldPositionWithZ(Camera worldCamera)
    {
        return GetMouseWorldPositionWithZ(Input.mousePosition, worldCamera);
    }

    private static Vector3 GetMouseWorldPositionWithZ(Vector3 screenPosition, Camera worldCamera)
    {
        Vector3 worldPosition = worldCamera.ScreenToWorldPoint(screenPosition);
        return worldPosition;
    }
    #endregion
}
