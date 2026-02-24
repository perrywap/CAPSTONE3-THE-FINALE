using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject playerBody;
    [SerializeField] private float moveSpeed = 5f;

    [SerializeField] private PlayerAimWeapon playerAimWeapon;
    private Animator animator;
    private NavMeshAgent agent;
    public Camera mainCamera;
    public bool useWASD = true; 

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        animator = playerBody.GetComponent<Animator>();

        playerAimWeapon.OnShoot += PlayerAimWeapon_OnShoot;
    }

    private void Update()
    {
        if (useWASD)
            HandleWASDMovement();
        else
            HandlePointClickMovement();
    }

    private void HandleWASDMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector3 move = new Vector3(horizontal, vertical, 0f).normalized;

        if (horizontal > 0f)
            transform.localScale = new Vector3(1f, 1f, 1f);
        else if (horizontal < 0f)
            transform.localScale = new Vector3(-1f, 1f, 1f);

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
    }

    private void HandlePointClickMovement1()
    {
        if (Mouse.current.rightButton.isPressed)
        {
            
            Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
            worldPos.z = 0f;
            agent.SetDestination(worldPos);
        }
    }

    private void HandlePointClickMovement()
    {
        if (Mouse.current.rightButton.isPressed)
        {
            Vector3 mouseScreenPos = Mouse.current.position.ReadValue();
            Vector3 worldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
            worldPos.z = 0f;

            Vector3 direction = (worldPos - transform.position).normalized;
            agent.SetDestination(worldPos);

            if (direction.x > 0f)
                transform.localScale = new Vector3(1f, 1f, 1f);
            else if (direction.x < 0f)
                transform.localScale = new Vector3(-1f, 1f, 1f);

            if (direction.magnitude > 0f)
                animator.SetBool("isMoving", true);
            else
                animator.SetBool("isMoving", false);
        }
        if(agent.velocity ==  Vector3.zero)
        {
            animator.SetBool("isMoving", false);
        }
    }

    private void PlayerAimWeapon_OnShoot(object sender, PlayerAimWeapon.OnShootEventArgs e)
    {
        Debug.Log("Shooted");

        Vector3 gunEnd = e.gunEndPointPosition;
        Vector3 target = e.shootPosition;

        Debug.DrawLine(gunEnd, target, Color.red, 1f);
    }
}