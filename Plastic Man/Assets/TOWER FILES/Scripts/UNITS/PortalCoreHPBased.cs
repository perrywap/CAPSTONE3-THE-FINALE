using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PortalCoreHPBased : PortalCore
{
    // THE PLAYER MUST REDUCE THIS PORTAL'S
    // HEALTH TO ZERO (0) IN ORDER TO WIN
    // THE CURRENT LEVEL

    [SerializeField] private float hp;
    
    public float Hp { get { return hp; } set { hp = value; } }

    private void TakeDamage(float damage)
    {
        hp -= damage;

        if (hp <= 0)
        {
            hp = 0;
            GameEnd();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isGameEnd)
            return;

        Unit unit = collision.GetComponent<Unit>();

        if (unit != null)
        {
            TakeDamage(unit.Damage);
        }
    }
}
