using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HudManager : MonoBehaviour
{
    public static HudManager Instance { get; private set; }

    [SerializeField] private GameObject summonPanel;
    [SerializeField] private GameObject fader;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        summonPanel.SetActive(GameManager.Instance.isGameFinished ? false : true);
    }

    public void FadeOut()
    {
        fader.SetActive(true);
        fader.GetComponent<Animation>().Play();
        
    }

}
