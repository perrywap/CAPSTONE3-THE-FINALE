using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PortalCoreNumberBased : PortalCore
{
    // A NUMBER OF UNITS MUST PASS THRU
    // IN ORDER TO WIN THE CURRENT LEVEL

    [SerializeField] private int unitsRequired;

    private void Start()
    {
        portalTxt.text = $"{unitsEntered}/{unitsRequired}";
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isGameEnd)
            return;

        Unit unit = collision.GetComponent<Unit>();

        if (unit != null)
        {
            unitsEntered++;

            portalTxt.text = $"{unitsEntered}/{unitsRequired}";

            if (unitsEntered >= unitsRequired)
            {
                unitsEntered = unitsRequired;
                GameEnd();
            }
        }
    }
}
