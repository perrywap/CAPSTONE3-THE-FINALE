using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SlimeCombat : UnitCombat
{
    [SerializeField] private List<Transform> targets = new List<Transform>();

    public void Explode()
    {
        foreach (Transform t in targets.ToList())
        {
            t.GetComponent<Tower>().TakeDamage(this.gameObject.GetComponent<Unit>().Damage);                
        }

        GameManager.Instance.unitsOnField.Remove(this.gameObject);
        Destroy(gameObject);
    }

    public override void OnAttackRangeEnter(Tower col)
    {
        targets.Add(col.transform);    
    }

    public override void OnAttackRangeExit(Tower col)
    {
        targets.Remove(col.transform);
    }
}
