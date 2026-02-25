using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomTrigger : MonoBehaviour
{
    public event System.Action<Tower> EnteredTrigger;
    public event System.Action<Tower> ExitTrigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Tower tower = collision.gameObject.GetComponent<Tower>();

        if (tower != null)
            EnteredTrigger?.Invoke(tower);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Tower tower = collision.gameObject.GetComponent<Tower>();

        if (tower != null)
            ExitTrigger?.Invoke(tower);
    }
}