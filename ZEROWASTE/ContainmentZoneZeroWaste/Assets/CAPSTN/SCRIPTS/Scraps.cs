using System;
using UnityEngine;

public enum ScrapType
{
    CLEAN,
    SLUDGE
}

public class Scraps : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform washStation;
    [SerializeField] private Transform salvageStation; 
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Sprite cleanSprite;
    [SerializeField] private Sprite sludgedSprite;


    [Header("Attributes")]
    [SerializeField] private float movespeed = 0.5f;
    [SerializeField] private ScrapType scrapType;
    
    private int currentIndex;
    private bool isClicked;

    public Transform[] Waypoints { get { return waypoints; } set { waypoints = value; } }

    private void Update()
    {
        Move();
    }

    public void InitializeScrap(Transform _washStation, Transform _salvageStation, Transform[] _waypoints)
    {
        washStation = _washStation;
        salvageStation = _salvageStation;
        waypoints = _waypoints;
    }

    public virtual void Move()
    {
        if (waypoints.Length == 0)
            return;

        if (currentIndex <= waypoints.Length - 1 && !isClicked)
        {
            transform.position = Vector2.MoveTowards(transform.position, waypoints[currentIndex].position, movespeed * Time.deltaTime);

            if (transform.position == waypoints[currentIndex].position)
            {
                currentIndex++;
            }

            if (currentIndex == waypoints.Length)
            {
                OnPathComplete();
            }
        }

        if(isClicked)
        {
            if (scrapType == ScrapType.CLEAN)
                GoToStation(salvageStation);
            else if (scrapType == ScrapType.SLUDGE)
                GoToStation(washStation);
        }
    }

    private void GoToStation(Transform stationPos)
    {
        transform.position = Vector2.MoveTowards(transform.position, stationPos.position, movespeed * 20f * Time.deltaTime);

        float distance = Vector3.Distance(this.transform.position, stationPos.position);
        if (distance <= 0.01)
        {
            Destroy(gameObject);
        }
    }

    private void OnPathComplete()
    {
        Destroy(gameObject);
    }

    public void OnScrapClicked()
    {
        isClicked = true;
    }
}
