using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PersistentData : MonoBehaviour
{
    public static PersistentData Instance { get; private set; }

    [Header("UNITS")]
    [SerializeField] private Text unitsOwnedTxt;
    public List<GameObject> unitsOwned = new List<GameObject>();
    public List<GameObject> unitsNotOwned = new List<GameObject>();
    public List<UnitData> unitDatas = new List<UnitData>();
    public List<GameObject> units = new List<GameObject>();
    public List<GameObject> upgradeCards = new List<GameObject>();


    [Header("GOLD MANAGEMENT")]
    [SerializeField] private Text goldTxt;
    public int gold = 0;


    [Header("Scene Load")]
    [SerializeField] private string scene;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(scene);
    }

    private void Update()
    {
        unitsOwnedTxt.text = unitsOwned.Count.ToString();
        goldTxt.text = gold.ToString();

        if (Input.mouseScrollDelta.y != 0)
        {
            Debug.Log("Mouse Scroll Delta Y: " + Input.mouseScrollDelta.y);
        }

        for (int i = 0; i < units.Count; i++)
        {
            GameObject unit = units[i];
            if (!unitsOwned.Contains(unit) && !unitsNotOwned.Contains(unit))
            {
                unitsNotOwned.Add(unit);
            }
        }

        for (int i = 0; i < unitsOwned.Count; i++)
        {
            GameObject newUnit = unitsOwned[i];
            if (unitsNotOwned.Contains(newUnit))
            {
                unitsNotOwned.Remove(newUnit);
            }
        }
    }
}
