using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Button[] skillButtonsPanels;
    [SerializeField] private GameObject[] towerButtonsPanels;
    [SerializeField] private GameObject towerGhostPrefab;
    [SerializeField] private GameObject targetLaserPrefab;

    [SerializeField] private CanvasGroup[] skillCanvasGroups;

    private void SetButtonState(int index, bool isEnabled)
    {
        skillCanvasGroups[index].interactable = isEnabled;
        skillCanvasGroups[index].blocksRaycasts = isEnabled;
        skillCanvasGroups[index].alpha = isEnabled ? 1f : 0.5f;
    }

    public bool isBuilding = false;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RefreshSkillButtons();
    }

    #region START WAVE BUTTON
    public void OnStartWaveBtnClicked()
    {
        if (isBuilding) return;

        foreach (GameObject spawner in GameObject.FindGameObjectsWithTag("Spawner"))
        {
            if(spawner.GetComponent<Spawner>() != null)
                spawner.GetComponent<Spawner>().StartWave();

            if (spawner.GetComponent<ScrapSpawner>() != null)
                spawner.GetComponent<ScrapSpawner>().StartSpawner();

            if (spawner.GetComponent<DumperSpawner>() != null)
                spawner.GetComponent<DumperSpawner>().StartSpawner();

        }
    }
    #endregion

    #region SKILL BUTTONS
    //public void RefreshSkillButtons()
    //{
    //    skillButtonsPanels[0].interactable =
    //        BuilderManager.Instance.IsStationTier3(BuildStationType.Printer);

    //    skillButtonsPanels[1].interactable =
    //        BuilderManager.Instance.IsStationTier3(BuildStationType.Forger);

    //    skillButtonsPanels[2].interactable =
    //        BuilderManager.Instance.IsStationTier3(BuildStationType.Modler);
    //}
    public void RefreshSkillButtons()
    {
        bool printerUnlocked =
            BuilderManager.Instance.IsStationTier3(BuildStationType.Printer);

        bool forgerUnlocked =
            BuilderManager.Instance.IsStationTier3(BuildStationType.Forger);

        bool modlerUnlocked =
            BuilderManager.Instance.IsStationTier3(BuildStationType.Modler);

        SetButtonState(0, printerUnlocked);
        SetButtonState(1, forgerUnlocked);
        SetButtonState(2, modlerUnlocked);
    }


    public void OnSolarRayBtnClicked()
    {
        GameObject targetGO = Instantiate(targetLaserPrefab);
    }
    #endregion

    #region BUILDER UPGRADE BUTTONS
    public void UpgradePrinter()
    {
        BuilderManager.Instance.UpgradeStation(BuildStationType.Printer);
    }

    public void UpgradeForger()
    {
        BuilderManager.Instance.UpgradeStation(BuildStationType.Forger);
    }

    public void UpgradeModler()
    {
        BuilderManager.Instance.UpgradeStation(BuildStationType.Modler);
    }
    #endregion

    #region BUILDER BUTTONS
    public void OnBuilderButtonClicked(int index)
    {
        for (int i = 0; i < towerButtonsPanels.Length; i++)
        {
            if (i == index)
            {
                towerButtonsPanels[i].SetActive(!towerButtonsPanels[i].activeSelf);
            }
            else
            {
                towerButtonsPanels[i].SetActive(false);
            }
        }
    }
    #endregion

    #region TOWER BUTTONS
    public void OnTowerBtnClicked(TowerData towerData)
    {
        if (isBuilding) return;

        if (!GameManager.Instance.CanSpend(towerData.resourceEntry))
            return;

        GameManager.Instance.SpendMaterials(towerData.resourceEntry);

        foreach(GameObject btn in towerButtonsPanels)
        {
            btn.SetActive(false);
        }

        isBuilding = true;

        towerGhostPrefab.GetComponent<TowerGhost>().GhostData = towerData;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0f;

        GameObject ghostGO = Instantiate(towerGhostPrefab, mouseWorldPos, Quaternion.identity);
    }
    #endregion
}
