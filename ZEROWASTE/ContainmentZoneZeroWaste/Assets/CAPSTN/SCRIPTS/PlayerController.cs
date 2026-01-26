using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }
    [Header("References")]
    [SerializeField] private GameObject buildPanel;
    [SerializeField] private GameObject towerGhostPrefab;

    public bool isBuilding = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        OnBuildPanelPressed();
    }

    public void OnBuildPanelPressed()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isBuilding) return;

            buildPanel.SetActive(!buildPanel.activeSelf);
        }
    }

    public void OnStartWaveBtnClicked()
    {
        if (isBuilding) return;

        foreach (GameObject spawner in GameObject.FindGameObjectsWithTag("Spawner"))
        {
            if(spawner.GetComponent<Spawner>() != null)
                spawner.GetComponent<Spawner>().StartWave();

            if (spawner.GetComponent<ScrapSpawner>() != null)
                spawner.GetComponent<ScrapSpawner>().StartSpawner();
        }
    }

    public void OnTowerBtnClicked(TowerData towerData)
    {
        if (isBuilding) return;

        if (!GameManager.Instance.CanSpend(towerData.resourceEntry))
            return;

        GameManager.Instance.SpendMaterials(towerData.resourceEntry);

        buildPanel.SetActive(false);
        isBuilding = true;

        towerGhostPrefab.GetComponent<TowerGhost>().GhostData = towerData;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0f;

        GameObject ghostGO = Instantiate(towerGhostPrefab, mouseWorldPos, Quaternion.identity);
    }
}
