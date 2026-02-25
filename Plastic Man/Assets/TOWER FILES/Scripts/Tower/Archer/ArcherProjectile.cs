using UnityEngine;

public class ArcherProjectile : Projectile
{
    private Unit target;

    public override void SetTarget(Unit newTarget, float dmg)
    {
        base.SetTarget(newTarget, dmg);
        target = newTarget;

        if (target != null)
        {
            Vector3 dir = target.transform.position - transform.position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle + 180);
        }
    }

    private void Update()
    {
        if (!initialized || target == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.transform.position - transform.position).normalized;
        transform.position += direction * speed * Time.deltaTime;

        float distance = Vector3.Distance(transform.position, target.transform.position);
        if (distance < 0.2f)
        {
            target.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}