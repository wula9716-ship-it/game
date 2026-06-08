using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

/// <summary>
/// 敌人AI - 控制敌人行为和状态
/// </summary>
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyAI : MonoBehaviour
{
    [Header("敌人属性")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float damage = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1f;

    [Header("感知系统")]
    [SerializeField] private float sightRange = 10f;
    [SerializeField] private float hearingRange = 15f;
    [SerializeField] private float fieldOfView = 120f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("巡逻设置")]
    [SerializeField] private List<Transform> patrolPoints = new List<Transform>();
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float patrolWaitTime = 3f;

    [Header("追击设置")]
    [SerializeField] private float chaseSpeed = 4f;
    [SerializeField] private float chaseRange = 15f;

    [Header("攻击设置")]
    [SerializeField] private float attackDamage = 10f;
    [SerializeField] private float attackForce = 5f;

    [Header("逃跑设置")]
    [SerializeField] private float fleeHealthThreshold = 20f;
    [SerializeField] private float fleeSpeed = 5f;
    [SerializeField] private float fleeDistance = 20f;

    // 组件引用
    private NavMeshAgent agent;
    private Animator animator;
    private Transform player;

    // 状态
    private EnemyState currentState = EnemyState.Patrol;
    private int currentPatrolIndex = 0;
    private float patrolWaitTimer;
    private float attackTimer;
    private float stateTimer;

    // 目标
    private Vector3 lastKnownPlayerPosition;
    private bool playerDetected;
    private bool playerInSight;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
    }

    private void Start()
    {
        // 查找玩家
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }

        // 开始巡逻
        StartPatrol();
    }

    private void Update()
    {
        // 更新计时器
        UpdateTimers();

        // 检测玩家
        DetectPlayer();

        // 执行当前状态
        ExecuteState();
    }

    /// <summary>
    /// 更新计时器
    /// </summary>
    private void UpdateTimers()
    {
        attackTimer -= Time.deltaTime;
        stateTimer += Time.deltaTime;

        if (currentState == EnemyState.Patrol)
        {
            patrolWaitTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// 检测玩家
    /// </summary>
    private void DetectPlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, directionToPlayer);

        // 视觉检测
        playerInSight = false;
        if (distanceToPlayer <= sightRange && angle <= fieldOfView * 0.5f)
        {
            // 检查是否有障碍物
            if (!Physics.Raycast(transform.position + Vector3.up, directionToPlayer, distanceToPlayer, obstacleLayer))
            {
                playerInSight = true;
                lastKnownPlayerPosition = player.position;
            }
        }

        // 听觉检测
        if (distanceToPlayer <= hearingRange)
        {
            // 玩家移动或攻击时会被听到
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null && (playerController.IsRunning || Input.GetMouseButton(0)))
            {
                lastKnownPlayerPosition = player.position;
                playerDetected = true;
            }
        }

        // 更新检测状态
        playerDetected = playerInSight || playerDetected;
    }

    /// <summary>
    /// 执行当前状态
    /// </summary>
    private void ExecuteState()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                ExecutePatrol();
                break;
            case EnemyState.Chase:
                ExecuteChase();
                break;
            case EnemyState.Attack:
                ExecuteAttack();
                break;
            case EnemyState.Flee:
                ExecuteFlee();
                break;
            case EnemyState.Search:
                ExecuteSearch();
                break;
        }
    }

    /// <summary>
    /// 执行巡逻
    /// </summary>
    private void ExecutePatrol()
    {
        // 检查是否需要切换到追击
        if (playerDetected)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        // 检查是否需要逃跑
        if (currentHealth <= fleeHealthThreshold)
        {
            ChangeState(EnemyState.Flee);
            return;
        }

        // 巡逻逻辑
        if (patrolPoints.Count == 0)
        {
            // 没有巡逻点，随机移动
            if (!agent.hasPath || agent.remainingDistance < 0.5f)
            {
                Vector3 randomDirection = Random.insideUnitSphere * 10f;
                randomDirection += transform.position;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomDirection, out hit, 10f, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }
            }
            return;
        }

        // 有巡逻点，按顺序巡逻
        if (agent.remainingDistance < 0.5f)
        {
            if (patrolWaitTimer <= 0)
            {
                // 移动到下一个巡逻点
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Count;
                agent.SetDestination(patrolPoints[currentPatrolIndex].position);
                patrolWaitTimer = patrolWaitTime;
            }
        }

        agent.speed = patrolSpeed;
    }

    /// <summary>
    /// 执行追击
    /// </summary>
    private void ExecuteChase()
    {
        // 检查是否丢失玩家
        if (!playerDetected && stateTimer > 5f)
        {
            ChangeState(EnemyState.Search);
            return;
        }

        // 检查是否可以攻击
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= attackRange)
        {
            ChangeState(EnemyState.Attack);
            return;
        }

        // 检查是否需要逃跑
        if (currentHealth <= fleeHealthThreshold)
        {
            ChangeState(EnemyState.Flee);
            return;
        }

        // 追击玩家
        agent.SetDestination(player.position);
        agent.speed = chaseSpeed;
    }

    /// <summary>
    /// 执行攻击
    /// </summary>
    private void ExecuteAttack()
    {
        // 检查玩家是否在攻击范围外
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > attackRange)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        // 面向玩家
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(directionToPlayer), Time.deltaTime * 5f);

        // 攻击冷却
        if (attackTimer <= 0)
        {
            Attack();
            attackTimer = attackCooldown;
        }

        agent.SetDestination(transform.position);
    }

    /// <summary>
    /// 执行逃跑
    /// </summary>
    private void ExecuteFlee()
    {
        // 检查是否安全
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer > fleeDistance)
        {
            ChangeState(EnemyState.Patrol);
            return;
        }

        // 逃跑方向（远离玩家）
        Vector3 fleeDirection = (transform.position - player.position).normalized;
        Vector3 fleeTarget = transform.position + fleeDirection * fleeDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleeTarget, out hit, fleeDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }

        agent.speed = fleeSpeed;
    }

    /// <summary>
    /// 执行搜索
    /// </summary>
    private void ExecuteSearch()
    {
        // 搜索最后已知位置
        if (agent.remainingDistance < 0.5f)
        {
            if (stateTimer > 10f)
            {
                // 搜索超时，返回巡逻
                ChangeState(EnemyState.Patrol);
                return;
            }

            // 在最后已知位置附近搜索
            Vector3 searchDirection = Random.insideUnitSphere * 5f;
            searchDirection += lastKnownPlayerPosition;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(searchDirection, out hit, 5f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
        }

        // 检查是否发现玩家
        if (playerDetected)
        {
            ChangeState(EnemyState.Chase);
        }

        agent.speed = patrolSpeed;
    }

    /// <summary>
    /// 攻击
    /// </summary>
    private void Attack()
    {
        if (player == null) return;

        // 播放攻击动画
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }

        // 造成伤害
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.TakeDamage(attackDamage);
        }

        // 击退效果
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            Vector3 knockbackDirection = (player.position - transform.position).normalized;
            // 可以在这里添加击退力
        }
    }

    /// <summary>
    /// 受到伤害
    /// </summary>
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        // 播放受伤动画
        if (animator != null)
        {
            animator.SetTrigger("Hit");
        }

        // 检测到玩家
        playerDetected = true;
        lastKnownPlayerPosition = player.position;

        // 检查是否死亡
        if (currentHealth <= 0)
        {
            Die();
        }
        // 检查是否需要逃跑
        else if (currentHealth <= fleeHealthThreshold)
        {
            ChangeState(EnemyState.Flee);
        }
        // 检查是否需要追击
        else if (currentState == EnemyState.Patrol)
        {
            ChangeState(EnemyState.Chase);
        }
    }

    /// <summary>
    /// 死亡
    /// </summary>
    private void Die()
    {
        // 播放死亡动画
        if (animator != null)
        {
            animator.SetTrigger("Die");
        }

        // 禁用AI
        agent.enabled = false;
        this.enabled = false;

        // 掉落物品
        DropLoot();

        // 延迟销毁
        Destroy(gameObject, 5f);
    }

    /// <summary>
    /// 掉落物品
    /// </summary>
    private void DropLoot()
    {
        // TODO: 实现物品掉落
        Debug.Log("掉落物品");
    }

    /// <summary>
    /// 改变状态
    /// </summary>
    private void ChangeState(EnemyState newState)
    {
        currentState = newState;
        stateTimer = 0;

        // 更新动画
        if (animator != null)
        {
            animator.SetInteger("State", (int)newState);
        }
    }

    /// <summary>
    /// 开始巡逻
    /// </summary>
    private void StartPatrol()
    {
        if (patrolPoints.Count > 0)
        {
            agent.SetDestination(patrolPoints[0].position);
        }
        ChangeState(EnemyState.Patrol);
    }

    /// <summary>
    /// 获取当前状态
    /// </summary>
    public EnemyState CurrentState => currentState;

    /// <summary>
    /// 获取当前生命值
    /// </summary>
    public float CurrentHealth => currentHealth;

    /// <summary>
    /// 获取最大生命值
    /// </summary>
    public float MaxHealth => maxHealth;
}

/// <summary>
/// 敌人状态
/// </summary>
public enum EnemyState
{
    Patrol,     // 巡逻
    Chase,      // 追击
    Attack,     // 攻击
    Flee,       // 逃跑
    Search      // 搜索
}