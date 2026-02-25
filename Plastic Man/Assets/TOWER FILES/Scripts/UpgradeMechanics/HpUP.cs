using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpUP : UpgradeCard
{
    [SerializeField] private float amount;

    public override void Activate(int i)
    {
        UnitData unit = PersistentData.Instance.unitDatas[i];

        unit.Hp += amount;
    }

    //public override void OnCardClicked()
    //{
    //    if (isPicked)
    //        return;

    //    base.OnCardClicked();
    //}
}
