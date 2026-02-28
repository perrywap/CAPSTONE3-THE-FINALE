using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float _movementSpeed = 2f;
    [SerializeField] private float _detectionRadius = 5f;
    [SerializeField] private float _changeDirectionTime = 2f;
    [SerializeField] private float _randomMoveDistance = 3f;
    [SerializeField] private GameObject _player;

    private Vector2 _targetPosition;
    private float _changeDirectionTimer;

    private enum State { Wander, Chase }
    private State _currentState;

    void Start()
    {
        _currentState = State.Wander;
        SetNewRandomPosition();
        _changeDirectionTimer = _changeDirectionTime;

        if (_player == null)
        {
            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                _player = playerObject;
            }
            else
            {
                Debug.LogError("Player object not found! Make sure it has the tag 'Player'.");
            }
        }
    }

    void Update()
    {
        switch (_currentState)
        {
            case State.Wander:
                Wander();
                break;

            case State.Chase:
                ChasePlayer();
                break;
        }

        DetectPlayer();
    }

    private void Wander()
    {
        transform.position = Vector2.MoveTowards(
            transform.position,
            _targetPosition,
            _movementSpeed * Time.deltaTime
        );

        _changeDirectionTimer -= Time.deltaTime;

        if (_changeDirectionTimer <= 0f)
        {
            SetNewRandomPosition();
            _changeDirectionTimer = _changeDirectionTime;
        }
    }

    private void SetNewRandomPosition()
    {
        float randomX = Random.Range(-_randomMoveDistance, _randomMoveDistance);
        float randomY = Random.Range(-_randomMoveDistance, _randomMoveDistance);

        _targetPosition = (Vector2)transform.position + new Vector2(randomX, randomY);
    }

    private void DetectPlayer()
    {
        if (_player == null) return;

        float distance = Vector2.Distance(transform.position, _player.transform.position);

        if (distance < _detectionRadius)
        {
            _currentState = State.Chase;
        }
        else
        {
            _currentState = State.Wander;
        }
    }

    private void ChasePlayer()
    {
        if (_player == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            _player.transform.position,
            _movementSpeed * Time.deltaTime
        );
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
    }
}
