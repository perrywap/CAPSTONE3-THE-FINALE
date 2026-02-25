using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurnEffect : MonoBehaviour
{
    public void OnAnimationEnd()
    {
        Destroy(this.gameObject);
    }
}
