using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance {get; private set;}
    
    private void Awake()
    {
        Instance = this;
    }

    public void ReturnToBigMap()
    {
         SceneManager.LoadScene("LevelSelect");    
    }

    public void OnRetryBtnClicked()
    {

    }

    public void OnQuitBtnClicked()
    {

    }
}
