using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMovement : MonoBehaviour
{
    [SerializeField] private GameObject wpManager;
    [SerializeField] private Transform[] points;
    [SerializeField] private int pointIndex;

    [Header("Walking Sound")]
    [SerializeField] private AudioClip walkingSound;

    private AudioController audioController;
    private AudioSource walkingSource;
    private bool isWalkingSoundPlaying = false;

    private Unit unit;

    public Transform[] Points { get { return points; } }
    public Unit Flyer { get { return unit; } }

    private void Start()
    {
        unit = GetComponent<Unit>();

        wpManager = GameObject.FindGameObjectWithTag("WaypointManager");
        points = wpManager.GetComponent<Waypoint>().waypoints;
        transform.position = points[pointIndex].transform.position;

        audioController = FindObjectOfType<AudioController>();

        if (audioController != null)
        {
            walkingSource = audioController.gameObject.AddComponent<AudioSource>();
            walkingSource.loop = true;
            walkingSource.playOnAwake = false;
            walkingSource.clip = walkingSound;
            walkingSource.outputAudioMixerGroup = audioController.soundSource.outputAudioMixerGroup;
        }
    }

    private void Update()
    {
        Move();
    }

    public virtual void Move()
    {
        if (unit.State != UnitState.WALKING)
        {
            StopWalkingSound();
            return;
        }

        if (pointIndex <= points.Length - 1)
        {
            StartWalkingSound();

            transform.position = Vector2.MoveTowards(transform.position, points[pointIndex].transform.position, unit.Speed * Time.deltaTime);

            if (transform.position == points[pointIndex].transform.position)
            {
                pointIndex++;
            }

            if (pointIndex == points.Length)
            {
                StopWalkingSound();
                unit.OnPathComplete();
            }
        }
    }

    private void StartWalkingSound()
    {
        if (!isWalkingSoundPlaying && walkingSource != null && walkingSound != null)
        {
            walkingSource.Play();
            isWalkingSoundPlaying = true;
        }
    }

    private void StopWalkingSound()
    {
        if (isWalkingSoundPlaying && walkingSource != null)
        {
            walkingSource.Stop();
            isWalkingSoundPlaying = false;
        }
    }
}
