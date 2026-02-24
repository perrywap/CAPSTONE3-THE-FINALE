using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] protected float fireRate = 0.2f;
    
    protected Animator animator;

    protected float lastFireTime;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogError("Animator component missing on Sword GameObject!");
    }

    public void TryFire(Vector3 mousePosition)
    {
        if (Time.time < lastFireTime + fireRate)
            return;

        lastFireTime = Time.time;

        Vector3 direction = (mousePosition - transform.position).normalized;
        Fire(transform.position, direction);
    }

    protected abstract void Fire(Vector3 origin, Vector3 direction);
}