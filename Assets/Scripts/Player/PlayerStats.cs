using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 玩家状态系统 - 管理生命值、饥饿值、口渴值、体力值
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [Header("生命值")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;
    [SerializeField] private float healthRegenRate = 1f;

    [Header("饥饿值")]
    [SerializeField] private float maxHunger = 100f;
    [SerializeField] private float currentHunger;
    [SerializeField] private float hungerDecayRate = 2f;
    [SerializeField] private float hungerDamageRate = 5f;

    [Header("口渴值")]
    [SerializeField] private float maxThirst = 100f;
    [SerializeField] private float currentThirst;
    [SerializeField] private float thirstDecayRate = 3f;
    [SerializeField] private float thirstDamageRate = 5f;

    [Header("体力值")]
    [SerializeField] private float maxStamina = 100f;
    [SerializeField] private float currentStamina;
    [SerializeField] private float staminaRegenRate = 10f;
    [SerializeField] private float staminaRegenDelay = 1f;

    [Header("温度")]
    [SerializeField] private float normalTemperature = 37f;
    [SerializeField] private float currentTemperature;
    [SerializeField] private float hypothermiaThreshold = 35f;
    [SerializeField] private float hyperthermiaThreshold = 39f;

    // 事件
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent<float> OnHungerChanged;
    public UnityEvent<float> OnThirstChanged;
    public UnityEvent<float> OnStaminaChanged;
    public UnityEvent<float> OnTemperatureChanged;
    public UnityEvent OnPlayerDied;

    // 状态
    private float staminaRegenTimer;
    private bool isDead;

    private void Awake()
    {
        // 初始化状态
        currentHealth = maxHealth;
        currentHunger = maxHunger;
        currentThirst = maxThirst;
        currentStamina = maxStamina;
        currentTemperature = normalTemperature;
    }

    private void Update()
    {
        if (isDead) return;

        UpdateHunger();
        UpdateThirst();
        UpdateStamina();
        UpdateTemperature();
        UpdateHealth();
    }

    /// <summary>
    /// 更新饥饿值
    /// </summary>
    private void UpdateHunger()
    {
        currentHunger -= hungerDecayRate * Time.deltaTime;
        currentHunger = Mathf.Clamp(currentHunger, 0f, maxHunger);

        OnHungerChanged?.Invoke(currentHunger);

        // 饥饿值为0时开始扣血
        if (currentHunger <= 0f)
        {
            TakeDamage(hungerDamageRate * Time.deltaTime);
        }
    }

    /// <summary>
    /// 更新口渴值
    /// </summary>
    private void UpdateThirst()
    {
        currentThirst -= thirstDecayRate * Time.deltaTime;
        currentThirst = Mathf.Clamp(currentThirst, 0f, maxThirst);

        OnThirstChanged?.Invoke(currentThirst);

        // 口渴值为0时开始扣血
        if (currentThirst <= 0f)
        {
            TakeDamage(thirstDamageRate * Time.deltaTime);
        }
    }

    /// <summary>
    /// 更新体力值
    /// </summary>
    private void UpdateStamina()
    {
        // 检查是否可以恢复体力
        if (staminaRegenTimer > 0f)
        {
            staminaRegenTimer -= Time.deltaTime;
        }
        else if (currentStamina < maxStamina)
        {
            currentStamina += staminaRegenRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        }

        OnStaminaChanged?.Invoke(currentStamina);
    }

    /// <summary>
    /// 更新温度
    /// </summary>
    private void UpdateTemperature()
    {
        // 温度会根据环境变化
        // 这里简化处理，实际应该根据天气、时间、装备等计算
        currentTemperature = normalTemperature;

        OnTemperatureChanged?.Invoke(currentTemperature);

        // 低温伤害
        if (currentTemperature < hypothermiaThreshold)
        {
            TakeDamage(2f * Time.deltaTime);
        }
        // 高温伤害
        else if (currentTemperature > hyperthermiaThreshold)
        {
            TakeDamage(2f * Time.deltaTime);
        }
    }

    /// <summary>
    /// 更新生命值
    /// </summary>
    private void UpdateHealth()
    {
        // 生命值恢复（需要饥饿值和口渴值都大于50%）
        if (currentHunger > 50f && currentThirst > 50f && currentHealth < maxHealth)
        {
            currentHealth += healthRegenRate * Time.deltaTime;
            currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
            OnHealthChanged?.Invoke(currentHealth);
        }
    }

    /// <summary>
    /// 受到伤害
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// 恢复生命值
    /// </summary>
    public void Heal(float amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    /// <summary>
    /// 恢复饥饿值
    /// </summary>
    public void RestoreHunger(float amount)
    {
        currentHunger += amount;
        currentHunger = Mathf.Clamp(currentHunger, 0f, maxHunger);
        OnHungerChanged?.Invoke(currentHunger);
    }

    /// <summary>
    /// 恢复口渴值
    /// </summary>
    public void RestoreThirst(float amount)
    {
        currentThirst += amount;
        currentThirst = Mathf.Clamp(currentThirst, 0f, maxThirst);
        OnThirstChanged?.Invoke(currentThirst);
    }

    /// <summary>
    /// 消耗体力
    /// </summary>
    public void ConsumeStamina(float amount)
    {
        currentStamina -= amount;
        currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
        staminaRegenTimer = staminaRegenDelay;
        OnStaminaChanged?.Invoke(currentStamina);
    }

    /// <summary>
    /// 死亡
    /// </summary>
    private void Die()
    {
        isDead = true;
        OnPlayerDied?.Invoke();
        Debug.Log("玩家死亡！");
    }

    /// <summary>
    /// 复活
    /// </summary>
    public void Revive()
    {
        isDead = false;
        currentHealth = maxHealth * 0.5f;
        currentHunger = maxHunger * 0.5f;
        currentThirst = maxThirst * 0.5f;
        currentStamina = maxStamina;
        OnHealthChanged?.Invoke(currentHealth);
        OnHungerChanged?.Invoke(currentHunger);
        OnThirstChanged?.Invoke(currentThirst);
        OnStaminaChanged?.Invoke(currentStamina);
    }

    // 属性访问器
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public float CurrentHunger => currentHunger;
    public float MaxHunger => maxHunger;
    public float CurrentThirst => currentThirst;
    public float MaxThirst => maxThirst;
    public float CurrentStamina => currentStamina;
    public float MaxStamina => maxStamina;
    public float CurrentTemperature => currentTemperature;
    public bool IsDead => isDead;
}