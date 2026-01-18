using UnityEngine;
using TMPro;
using System.Collections.Generic;
using NUnit.Framework;

public class GameManager : MonoBehaviour
{
    #region VARIABLES
    public static GameManager Instance { get; private set; }

    [Header("PLAYER HUD")]
    [SerializeField] private TextMeshProUGUI healthTxt;
    [SerializeField] private TextMeshProUGUI filamentTxt;
    [SerializeField] private TextMeshProUGUI waveTxt;
    [SerializeField] private GameObject startWaveBtn;

    public bool _isWaveRunning;

    [Header("PLAYER STATS")]
    [SerializeField] private int _health;
    [SerializeField] private int _filament;

    [Header("SPAWNED UNITS")]
    public List<GameObject> _enemies = new List<GameObject>();

    public TextMeshProUGUI WaveText { get { return waveTxt; } set { waveTxt = value; } }
    #endregion

    #region UNITY METHODS
    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        HudManager();
    }
    #endregion

    #region METHODS
    private void HudManager()
    {
        healthTxt.text = _health.ToString();
        filamentTxt.text = _filament.ToString();

        startWaveBtn.SetActive(_isWaveRunning || _enemies.Count != 0 ? false : true);
    }
    #endregion
}

