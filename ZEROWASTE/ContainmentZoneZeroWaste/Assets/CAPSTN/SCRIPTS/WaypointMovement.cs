//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEditor;
//using UnityEngine;

//public class WaypointMovement : MonoBehaviour
//{
//    #region VARIABLES
//    [SerializeField] private Transform[] waypoints;
//    [SerializeField] private int currentIndex;

//    private EnemyBase enemy;
//    #endregion

//    #region GETTERS AND SETTERS
//    public Transform[] Waypoints { get { return waypoints; } set { waypoints = value; } }
//    #endregion

//    #region UNITY METHODS
//    private void Start()
//    {
//        enemy = GetComponent<EnemyBase>();
//    }

//    private void Update()
//    {
//        Move();
//    }
//    #endregion

//    #region METHODS
//    public virtual void Move()
//    {
//        if (waypoints.Length == 0 || enemy.IsDead)
//            return;

//        if (currentIndex <= waypoints.Length - 1)
//        {
//            transform.position = Vector2.MoveTowards(transform.position, waypoints[currentIndex].position, enemy.MoveSpeed * Time.deltaTime);

//            if (transform.position == waypoints[currentIndex].position)
//            {
//                currentIndex++;
//            }

//            if (currentIndex == waypoints.Length)
//            {
//                OnPathComplete();
//            }
//        }
//    }

//    private void OnPathComplete()
//    {
//        this.gameObject.GetComponent<EnemyBase>().Die();
//    }
//    #endregion
//}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public enum MoveDirection
{
    UP,
    DOWN,
    LEFT,
    RIGHT,
    UP_LEFT,
    UP_RIGHT,
    DOWN_LEFT,
    DOWN_RIGHT
}

public class WaypointMovement : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private int currentIndex;

    public MoveDirection direction;

    private EnemyBase enemy;
    private Animator animator; // 1. Reference to Animator
    private SpriteRenderer spriteRenderer; // 2. Reference to flip the sprite
    
    #endregion

    #region GETTERS AND SETTERS
    public Transform[] Waypoints { get { return waypoints; } set { waypoints = value; } }
    #endregion

    #region UNITY METHODS
    private void Start()
    {
        enemy = GetComponent<EnemyBase>();
        animator = GetComponent<Animator>(); // Get the components
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Move();
    }
    #endregion

    #region METHODS
    public virtual void Move()
    {
        if (waypoints.Length == 0 || enemy.IsDead)
            return;

        if (currentIndex < waypoints.Length)
        {
            Vector3 targetPosition = waypoints[currentIndex].position;
            Vector3 moveVector = (targetPosition - transform.position).normalized;

            UpdateAnimation(moveVector);

            transform.position = Vector2.MoveTowards(transform.position, targetPosition, enemy.MoveSpeed * Time.deltaTime);

            if (transform.position == targetPosition)
            {
                currentIndex++;

                if (currentIndex >= waypoints.Length)
                {
                    OnPathComplete();
                }
            }
        }
    }

    private void UpdateAnimation(Vector3 dir)
    {
        if (dir.x < -0.1f)
        {
            spriteRenderer.flipX = true;
        }
        else if (dir.x > 0.1f)
        {
            spriteRenderer.flipX = false;
        }
        animator.SetFloat("DirX", Mathf.Abs(dir.x));
        animator.SetFloat("DirY", dir.y);
    }

    private void OnPathComplete()
    {
        this.gameObject.GetComponent<EnemyBase>().Die();
    }
    #endregion
}