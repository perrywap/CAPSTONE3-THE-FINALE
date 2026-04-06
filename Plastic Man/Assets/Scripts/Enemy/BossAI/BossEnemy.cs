using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class BossEnemy : MonoBehaviour
{
    [Header("Health & Phases")]
    public float maxHealth = 500f;
    public float currentHealth;
    public float phaseTwoThreshold = 0.5f;
    public float phaseTwoCooldownMultiplier = 0.6f;
    private bool isPhaseTwo = false;

    [Header("Audio")]
    [SerializeField] private AudioClip damageSfx;
    [SerializeField] private AudioClip deathSfx;
    [SerializeField] private float damageVolume = 0.5f;
    [SerializeField] private float deathVolume = 0.7f;

    [Header("Navigation Settings")]
    public float movementSpeed = 4f;
    public float detectionRange = 20f;
    public float attackRange = 7f;

    [Header("Attack Prefabs & Points")]
    public GameObject bulletPrefab;
    public GameObject grenadePrefab;
    public Transform firePoint;
    public GameObject swordHitbox;

    [Header("Relentless Balancing")]
    public float attackCooldown = 1.5f;
    public float dashSpeed = 18f;
    public float dashDuration = 0.4f;

    [Header("True Ending Visuals")]
    public Sprite deadBossSprite; // <-- NEW: Drag your dead boss artwork here!

    private NavMeshAgent agent;
    private GameObject player;
    private SpriteRenderer spriteRenderer;
    private float nextAttackTime;
    private bool isAttacking = false;
    private List<int> abilityQueue = new List<int>();

    private Animator anim;
    public float dashTime = 0.2f;
    public float stopTime = 0.3f;
    public float dashSpeedMultiplier = 3f;

    private Coroutine chaseRoutine;

    void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();

        agent.speed = movementSpeed;
        agent.acceleration = 60f;
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        if (swordHitbox) swordHitbox.SetActive(false);
        FillAbilityQueue();

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, 1.0f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position);
        }
    }

    void Update()
    {
        if (player == null || isAttacking) return;
        CheckPhaseTransition();

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            StartAttack();
        }
        else if (distanceToPlayer <= detectionRange)
        {
            ChasePlayer();
        }

        FlipSprite();
    }

    void CheckPhaseTransition()
    {
        if (!isPhaseTwo && (currentHealth / maxHealth) <= phaseTwoThreshold)
        {
            isPhaseTwo = true;
            attackCooldown *= phaseTwoCooldownMultiplier;
            spriteRenderer.color = Color.red;
            Debug.Log("<color=red><b>PHASE 2:</b> Boss is faster and relentless!</color>");
        }
    }

    void FillAbilityQueue()
    {
        abilityQueue = new List<int> { 2, 2, 3, 0, 1 };
        for (int i = 0; i < abilityQueue.Count; i++)
        {
            int temp = abilityQueue[i];
            int randomIndex = Random.Range(i, abilityQueue.Count);
            abilityQueue[i] = abilityQueue[randomIndex];
            abilityQueue[randomIndex] = temp;
        }
    }

    void StartAttack()
    {
        isAttacking = true;
        agent.isStopped = true;

        if (abilityQueue.Count == 0) FillAbilityQueue();

        int choice = abilityQueue[0];
        abilityQueue.RemoveAt(0);

        switch (choice)
        {
            case 0: ExecuteShoot(); break;
            case 1: ExecuteGrenade(); break;
            case 2: StartCoroutine(ExecuteDash(false)); break;
            case 3: StartCoroutine(ExecuteDash(true)); break;
        }
        nextAttackTime = Time.time + attackCooldown;
    }

    public void Shoot()
    {
        anim.SetBool("IsShooting", true);
        if (bulletPrefab && firePoint) Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Invoke("ResetAttackState", 0.3f);
    }

    public void ShootDone() { anim.SetBool("IsShooting", false); }
    public void ExecuteShoot() { anim.SetTrigger("Aim"); }

    void ExecuteGrenade()
    {
        if (grenadePrefab == null || firePoint == null || player == null) return;

        Vector3 targetPos = player.transform.position;
        targetPos.z = 0f;

        GameObject grenadeObj = Instantiate(grenadePrefab, firePoint.position, Quaternion.identity);
        GrenadeProjectile grenade = grenadeObj.GetComponent<GrenadeProjectile>();
        if (grenade != null) grenade.SetTarget(targetPos);

        anim.SetBool("IsThrowing", true);
        ResetAttackState();
    }

    public void ExecuteGrenadeDone() { anim.SetBool("IsThrowing", false); }
    public void SwordAttackDone() { anim.SetBool("SwordAttack", false); }

    IEnumerator ExecuteDash(bool includeSlice)
    {
        anim.SetBool("SwordAttack", true);
        if (anim.GetBool("IsShooting") == true) yield return null;

        Vector3 dashDir = (player.transform.position - transform.position).normalized;
        float startTime = Time.time;

        if (includeSlice && swordHitbox) swordHitbox.SetActive(true);

        while (Time.time < startTime + dashDuration)
        {
            transform.position += dashDir * dashSpeed * Time.deltaTime;
            yield return null;
        }

        if (swordHitbox) swordHitbox.SetActive(false);
        ResetAttackState();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        StartCoroutine(HitFlash());

        if (damageSfx != null && SfxManager.instance != null)
            SfxManager.instance.PlaySFX(damageSfx, damageVolume);

        if (currentHealth <= 0) Die();
    }

    IEnumerator HitFlash()
    {
        Color baseColor = isPhaseTwo ? Color.red : Color.white;
        spriteRenderer.color = Color.yellow;
        yield return new WaitForSeconds(0.05f);
        spriteRenderer.color = baseColor;
    }

    void Die()
    {
        Debug.Log("<color=yellow>Boss Defeated!</color>");
        StopAllCoroutines(); // Instantly stops dashing/chasing!

        if (deathSfx != null && SfxManager.instance != null)
            SfxManager.instance.PlaySFX(deathSfx, deathVolume);

        LootSpawner lootSpawner = Object.FindFirstObjectByType<LootSpawner>();
        if (lootSpawner != null) lootSpawner.DropLoot(transform.position);

        EnemySpawner spawner = Object.FindFirstObjectByType<EnemySpawner>();
        if (spawner != null) spawner.RemoveEnemyFromList(gameObject);

        // --- THE TRUE ENDING TRIGGER ---
        if (GameManager.Instance != null && GameManager.Instance.trueEndingSequence != null)
        {
            // 1. Permanently shut down all combat logic
            agent.enabled = false;
            GetComponent<Collider2D>().enabled = false;
            anim.enabled = false;

            // 2. Snap to the dead sprite
            if (deadBossSprite != null) spriteRenderer.sprite = deadBossSprite;
            spriteRenderer.color = Color.white;

            // 3. Command the GameManager to start the cinematic using this exact location
            GameManager.Instance.TriggerTrueEnding(this.transform);

            // 4. Turn off this script permanently
            this.enabled = false;
        }
        else
        {
            // Failsafe for normal levels
            if (GameManager.Instance != null) GameManager.Instance.UnregisterEnemy(gameObject);
            Destroy(gameObject);
        }
    }

    void ChasePlayer()
    {
        if (chaseRoutine == null) chaseRoutine = StartCoroutine(DashChase());
    }

    IEnumerator DashChase()
    {
        anim.SetBool("IsAttacking", true);
        while (player != null)
        {
            agent.isStopped = false;
            agent.speed = movementSpeed * dashSpeedMultiplier;
            agent.SetDestination(player.transform.position);
            yield return new WaitForSeconds(dashTime);

            agent.isStopped = true;
            yield return new WaitForSeconds(stopTime);
        }
        chaseRoutine = null;
    }

    void ResetAttackState() { isAttacking = false; }

    void FlipSprite()
    {
        if (isAttacking) return;
        if (agent.velocity.x > 0.1f) spriteRenderer.flipX = false;
        else if (agent.velocity.x < -0.1f) spriteRenderer.flipX = true;
    }
}