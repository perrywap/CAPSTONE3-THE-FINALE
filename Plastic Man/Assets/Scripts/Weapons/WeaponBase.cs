using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] protected float fireRate = 0.2f;
    [SerializeField] protected float damage = 10f;
    [SerializeField] protected bool isAutomatic = false;

    protected Animator animator;
    protected float lastFireTime;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
            Debug.LogError($"{gameObject.name} is missing an Animator!");
    }

    public void HandleInput(Vector3 mousePosition)
    {
        if (isAutomatic)
        {
            //if (Input.GetMouseButton(0) && !CursorManager.Instance.isUsingDefaultCursor)
            if (Input.GetMouseButton(0))
                TryFire(mousePosition);
        }
        else
        {
            //if (Input.GetMouseButtonDown(0) && !CursorManager.Instance.isUsingDefaultCursor)
            if (Input.GetMouseButtonDown(0))
                TryFire(mousePosition);
        }
    }

    public void TryFire(Vector3 mousePosition)
    {
        if (Time.time < lastFireTime + fireRate)
            return;

        lastFireTime = Time.time;

        if (animator != null)
            animator.SetTrigger("fire");

        Vector3 direction = (mousePosition - transform.position).normalized;
        Fire(transform.position, direction);
    }

    protected abstract void Fire(Vector3 origin, Vector3 direction);
}