using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fader : MonoBehaviour
{
    public void ReturnToBigMap()
    {
        //GameSceneManager.Instance.ReturnToBigMap();
        ProgressionManager.Instance.WinLevel();
    }
}
