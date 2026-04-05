//////using UnityEngine;

//////public abstract class WeaponBase : MonoBehaviour
//////{
//////    [Header("Weapon Settings")]
//////    [SerializeField] protected float fireRate = 0.2f;
//////    [SerializeField] protected float damage = 10f;
//////    [SerializeField] protected bool isAutomatic = false;

//////    protected Animator animator;
//////    protected float lastFireTime;

//////    private void Awake()
//////    {
//////        animator = GetComponent<Animator>();

//////        if (animator == null)
//////            Debug.LogError($"{gameObject.name} is missing an Animator!");
//////    }

//////    public void HandleInput(Vector3 mousePosition)
//////    {
//////        if (isAutomatic)
//////        {
//////            //if (Input.GetMouseButton(0) && !CursorManager.Instance.isUsingDefaultCursor)
//////            if (Input.GetMouseButton(0))
//////                TryFire(mousePosition);
//////        }
//////        else
//////        {
//////            //if (Input.GetMouseButtonDown(0) && !CursorManager.Instance.isUsingDefaultCursor)
//////            if (Input.GetMouseButtonDown(0))
//////                TryFire(mousePosition);
//////        }
//////    }

//////    public void TryFire(Vector3 mousePosition)
//////    {
//////        if (Time.time < lastFireTime + fireRate)
//////            return;

//////        lastFireTime = Time.time;

//////        if (animator != null)
//////            animator.SetTrigger("fire");

//////        Vector3 direction = (mousePosition - transform.position).normalized;
//////        Fire(transform.position, direction);
//////    }

//////    protected abstract void Fire(Vector3 origin, Vector3 direction);
//////}

////using UnityEngine;

////public abstract class WeaponBase : MonoBehaviour
////{
////    [Header("Weapon Settings")]
////    [SerializeField] protected float fireRate = 0.2f;
////    [SerializeField] protected float damage = 10f;
////    [SerializeField] protected bool isAutomatic = false;

////    [Header("Energy settings")]
////    public float maxEnergy = 100f;
////    public float currentEnergy;
////    public float depleteAmount = 1f;
////    public float regenRate = 0.5f;

////    protected Animator animator;
////    protected float lastFireTime;

////    private void Awake()
////    {
////        animator = GetComponent<Animator>();

////        if (animator == null)
////            Debug.LogError($"{gameObject.name} is missing an Animator!");
////    }

////    private void Start()
////    {
////        currentEnergy = maxEnergy;
////    }

////    private void Update()
////    {
////        RegenEnergy();
////    }

////    public void HandleInput(Vector3 mousePosition)
////    {
////        if (isAutomatic)
////        {
////            if (Input.GetMouseButton(0))
////                TryFire(mousePosition);
////        }
////        else
////        {
////            if (Input.GetMouseButtonDown(0))
////                TryFire(mousePosition);
////        }
////    }

////    public void TryFire(Vector3 mousePosition)
////    {
////        if (Time.time < lastFireTime + fireRate)
////            return;

////        if (currentEnergy < depleteAmount)
////            return;

////        lastFireTime = Time.time;

////        if (animator != null)
////            animator.SetTrigger("fire");

////        Vector3 direction = (mousePosition - transform.position).normalized;
////        ConsumeEnergy();
////        Fire(transform.position, direction);
////    }

////    public void ConsumeEnergy()
////    {
////        currentEnergy -= depleteAmount;

////        if (currentEnergy < 0f)
////            currentEnergy = 0f;
////    }

////    private void RegenEnergy()
////    {
////        if (currentEnergy >= maxEnergy)
////            return;

////        currentEnergy += regenRate * Time.deltaTime;

////        if (currentEnergy > maxEnergy)
////            currentEnergy = maxEnergy;
////    }

////    protected abstract void Fire(Vector3 origin, Vector3 direction);
////}

//using UnityEngine;

//public abstract class WeaponBase : MonoBehaviour
//{
//    [Header("Weapon Settings")]
//    [SerializeField] protected float fireRate = 0.2f;
//    [SerializeField] protected float damage = 10f;
//    [SerializeField] protected bool isAutomatic = false;

//    [Header("Energy settings")]
//    public float maxEnergy = 100f;
//    public float currentEnergy;
//    public float depleteAmount = 1f;
//    public float regenRate = 0.5f;

//    protected Animator animator;
//    protected float lastFireTime;

//    public float CurrentEnergy => currentEnergy;
//    public float MaxEnergy => maxEnergy;

//    private void Awake()
//    {
//        animator = GetComponent<Animator>();

//        if (animator == null)
//            Debug.LogError($"{gameObject.name} is missing an Animator!");
//    }

//    private void Start()
//    {
//        if (currentEnergy <= 0f)
//            currentEnergy = maxEnergy;
//    }

//    private void Update()
//    {
//        RegenEnergy();
//    }

//    public void HandleInput(Vector3 mousePosition)
//    {
//        if (isAutomatic)
//        {
//            if (Input.GetMouseButton(0))
//                TryFire(mousePosition);
//        }
//        else
//        {
//            if (Input.GetMouseButtonDown(0))
//                TryFire(mousePosition);
//        }
//    }

//    public void TryFire(Vector3 mousePosition)
//    {
//        if (Time.time < lastFireTime + fireRate)
//            return;

//        if (currentEnergy < depleteAmount)
//            return;

//        lastFireTime = Time.time;

//        if (animator != null)
//            animator.SetTrigger("fire");

//        Vector3 direction = (mousePosition - transform.position).normalized;
//        ConsumeEnergy();
//        Fire(transform.position, direction);
//    }

//    public void ConsumeEnergy()
//    {
//        currentEnergy -= depleteAmount;

//        if (currentEnergy < 0f)
//            currentEnergy = 0f;
//    }

//    public void SetEnergy(float value)
//    {
//        currentEnergy = Mathf.Clamp(value, 0f, maxEnergy);
//    }

//    private void RegenEnergy()
//    {
//        if (currentEnergy >= maxEnergy)
//            return;

//        currentEnergy += regenRate * Time.deltaTime;

//        if (currentEnergy > maxEnergy)
//            currentEnergy = maxEnergy;
//    }

//    protected abstract void Fire(Vector3 origin, Vector3 direction);
//}

using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] protected float fireRate = 0.2f;
    [SerializeField] protected float damage = 10f;
    [SerializeField] protected bool isAutomatic = false;

    [Header("Energy settings")]
    public float maxEnergy = 100f;
    public float currentEnergy;
    public float depleteAmount = 1f;
    public float regenRate = 0.5f;

    protected Animator animator;
    protected float lastFireTime;

    public float CurrentEnergy => currentEnergy;
    public float MaxEnergy => maxEnergy;
    public float RegenRate => regenRate;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        if (animator == null)
            Debug.LogError($"{gameObject.name} is missing an Animator!");
    }

    private void Start()
    {
        if (currentEnergy <= 0f)
            currentEnergy = maxEnergy;
    }

    public void HandleInput(Vector3 mousePosition)
    {
        if (isAutomatic)
        {
            if (Input.GetMouseButton(0))
                TryFire(mousePosition);
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
                TryFire(mousePosition);
        }
    }

    public void TryFire(Vector3 mousePosition)
    {
        if (Time.time < lastFireTime + fireRate)
            return;

        if (currentEnergy < depleteAmount)
            return;

        lastFireTime = Time.time;

        if (animator != null)
            animator.SetTrigger("fire");

        Vector3 direction = (mousePosition - transform.position).normalized;
        ConsumeEnergy();
        Fire(transform.position, direction);
    }

    public void ConsumeEnergy()
    {
        currentEnergy -= depleteAmount;

        if (currentEnergy < 0f)
            currentEnergy = 0f;
    }

    public void SetEnergy(float value)
    {
        currentEnergy = Mathf.Clamp(value, 0f, maxEnergy);
    }

    protected abstract void Fire(Vector3 origin, Vector3 direction);
}