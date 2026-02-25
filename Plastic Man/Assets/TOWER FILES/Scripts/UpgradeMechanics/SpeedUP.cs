using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedUP : UpgradeCard
{
    [SerializeField] private float amount;

    public override void Activate(int i)
    {
        UnitData unit = PersistentData.Instance.unitDatas[i];

        unit.Speed += amount;
    }
}
