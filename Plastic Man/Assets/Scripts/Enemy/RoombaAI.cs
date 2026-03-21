using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class RoombaAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _movementSpeed = 2f;
    [SerializeField] private float _detectionRadius = 10f;
    [SerializeField] private float _attackRadius = 2f;
    [SerializeField] private float _attackCooldown = 3f;
    [SerializeField] private float _changeDirectionTime = 3f;
    [SerializeField] private float _randomMoveDistance = 5f;
    [SerializeField] private GameObject _player;

    [Header("Ranged Combat")]
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private float _bulletSpeed = 10f;

    [Header("Audio")]
    [SerializeField] private AudioClip wanderLoop;
    [SerializeField] private AudioClip chaseLoop;
    [SerializeField] private AudioClip attackSfx;

    private NavMeshAgent _agent;
    private SpriteRenderer _spriteRenderer;
    private AudioSource _audioSource;

    private float _changeDirectionTimer;
    private float _attackTimer;

    private enum State { Wander, Chase, Attack }
    private State _currentState;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();

        _agent.speed = _movementSpeed;
        _agent.stoppingDistance = _attackRadius - 1f;
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        if (_player == null)
            _player = GameObject.FindGameObjectWithTag("Player");

        _currentState = State.Wander;
        SetNewRandomPosition();

        PlayLoop(wanderLoop);
    }

    void Update()
    {
        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

        if (_attackTimer > 0) _attackTimer -= Time.deltaTime;

        DetectPlayer();

        switch (_currentState)
        {
            case State.Wander:
                Wander();
                PlayLoop(wanderLoop);
                break;

            case State.Chase:
                ChasePlayer();
                PlayLoop(chaseLoop);
                break;

            case State.Attack:
                PerformAttack();
                break;
        }

        FlipSprite();
    }

    private void DetectPlayer()
    {
        if (_player == null) return;

        float distance = Vector2.Distance(transform.position, _player.transform.position);

        if (distance <= _attackRadius && _attackTimer <= 0)
            _currentState = State.Attack;
        else if (distance <= _detectionRadius)
            _currentState = State.Chase;
        else
            _currentState = State.Wander;
    }

    private void PerformAttack()
    {
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;

        if (attackSfx != null)
            _audioSource.PlayOneShot(attackSfx);

        ShootBullet();

        _attackTimer = _attackCooldown;
        _currentState = State.Chase;

        PlayLoop(chaseLoop);
    }

    private void PlayLoop(AudioClip clip)
    {
        if (_audioSource == null || clip == null) return;

        if (_audioSource.clip == clip && _audioSource.isPlaying) return;

        _audioSource.clip = clip;
        _audioSource.loop = true;
        _audioSource.Play();
    }

    public void ShootBullet()
    {
        if (_player == null || _bulletPrefab == null || _firePoint == null) return;

        GameObject bullet = Instantiate(_bulletPrefab, _firePoint.position, Quaternion.identity);
        Vector2 direction = (_player.transform.position - _firePoint.position).normalized;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction * _bulletSpeed;

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
}