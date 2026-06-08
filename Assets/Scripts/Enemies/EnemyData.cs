using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 敌人数据 - 定义敌人属性
/// </summary>
[CreateAssetMenu(fileName = "New Enemy", menuName = "Survival Island/Enemy Data")]
public class EnemyData : ScriptableObject
{
    [Header("基础信息")]
    public string enemyName;
    public string enemyDescription;
    public GameObject enemyPrefab;

    [Header("敌人类型")]
    public EnemyType enemyType;
    public EnemyDifficulty difficulty;

    [Header("生命值")]
    public float maxHealth = 100f;
    public float healthRegenRate = 0f;

    [Header("攻击属性")]
    public float damage = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    public float attackSpeed = 1f;

    [Header("移动属性")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 4f;
    public float fleeSpeed = 5f;

    [Header("感知属性")]
    public float sightRange = 10f;
    public float hearingRange = 15f;
    public float fieldOfView = 120f;

    [Header("巡逻设置")]
    public float patrolWaitTime = 3f;
    public float patrolRadius = 10f;

    [Header("追击设置")]
    public float chaseRange = 15f;
    public float chaseTimeout = 5f;

    [Header("逃跑设置")]
    public float fleeHealthThreshold = 20f;
    public float fleeDistance = 20f;

    [Header("掉落物品")]
    public List<LootItem> lootTable = new List<LootItem>();

    [Header("行为模式")]
    public List<EnemyBehavior> behaviors = new List<EnemyBehavior>();

    [Header("群体行为")]
    public bool isGroupEnemy = false;
    public int groupSize = 1;
    public float groupRadius = 5f;
}

/// <summary>
/// 敌人类型
/// </summary>
public enum EnemyType
{
    Wildlife,       // 野生动物
    Supernatural,   // 超自然生物
    Alien           // 外星生物
}

/// <summary>
/// 敌人难度
/// </summary>
public enum EnemyDifficulty
{
    Easy,           // 简单
    Medium,         // 中等
    Hard,           // 困难
    Boss            // BOSS
}

/// <summary>
/// 掉落物品
/// </summary>
[System.Serializable]
public class LootItem
{
    public ItemData item;
    public int minAmount = 1;
    public int maxAmount = 1;
    public float dropChance = 1f;
}

/// <summary>
/// 敌人行为
/// </summary>
[System.Serializable]
public class EnemyBehavior
{
    public string behaviorName;
    public BehaviorType type;
    public float triggerChance = 1f;
    public float cooldown = 1f;
}

/// <summary>
/// 行为类型
/// </summary>
public enum BehaviorType
{
    Attack,         // 攻击
    Flee,           // 逃跑
    CallForHelp,    // 呼叫帮助
    Heal,           // 治疗
    Buff,           // 增益
    Debuff          // 减益
}