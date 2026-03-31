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

    [Header("Ranged Combat")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _bulletSpeed = 10f;

    private NavMeshAgent _agent;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private float _changeDirectionTimer;
    private float _attackTimer;
    private bool _isAttacking;
    private float _attackSafetyTimer;

    private enum State { Wander, Chase, Attack }
    private State _currentState;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();

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

        if (_isAttacking)
        {
            _attackSafetyTimer += Time.deltaTime;
            if (_attackSafetyTimer > 2.5f) OnAttackFinished();

            _animator.speed = 1;
            return;
        }

        DetectPlayer();

        switch (_currentState)
        {
            case State.Wander: Wander(); break;
            case State.Chase: ChasePlayer(); break;
            case State.Attack: PerformAttack(); break;
        }

        UpdateAnimations();
    }

    private void DetectPlayer()
    {
        if (_player == null) return;

        float distance = Vector2.Distance(transform.position, _player.transform.position);

        if (distance <= _attackRadius && _attackTimer <= 0)
        {
            _currentState = State.Attack;
        }
        else if (distance <= _detectionRadius)
        {
            _currentState = State.Chase;
        }
        else if (_currentState != State.Wander)
        {
            _currentState = State.Wander;
            _changeDirectionTimer = 0;
        }
    }

    private void PerformAttack()
    {
        if (_player == null) return;

        _isAttacking = true;
        _attackSafetyTimer = 0f;
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;

        Vector2 dir = ((Vector2)_player.transform.position - (Vector2)transform.position).normalized;

        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            _animator.Play("RoombaAttackSide", 0, 0);
            _spriteRenderer.flipX = dir.x < 0;
        }
        else
        {
            _animator.Play(dir.y > 0 ? "RoombaBackAttack" : "RoombaAttackFront", 0, 0);
        }
    }

    public void ShootBullet()
    {
        if (_player == null || _bulletPrefab == null || _firePoint == null) return;

        GameObject bullet = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.identity);
        Vector2 direction = ((Vector2)_player.transform.position - (Vector2)_firePoint.position).normalized;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = direction * _bulletSpeed;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    public void OnAttackFinished()
    {
        _isAttacking = false;
        _agent.isStopped = false;
        _attackTimer = _attackCooldown;
        _currentState = State.Chase;
    }

    private void UpdateAnimations()
    {
        if (_isAttacking) return;

        Vector2 velocity = _agent.velocity;

        if (velocity.magnitude < 0.1f)
        {
            _animator.speed = 0;
            return;
        }

        _animator.speed = 1;

        if (Mathf.Abs(velocity.y) > Mathf.Abs(velocity.x))
        {
            _animator.Play(velocity.y > 0 ? "RoombaMoveUp" : "RoombaMoveDown");
        }
        else
        {
            _animator.Play("RoombaMoveSide");
            _spriteRenderer.flipX = velocity.x < 0;
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