using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCountIncrease : UpgradeCard
{
    [SerializeField] private int amount;

    public override void Activate(int i)
    {
        UnitData unit = PersistentData.Instance.unitDatas[i];

        unit.SpawnCount += amount;
    }

    //public override void OnCardClicked()
    //{
    //    if (isPicked)
    //        return;

    //    base.OnCardClicked();
    //}
}
