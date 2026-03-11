using UnityEngine;
using UnityEngine.UIElements;

public class PlayerAnimation : MonoBehaviour
{
    public static PlayerAnimation Instance { get; private set; }

    [SerializeField] private SpriteRenderer upperBody;
    [SerializeField] private SpriteRenderer lowerBody;

    public Animator topAnim;
    private Animator botAnim;

    public int weapIndex;

    public bool isMoving;
    public bool isShooting;
    public bool isAimingLeft;
    public bool isMovingLeft;

    public Vector2 AimDirection { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        topAnim = upperBody.GetComponent<Animator>();
        botAnim = lowerBody.GetComponent<Animator>();
    }

    void Update()
    {
        HandleLookAnimation();
        HandleBodyAnimation();
    }

    private float HandleAiming()
    {
        Vector3 mousePosition = GetMouseWorldPosition();
        Vector3 aimDirection = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;

        Vector3 localScale = Vector3.one;

        if (angle > 90 || angle < -90)
            localScale.y = -1f;
        else
            localScale.y = 1f;

        return angle;
    }

    private void HandleLookAnimation()
    {
        float angle = HandleAiming();

        isAimingLeft = angle > 90f || angle < -90f;
        

        if (isAimingLeft)
        {
            if (angle > 0)
                angle = 180f - angle;
            else
                angle = -180f - angle;
        }

        upperBody.flipX = isAimingLeft;
        lowerBody.flipX = isAimingLeft;

        float snappedAngle = GetSnappedAngle(angle);

        topAnim.SetFloat("angle", snappedAngle);
        topAnim.SetBool("isShooting", isShooting);
    }

    private float GetSnappedAngle(float angle)
    {
        float[] angles = { -60f, -30f, 0f, 30f, 60f };

        float closest = angles[0];
        float minDiff = Mathf.Abs(angle - closest);

        for (int i = 1; i < angles.Length; i++)
        {
            float diff = Mathf.Abs(angle - angles[i]);
            if (diff < minDiff)
            {
                closest = angles[i];
                minDiff = diff;
            }
        }

        return closest;
    }

    private void HandleBodyAnimation()
    {
        botAnim.SetBool("isMoving", isMoving);
        botAnim.SetBool("isMovingLeft", isMovingLeft);
        botAnim.SetBool("isAimingLeft", isAimingLeft);

        topAnim.SetInteger("equippedWeap", weapIndex);
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

