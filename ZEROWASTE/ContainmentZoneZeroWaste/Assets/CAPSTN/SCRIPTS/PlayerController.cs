using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public void OnStartWaveBtnClicked()
    {
        foreach(GameObject spawner in GameObject.FindGameObjectsWithTag("Spawner"))
        {
            if(spawner.GetComponent<Spawner>() != null)
                spawner.GetComponent<Spawner>().StartWave();

            if (spawner.GetComponent<ScrapSpawner>() != null)
                spawner.GetComponent<ScrapSpawner>().StartSpawner();
        }
    }
}
