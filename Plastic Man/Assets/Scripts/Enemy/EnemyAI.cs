using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _movementSpeed = 2f;
    [SerializeField] private float _detectionRadius = 5f;
    [SerializeField] private float _attackRadius = 1.5f;
    [SerializeField] private float _attackCooldown = 2f;
    [SerializeField] private float _changeDirectionTime = 2f;
    [SerializeField] private float _randomMoveDistance = 3f;
    [SerializeField] private GameObject _player;

    private NavMeshAgent _agent;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private float _changeDirectionTimer;
    private float _attackTimer;
    private string _currentTrigger;
    private bool _isAttacking;

    private enum State { Wander, Chase, Attack }
    private State _currentState;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _agent.speed = _movementSpeed;
        _agent.updateRotation = false;
        _agent.updateUpAxis = false;

        _currentState = State.Wander;
        SetNewRandomPosition();
    }

    void Update()
    {
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

    private void DetectPlayer()
    {
        if (_player == null) return;
        float distance = Vector2.Distance(transform.position, _player.transform.position);

        if (distance < _attackRadius && _attackTimer <= 0)
        {
            _currentState = State.Attack;
        }
        else if (distance < _detectionRadius)
        {
            _currentState = State.Chase;
        }
        else if (_currentState != State.Wander)
        {
            _currentState = State.Wander;
            SetNewRandomPosition();
        }
    }

    private void PerformAttack()
    {
        _isAttacking = true;
        _attackTimer = _attackCooldown;

        _agent.isStopped = true;
        _agent.velocity = Vector3.zero;

        _animator.SetTrigger("AttackSlime");
    }

    public void OnAttackAnimationFinished()
    {
        _isAttacking = false;
        _agent.isStopped = false;
        _currentTrigger = "";

        if (_player != null)
        {
            _currentState = State.Chase;
            ForceMoveToDestination(_player.transform.position);
        }
    }

    private void UpdateAnimation()
    {
        if (_isAttacking) return;

        float speed = _agent.velocity.magnitude;

        if (speed > 0.1f)
            SafeSetTrigger("MoveSlime");
        else
            SafeSetTrigger("IdleSlime");
    }

    private void FlipSprite()
    {
        if (_agent.velocity.x > 0.1f) _spriteRenderer.flipX = false;
        else if (_agent.velocity.x < -0.1f) _spriteRenderer.flipX = true;
    }

    private void SafeSetTrigger(string triggerName)
    {
        if (_currentTrigger == triggerName) return;

        _animator.ResetTrigger("IdleSlime");
        _animator.ResetTrigger("MoveSlime");

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
        _agent.isStopped = false;
        _agent.SetDestination(target);
    }
}