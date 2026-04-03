using UnityEngine;
using UnityEngine.UI;

public class PrinterManager : MonoBehaviour
{

    [SerializeField] private Transform printedSlot;
    [SerializeField] private Transform moduleSlot;
    [SerializeField] private Transform filamentSlot;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }


}
