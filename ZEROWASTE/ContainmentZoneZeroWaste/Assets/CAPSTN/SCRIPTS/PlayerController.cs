using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public void OnStartWaveBtnClicked()
    {
        foreach(GameObject spawner in GameObject.FindGameObjectsWithTag("Spawner"))
        {
            spawner.GetComponent<Spawner>().StartWave();
        }
    }
}
