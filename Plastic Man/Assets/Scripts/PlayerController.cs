using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private NavMeshAgent agent;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
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

        HandleWASDMovement();
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
    }
}