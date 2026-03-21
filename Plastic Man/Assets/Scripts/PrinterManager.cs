using UnityEngine;
using UnityEngine.UI;

public class PrinterManager : MonoBehaviour
{
    public static PrinterManager Instance { get; private set; }

    [SerializeField] private Transform movingPart;
    [SerializeField] private Transform startPos, endPos;
    [SerializeField] private Image moduleToPrint;

    private float moduleFill = 0f;

    private Animator animator;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
        animator.SetBool("isMoving", true);
        moduleToPrint.fillAmount = moduleFill;
    }

    private void Update()
    {
        if (movingPart != null)
        {
            moduleToPrint.fillAmount = moduleFill   ;
        }
    }

    public void OnPrintBtnClicked()
    {

    }

    public void Print()
    {
        if (movingPart.position.y >= endPos.position.y)
        {
            animator.SetBool("isMoving", false);
            return;
        }
            

        //float posY = movingPart.position.y + 27.5f;
        moduleFill += .1f;
        movingPart.position += new Vector3(0f, 27.5f * 1.5f, 0f);
    }
}
