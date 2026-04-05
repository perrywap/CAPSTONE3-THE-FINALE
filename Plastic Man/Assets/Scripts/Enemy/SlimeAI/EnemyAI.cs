using UnityEngine;
using UnityEngine.AI;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class EnemyAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _movementSpeed = 2.5f;
    [SerializeField] private float _detectionRadius = 8f;
    [SerializeField] private float _attackRadius = 1.8f;
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private float _changeDirectionTime = 3f;
    [SerializeField] private float _randomMoveDistance = 4f;
    [SerializeField] private GameObject _player;
    [SerializeField] private LayerMask _obstacleMask;

    [Header("Lunge Settings")]
    [SerializeField] private float _lungeForce = 12f;
    [SerializeField] private float _lungeDuration = 0.25f;

    [Header("Combat")]
    [SerializeField] private GameObject _hitbox;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip wanderClip;
    [SerializeField] private AudioClip chaseClip;
    [SerializeField] private AudioClip attackClip;
    [SerializeField] private AudioClip deathClip;

    private NavMeshAgent _agent;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private AudioSource _audioSource;

    private float _changeDirectionTimer;
    private float _attackTimer;
    private string _currentTrigger;
    private bool _isAttacking;
    private bool _isDead;
    private State _currentState;

    private enum State { Wander, Chase, Attack }

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _audioSource = GetComponent<AudioSource>();

        _agent.speed = _movementSpeed;
        _agent.stoppingDistance = 1.2f;
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        if (_player == null)
            _player = GameObject.FindGameObjectWithTag("Player");

        _currentState = State.Wander;
        SetNewRandomPosition();

        if (_hitbox != null)
            _hitbox.SetActive(false);

        PlayStateAudio(_currentState);
    }

    void Update()
    {
        if (_isDead) return;

        transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

        if (_attackTimer > 0) _attackTimer -= Time.deltaTime;
        if (_isAttacking) return;

        DetectPlayer();

        switch (_currentState)
        {
            case State.Wander: Wander(); break;
            case State.Chase: ChasePlayer(); break;
            case State.Attack: PerformAttack(); break;
        }

        UpdateAnimation();
        FlipSprite();
    }

    public void Die()
    {
        if (_isDead) return;
        _isDead = true;

        if (_agent.isOnNavMesh)
        {
            _agent.isStopped = true;
            _agent.velocity = Vector3.zero;
        }

        SafeSetTrigger("SlimeDeath");

        if (deathClip != null)
            _audioSource.PlayOneShot(deathClip);

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;
    }

    public void DestroyAfterDeath()
    {
        LootSpawner lootSpawner = Object.FindFirstObjectByType<LootSpawner>();
        if (lootSpawner != null)
        {
            lootSpawner.DropLoot(transform.position);
        }

        EnemySpawner spawner = Object.FindFirstObjectByType<EnemySpawner>();
        if (spawner != null)
        {
            spawner.RemoveEnemyFromList(gameObject);
        }

        Destroy(gameObject);
    }

    private void DetectPlayer()
    {
        if (_player == null) return;

        float distance = Vector2.Distance(transform.position, _player.transform.position);
        State previousState = _currentState;

        bool hasLineOfSight = false;
        if (distance <= _detectionRadius)
        {
            Vector2 direction = (_player.transform.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, _obstacleMask);

            if (hit.collider == null)
                hasLineOfSight = true;
        }

        if (distance <= _attackRadius && _attackTimer <= 0 && hasLineOfSight)
            _currentState = State.Attack;
        else if (distance <= _detectionRadius && hasLineOfSight)
            _currentState = State.Chase;
        else
            _currentState = State.Wander;

        if (previousState != _currentState)
            PlayStateAudio(_currentState);
    }

    private void PlayStateAudio(State state)
    {
        if (_audioSource == null || _isDead) return;

        switch (state)
        {
            case State.Wander: _audioSource.clip = wanderClip; break;
            case State.Chase: _audioSource.clip = chaseClip; break;
            case State.Attack: _audioSource.clip = attackClip; break;
        }

        if (_audioSource.clip != null)
        {
            _audioSource.loop = (state != State.Attack);
            if (!_audioSource.isPlaying || _audioSource.clip != attackClip)
                _audioSource.Play();
        }
    }

    private void PerformAttack()
    {
        _isAttacking = true;
        _attackTimer = _attackCooldown;
        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;

        SafeSetTrigger("AttackSlime");

        if (attackClip != null)
            _audioSource.PlayOneShot(attackClip);

        StartCoroutine(LungeRoutine());
    }

    private IEnumerator LungeRoutine()
    {
        if (_player == null) yield break;

        Vector3 lungeDir = (_player.transform.position - transform.position).normalized;
        float elapsed = 0f;

        while (elapsed < _lungeDuration)
        {
            if (_isDead) yield break;
            transform.position += lungeDir * _lungeForce * Time.deltaTime;
            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void EnableHitbox() => _hitbox?.SetActive(true);
    public void DisableHitbox() => _hitbox?.SetActive(false);

    public void OnAttackAnimationFinished()
    {
        if (_isDead) return;

        _isAttacking = false;
        _agent.isStopped = false;
        _currentTrigger = "";
        DisableHitbox();

        if (_player != null)
        {
            _currentState = State.Chase;
            ForceMoveToDestination(_player.transform.position);
            PlayStateAudio(_currentState);
        }
    }

    private void UpdateAnimation()
    {
        if (_isAttacking || _isDead) return;
        float speed = _agent.velocity.magnitude;
        SafeSetTrigger(speed > 0.1f ? "MoveSlime" : "IdleSlime");
    }

    private void FlipSprite()
    {
        if (_isAttacking || _isDead) return;
        if (_agent.velocity.x > 0.1f) _spriteRenderer.flipX = false;
        else if (_agent.velocity.x < -0.1f) _spriteRenderer.flipX = true;
    }

    private void SafeSetTrigger(string triggerName)
    {
        if (_currentTrigger == triggerName) return;

        _animator.ResetTrigger("IdleSlime");
        _animator.ResetTrigger("MoveSlime");
        _animator.ResetTrigger("AttackSlime");
        _animator.ResetTrigger("SlimeDeath");

        _animator.SetTrigger(triggerName);
        _currentTrigger = triggerName;
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
        if (_agent.isOnNavMesh && !_isDead)
        {
            _agent.isStopped = false;
            _agent.SetDestination(new Vector3(target.x, target.y, 0f));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _attackRadius);
    }
}