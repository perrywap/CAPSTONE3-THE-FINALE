using UnityEngine;

public class Sword : WeaponBase
{
    [SerializeField] private float range = 1.5f;
    //[SerializeField] private int damage = 10;
    [SerializeField] private LayerMask hitLayer;

    protected override void Fire(Vector3 origin, Vector3 direction)
    {
        animator.SetTrigger("swing");
        Vector3 hitPosition = origin + direction.normalized * range / 2f;
        float hitRadius = range / 2f;

        Collider2D[] hits = Physics2D.OverlapCircleAll(hitPosition, hitRadius, hitLayer);

        foreach (Collider2D hit in hits)
        {
            Debug.Log("Slash!");
        }

        Debug.DrawLine(origin, origin + direction.normalized * range, Color.red, 0.5f);
        Debug.DrawRay(hitPosition, Vector3.up * 0.01f, Color.green, 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying) return;
        if (transform == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range / 2f);
    }
}