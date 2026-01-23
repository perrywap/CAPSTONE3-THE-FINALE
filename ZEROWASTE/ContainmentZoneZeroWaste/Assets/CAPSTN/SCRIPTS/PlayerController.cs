using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject buildPanel;
    [SerializeField] private GameObject towerGhostPrefab;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            buildPanel.SetActive(!buildPanel.activeSelf);
        }
    }

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

    public void OnTowerBtnClicked(TowerData towerData)
    {
        buildPanel.SetActive(false);
        towerGhostPrefab.GetComponent<TowerGhost>().GhostData = towerData;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0f;

        GameObject ghostGO = Instantiate(towerGhostPrefab, mouseWorldPos, Quaternion.identity);
    }
}
