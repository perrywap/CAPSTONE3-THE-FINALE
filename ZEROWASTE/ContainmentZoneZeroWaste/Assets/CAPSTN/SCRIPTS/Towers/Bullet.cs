using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected Rigidbody2D rb;

    [Header("Attributes")]
    [SerializeField] protected float bulletSpeed = 5f;
    [SerializeField] protected float bulletLife = 3f;

    protected float damage; 

    protected Transform target;

    private void Start()
    {
        StartCoroutine(BulletLifeSpan());
    }

    public void SetTarget(Transform _target)
    {
        target = _target;
    }

    public void SetDamage(float _damage)
    {
        damage = _damage;
    }

    protected virtual void FixedUpdate()
    {
        BulletMovement();
    }

    virtual public void BulletMovement()
    {
        if (!target)
            return;

        Vector2 direction = (target.position - transform.position).normalized;

        rb.linearVelocity = direction * bulletSpeed;
    }

    virtual public void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyBase enemy = collision.gameObject.GetComponent<EnemyBase>();

        if (enemy != null)
        {
            HpBarComponent hpBar = collision.gameObject.GetComponentInChildren<HpBarComponent>(true);

            if (hpBar != null)
            {
                hpBar.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }

     public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private IEnumerator BulletLifeSpan()
    {
        yield return new WaitForSeconds(bulletLife);
        Destroy(gameObject);
    }
}