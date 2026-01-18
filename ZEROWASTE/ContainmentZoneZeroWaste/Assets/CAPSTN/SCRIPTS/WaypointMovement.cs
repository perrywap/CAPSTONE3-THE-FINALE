using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class WaypointMovement : MonoBehaviour
{
    #region VARIABLES
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private int currentIndex;

    private EnemyBase enemy;
    #endregion

    #region GETTERS AND SETTERS
    public Transform[] Waypoints { get { return waypoints; } set { waypoints = value; } }
    #endregion

    #region UNITY METHODS
    private void Start()
    {
        enemy = GetComponent<EnemyBase>();
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

        if (currentIndex <= waypoints.Length - 1)
        {
            transform.position = Vector2.MoveTowards(transform.position, waypoints[currentIndex].position, enemy.MoveSpeed * Time.deltaTime);

            if (transform.position == waypoints[currentIndex].position)
            {
                currentIndex++;
            }

            if (currentIndex == waypoints.Length)
            {
                OnPathComplete();
            }
        }
    }

    private void OnPathComplete()
    {
        this.gameObject.GetComponent<EnemyBase>().Die();
    }
    #endregion
}