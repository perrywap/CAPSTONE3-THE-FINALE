using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorchField : MonoBehaviour
{
    [SerializeField] private int damage;
    private Coroutine coroutine;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Unit unit = collision.gameObject.GetComponent<Unit>();

        if (unit != null)
        {
            coroutine = StartCoroutine(ApplyScorchDamage(unit));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Unit unit = collision.gameObject.GetComponent<Unit>();

        if (unit != null)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }
    }

    private IEnumerator ApplyScorchDamage(Unit unit)
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(.5f);
            unit.TakeDamage(damage);
        }
    }
}
