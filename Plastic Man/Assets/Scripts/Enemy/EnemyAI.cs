using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 2f;
    [SerializeField] private float _detectionRadius = 5f;
    [SerializeField] private float _changeDirectionTime = 2f;
    [SerializeField] private float _randomMoveDistance = 3f;
    [SerializeField] private GameObject _player;

    private NavMeshAgent _agent;
    private float _changeDirectionTimer;

    private enum State { Wander, Chase }
    private State _currentState;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = _movementSpeed;
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        _currentState = State.Wander;
        SetNewRandomPosition();
        _changeDirectionTimer = _changeDirectionTime;

        if (_player == null)
        {
            _player = GameObject.FindWithTag("Player");
        }
    }

    void Update()
    {
        DetectPlayer();

        switch (_currentState)
        {
            case State.Wander:
                Wander();
                break;

            case State.Chase:
                ChasePlayer();
                break;
        }
    }

    private void Wander()
    {
        _changeDirectionTimer -= Time.deltaTime;

        if (_changeDirectionTimer <= 0f)
        {
            SetNewRandomPosition();
            _changeDirectionTimer = _changeDirectionTime;
        }
    }

    private void SetNewRandomPosition()
    {
        Vector3 randomPos = (Vector3)transform.position + new Vector3(
            Random.Range(-_randomMoveDistance, _randomMoveDistance),
            Random.Range(-_randomMoveDistance, _randomMoveDistance),
            0
        );

        _agent.SetDestination(randomPos);
    }

    private void DetectPlayer()
    {
        if (_player == null) return;

        float distance = Vector2.Distance(transform.position, _player.transform.position);

        if (distance < _detectionRadius)
        {
            _currentState = State.Chase;
        }
        else if (_currentState == State.Chase)
        {
            _currentState = State.Wander;
            SetNewRandomPosition();
        }
    }

    private void ChasePlayer()
    {
        if (_player == null) return;
        _agent.SetDestination(_player.transform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}