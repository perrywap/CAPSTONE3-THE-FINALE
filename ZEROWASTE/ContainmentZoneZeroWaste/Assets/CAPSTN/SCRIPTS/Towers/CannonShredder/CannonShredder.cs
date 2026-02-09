using UnityEngine;

public class CannonShredder : TowerBase
{
    [Header("Laser Settings")]
    [Tooltip("How wide the laser is. 1.5f usually fits ~2-3 grouped enemies.")]
    [SerializeField] private float laserWidth = 1.5f; 

    private void Reset()
    {
        // --- CANNON SHREDDER STATS ---
        
        // Slow Attack Speed (0.5 shots per second = 1 shot every 2 seconds)
        bps = 0.5f;           
        
        // High Damage (Laser melts things)
        damage = 40f;        
        
        // Long Range (It shoots a beam across the map area)
        attackRange = 8f;   
        
        // Slower rotation for that "Heavy Weapon" feel
        rotationSpeed = 80f; 
    }

    public override void Shoot()
    {
        if (target == null) return;

        // Instantiate the Laser Prefab (assigned in Inspector as Bullet Prefab)
        GameObject laserObj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
        
        LaserBeam laser = laserObj.GetComponent<LaserBeam>();

        if (laser != null)
        {
            // Calculate direction towards target
            Vector2 direction = (target.position - firingPoint.position).normalized;

            // Fire the laser!
            // It goes to the full attackRange, piercing/shredding through everything in the line.
            laser.Fire(firingPoint.position, direction, attackRange, laserWidth, damage, enemyMask);
        }
        else
        {
            Debug.LogWarning("CannonShredder: Assigned Bullet Prefab does not have a 'LaserBeam' script!");
        }
    }
}