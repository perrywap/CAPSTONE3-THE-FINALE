using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PortalCore : MonoBehaviour
{
    // THIS PORTAL CORE WILL ONLY NEED
    // ONE (1) UNIT TO PASS THRU TO
    // WIN THE CURRENT LEVEL

    public TextMeshProUGUI portalTxt;
    public int unitsEntered = 0;

    public bool isGameEnd;

    public void GameEnd()
    {
        isGameEnd = true;   
        GameManager.Instance.winPanel.SetActive(true);
        GameManager.Instance.isGameFinished = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isGameEnd)
            return;

        Unit unit = collision.GetComponent<Unit>();

        if (unit != null)
        {
            unitsEntered++;
            
            if(unitsEntered > 1)
                unitsEntered = 1;

            portalTxt.text = $"{unitsEntered}/1";
            GameEnd();
        }
    }
}
