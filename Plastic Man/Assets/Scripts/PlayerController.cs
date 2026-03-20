using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    #region Variables
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;

    [Header("After Image Settings")]
    public GameObject afterImagePrefab;
    public float afterImageSpacing = 0.05f;

    private NavMeshAgent agent;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isDashing;
    private float dashTimer;
    private float imageTimer;
    #endregion

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Update()
    {
        if (animator != null)
        {
            float angle = PlayerLookAt.Instance.angle;
            animator.SetFloat("angle", angle);
        }

        if (!isDashing)
            HandleWASDMovement();

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            Dash();

        HandleDash();
    }

    private void HandleWASDMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(horizontal, vertical, 0f).normalized;

        if (move.magnitude > 0f)
        {
            animator.SetBool("isMoving", true);
            agent.velocity = move * moveSpeed;
        }
        else
        {
            animator.SetBool("isMoving", false);
            agent.velocity = Vector3.zero;
        }

        foreach (Module module in this.gameObject.GetComponentsInChildren<Module>())
        {
            module.HandleFrame(animator.GetBool("isMoving"));
        }
    }

    private void Dash()
    {
        isDashing = true;
        dashTimer = dashDuration;
    }

    private void HandleDash()
    {
        if (!isDashing) return;

        dashTimer -= Time.deltaTime;

        Vector3 dashDirection = agent.velocity.normalized;
        agent.velocity = dashDirection * dashSpeed;

        imageTimer -= Time.deltaTime;

        if (imageTimer <= 0f)
        {
            SpawnAfterImage();
            imageTimer = afterImageSpacing;
        }

        if (dashTimer <= 0f)
        {
            isDashing = false;
        }
    }

    private void SpawnAfterImage()
    {
        GameObject img = Instantiate(afterImagePrefab, transform.position, transform.rotation);

        SpriteRenderer sr = img.GetComponent<SpriteRenderer>();
        sr.sprite = spriteRenderer.sprite;
        sr.flipX = spriteRenderer.flipX;

        sr.sortingLayerID = spriteRenderer.sortingLayerID;
        sr.sortingOrder = spriteRenderer.sortingOrder - 1;
    }
}