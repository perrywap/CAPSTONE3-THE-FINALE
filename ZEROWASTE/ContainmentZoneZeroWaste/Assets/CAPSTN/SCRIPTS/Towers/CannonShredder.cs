using UnityEngine;

public class CannonShredder : TowerBase
{
    public override void Shoot()
    {
        GameObject bulletObj = Instantiate(bulletPrefab, firingPoint.position, Quaternion.identity);
        ShredderBullet bullet = bulletObj.GetComponent<ShredderBullet>();


        Vector2 fireDir = (target.position - firingPoint.position).normalized;
        bullet.Init(fireDir, damage);
    }
}
