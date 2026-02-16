using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TargetLaser : MonoBehaviour
{
    [SerializeField] private GameObject solarRay;
    [SerializeField] private Animator animator;

    private bool isLocked;

    void Update()
    {
        FollowMouse();

        if(Input.GetMouseButtonDown(0))
        {
            isLocked = true;
            animator.SetBool("isLockedIn", true);
        }
    }

    private void FollowMouse()
    {
        if (isLocked)
            return;

        if (Mouse.current == null) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(
            Mouse.current.position.ReadValue()
        );

        mouseWorldPos.z = 0f;
        transform.position = mouseWorldPos;
    }

    public void Fire()
    {
        GameObject solarGO = Instantiate(solarRay, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
