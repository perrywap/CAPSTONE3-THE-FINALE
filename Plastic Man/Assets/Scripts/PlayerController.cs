using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("Dash Settings")]
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 4f;

    [Header("After Image Settings")]
    public GameObject afterImagePrefab;
    public float afterImageSpacing = 0.05f;

    private NavMeshAgent agent;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private bool isDashing;
    private bool canDash;
    private float dashTimer;
    private float imageTimer;
    private float cooldownTimer;

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

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (!isDashing)
            HandleWASDMovement();

        if (Keyboard.current.leftShiftKey.wasPressedThisFrame && cooldownTimer <= 0)
        {
            Dash();
        }

        HandleDash();
    }

    private void HandleWASDMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(horizontal, vertical, 0f).normalized;

        if (move.magnitude > 0.1f)
        {
            canDash = true;
            animator.SetBool("isMoving", true);
            agent.velocity = move * moveSpeed;
        }
        else
        {
            canDash = false;
            animator.SetBool("isMoving", false);
            agent.velocity = Vector3.zero;
        }

        if (Module.Instance != null)
            Module.Instance.HandleFrame(animator.GetBool("isMoving"));
    }

    private void Dash()
    {
        if (!canDash) return;

        isDashing = true;
        dashTimer = dashDuration;
        cooldownTimer = dashCooldown;
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
        if (afterImagePrefab == null) return;

        foreach (SpriteRenderer spr in this.GetComponentsInChildren<SpriteRenderer>())
        {
            GameObject img = Instantiate(afterImagePrefab, transform.position, transform.rotation);

            SpriteRenderer sr = img.GetComponent<SpriteRenderer>();
            sr.sprite = spr.sprite;
            sr.flipX = spr.flipX;

            sr.sortingLayerID = spr.sortingLayerID;
            sr.sortingOrder = spr.sortingOrder - 1;
        }
    }
}