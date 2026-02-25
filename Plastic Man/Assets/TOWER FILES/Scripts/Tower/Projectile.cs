using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("General Projectile Stats")]
    [SerializeField] protected float speed = 5f;
    protected float damage;
    protected bool initialized = false;

    public virtual void SetTarget(Unit target, float dmg)
    {
        damage = dmg;
        initialized = true;
    }
}
