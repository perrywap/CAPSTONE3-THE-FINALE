using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("MANA MANAGEMENT")]
    [SerializeField] private Image manaImage;           // UI
    [SerializeField] private Text manatext;             // UI
    [SerializeField] private float currentMana;
    [SerializeField] private float maxMana;
    [SerializeField] private float regenAmount;
    [SerializeField] private float regenRate = 5f;

    public float CurrentMana { get { return currentMana; } }

    [Header("GAME STATE")]
    public GameObject winPanel;
    public GameObject losePanel;
    [SerializeField] private int cardsOnHandCount;
    [SerializeField] private int unitsOnFieldCount;
    public List<GameObject> unitsOnField = new List<GameObject>();
    public List<GameObject> cardsOnHand = new List<GameObject>();
    public bool isGameFinished;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        currentMana = maxMana;
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        isGameFinished = false;

        StartCoroutine(RegenerateMana());
    }

    private void Update()
    {
        ManaManager();

        cardsOnHandCount = cardsOnHand.Count;
        unitsOnFieldCount = unitsOnField.Count;

        if (!isGameFinished && cardsOnHandCount == 0 && unitsOnFieldCount == 0)
        {
            StartCoroutine(DelayedLoseCheck());
        }
    }

    private IEnumerator DelayedLoseCheck()
    {
        yield return new WaitForSeconds(2.0f);

        if (cardsOnHand.Count == 0 && unitsOnField.Count == 0)
        {
            losePanel.SetActive(true);
            isGameFinished = true;
        }
    }

    #region MANA MANAGEMENT
    private void ManaManager()
    {
        if(currentMana >= maxMana)
            currentMana = maxMana;            

        if (currentMana < 0)
            currentMana = 0;

        manatext.text = currentMana.ToString();
        manaImage.fillAmount = currentMana / maxMana;
    }

    public void UseMana(int cost)
    {
        currentMana -= cost;
    }

    private IEnumerator RegenerateMana()
    {
        while (true)
        {
            if (currentMana < maxMana)
            {
                yield return new WaitForSeconds(regenRate);
                currentMana += regenAmount;
            }
            else
            {
                yield return null;
            }
        }
    }
    #endregion


}
