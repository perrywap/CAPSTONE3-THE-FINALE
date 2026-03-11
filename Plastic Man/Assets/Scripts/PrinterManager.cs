using UnityEngine;

public class PrinterManager : MonoBehaviour
{
    public static PrinterManager Instance { get; private set; }

    [SerializeField] private float fillamentAmount;
    [SerializeField] private Transform fillamentSlot;


    private void Awake()
    {
        Instance = this;
    }



    public void OnPrintBtnClicked()
    {

    }
}
