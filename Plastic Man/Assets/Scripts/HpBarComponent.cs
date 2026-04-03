//using Unity.VisualScripting;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//public class HpBarComponent : MonoBehaviour
//{
//    [SerializeField] private EnemyHealth enemyHealth;
//    [SerializeField] private Image hpBar;
//    [SerializeField] private TextMeshProUGUI hpText;

//    private Animator animator;
//    private float currentHp;
//    private float maxHp;


//    private void Start()
//    {
//        animator = GetComponent<Animator>();

//        if (GetComponentInParent<EnemyHealth>() != null) 
//            enemyHealth = GetComponentInParent<EnemyHealth>();

//        currentHp = enemyHealth.CurrentHealth;
//        maxHp = enemyHealth.MaxHealth;

//        hpBar.fillAmount = currentHp / maxHp;
//        hpText.text = currentHp.ToString();
//    }

//    private void Update()
//    {
//        currentHp = enemyHealth.CurrentHealth;
//        maxHp = enemyHealth.MaxHealth;

//    }

//    public void DecreaseHp()
//    {
//        animator.SetTrigger("damaged");
//    }    

//}
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HpBarComponent : MonoBehaviour
{
    [SerializeField] private EnemyHealth enemyHealth;
    [SerializeField] private Image hpBar;
    [SerializeField] private TextMeshProUGUI hpText;

    private Animator animator;
    private float currentHp;
    private float maxHp;

    [Header("Visibility Settings")]
    [SerializeField] private float visibleDuration = 2f;

    private float visibleTimer;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        // Ensure there's a CanvasGroup
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (GetComponentInParent<EnemyHealth>() != null)
            enemyHealth = GetComponentInParent<EnemyHealth>();

        maxHp = enemyHealth.MaxHealth;
        currentHp = enemyHealth.CurrentHealth;

        UpdateHpUI();

        HideInstant(); // Start hidden
    }

    private void Update()
    {
        currentHp = enemyHealth.CurrentHealth;
        maxHp = enemyHealth.MaxHealth;

        UpdateHpUI();

        // Handle auto-hide
        if (visibleTimer > 0)
        {
            visibleTimer -= Time.deltaTime;

            if (visibleTimer <= 0)
            {
                Hide();
            }
        }
    }

    private void UpdateHpUI()
    {
        hpBar.fillAmount = currentHp / maxHp;
        hpText.text = Mathf.CeilToInt(currentHp).ToString();
    }

    public void OnDamaged()
    {
        Show();

        if (animator != null)
            animator.SetTrigger("damaged");

        visibleTimer = visibleDuration;
    }

    private void Show()
    {
        canvasGroup.alpha = 1;
    }

    private void Hide()
    {
        canvasGroup.alpha = 0;
    }

    private void HideInstant()
    {
        canvasGroup.alpha = 0;
        visibleTimer = 0;
    }
}