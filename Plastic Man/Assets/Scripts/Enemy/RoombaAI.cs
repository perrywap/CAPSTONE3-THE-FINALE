using UnityEngine;
using UnityEngine.AI;

public class RoombaAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _movementSpeed = 2f;
    [SerializeField] private float _detectionRadius = 10f;
    [SerializeField] private float _attackRadius = 6f;
    [SerializeField] private float _attackCooldown = 3f;
    [SerializeField] private float _changeDirectionTime = 3f;
    [SerializeField] private float _randomMoveDistance = 5f;
    [SerializeField] private GameObject _player;

    [SerializeField] private LayerMask _obstacleMask;

    [Header("Ranged Combat")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _bulletSpeed = 10f;

    private NavMeshAgent _agent;
    private SpriteRenderer _spriteRenderer;
    private float _changeDirectionTimer;
    private float _attackTimer;

    private enum State { Wander, Chase, Attack }
    private State _currentState;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _agent.speed = _movementSpeed;
        _agent.stoppingDistance = _attackRadius - 1f;
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        if (_player == null) _player = GameObject.FindGameObjectWithTag("Player");

        _currentState = State.Wander;
        SetNewRandomPosition();
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

        if (_attackTimer > 0) _attackTimer -= Time.deltaTime;

        DetectPlayer();

        switch (_currentState)
        {
            case State.Wander: Wander(); break;
            case State.Chase: ChasePlayer(); break;
            case State.Attack: PerformAttack(); break;
        }

        FlipSprite();
    }

    private void DetectPlayer()
    {
        if (_player == null) return;

        float distance = Vector2.Distance(transform.position, _player.transform.position);

        bool hasLineOfSight = false;
        if (distance <= _detectionRadius)
        {
            Vector2 direction = (_player.transform.position - transform.position).normalized;
            // Shoot a ray to see if a wall or blocker is in the way
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, _obstacleMask);

            // If the ray hits nothing, the enemy can see the player clearly
            if (hit.collider == null)
            {
                hasLineOfSight = true;
            }
        }

        // Only attack or chase if they have a clear Line of Sight
        if (distance <= _attackRadius && _attackTimer <= 0 && hasLineOfSight)
        {
            _currentState = State.Attack;
        }
        else if (distance <= _detectionRadius && hasLineOfSight)
        {
            _currentState = State.Chase;
        }
        else if (_currentState != State.Wander)
        {
            // Go back to wandering if player hides behind a wall/blocker
            _currentState = State.Wander;
            _changeDirectionTimer = 0;
        }
    }

    private void PerformAttack()
    {
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;

        ShootBullet();

        _attackTimer = _attackCooldown;
        _currentState = State.Chase; // Immediately go back to chasing
    }

    public void ShootBullet()
    {
        if (_player == null || _bulletPrefab == null || _firePoint == null) return;

        GameObject bullet = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.identity);
        Vector2 direction = (_player.transform.position - _firePoint.position).normalized;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = direction * _bulletSpeed;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    private void FlipSprite()
    {
        if (_agent.velocity.x > 0.1f) _spriteRenderer.flipX = false;
        else if (_agent.velocity.x < -0.1f) _spriteRenderer.flipX = true;
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

    private void ChasePlayer()
    {
        if (_player == null) return;
        ForceMoveToDestination(_player.transform.position);
    }

    private void SetNewRandomPosition()
    {
        Vector3 randomPos = transform.position + new Vector3(
            Random.Range(-_randomMoveDistance, _randomMoveDistance),
            Random.Range(-_randomMoveDistance, _randomMoveDistance),
            0
        );
        ForceMoveToDestination(randomPos);
    }

    private void ForceMoveToDestination(Vector3 target)
    {
        if (_agent.isOnNavMesh)
        {
            _agent.isStopped = false;
            _agent.SetDestination(new Vector3(target.x, target.y, 0f));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRadius);
    }
}