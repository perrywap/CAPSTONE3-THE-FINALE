using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowField : MonoBehaviour
{
    [Range(0f, 1f)]
    [SerializeField] private float slowAmount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Unit unit = collision.gameObject.GetComponent<Unit>();

        if (unit != null)
        {
            float actualSlow = Mathf.Min(slowAmount, unit.Speed);
            unit.Speed -= actualSlow;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Unit unit = collision.gameObject.GetComponent<Unit>();

        if (unit != null)
        {
            float actualSlow = Mathf.Min(slowAmount, unit.Speed);
            unit.Speed += actualSlow;
        }
    }
}
