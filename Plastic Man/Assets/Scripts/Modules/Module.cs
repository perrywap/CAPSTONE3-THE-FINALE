using UnityEngine;

public class Module : MonoBehaviour
{
    public static Module Instance { get; private set; }

    protected Animator animator;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if(animator == null)
        {
            if(this.GetComponent<Animator>() != null)
                animator = this.GetComponent<Animator>();
        }
    }

    public void HandleFrame(bool isMoving)
    {
        if(animator != null)
        {
            animator.SetBool("isMoving", isMoving);
        }
    }
}
