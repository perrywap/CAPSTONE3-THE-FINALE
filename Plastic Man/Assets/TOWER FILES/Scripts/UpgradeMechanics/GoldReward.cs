using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldReward : MonoBehaviour
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void OnGoldIconClicked()
    {
        anim.SetTrigger("FadeOut");
    }

    public void ShowUpgrades()
    {
        RewardsPanel.Instance.ShowRewards();
    }
}
