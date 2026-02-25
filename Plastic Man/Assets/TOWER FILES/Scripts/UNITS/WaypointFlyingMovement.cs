using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaypointFlyingMovement : WaypointMovement
{
    public override void Move()
    {
        if (this.gameObject.GetComponent<Unit>().State == UnitState.ATTACKING)
            return;

        if (this.gameObject.GetComponent<Unit>().State == UnitState.DEAD)
            return;

        transform.position = Vector2.MoveTowards(transform.position, Points[Points.Length - 1].transform.position, Flyer.Speed * Time.deltaTime);

        if(Vector2.Distance(this.transform.position, Points[Points.Length-1].position) <= .5f)
        {
            Flyer.OnPathComplete();
        }
    }
}
