using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Unit
{
    public override void Die()
    {
        if (!isDead)
            return;

        this.gameObject.GetComponent<Animator>().SetTrigger("explode");
    }
}
