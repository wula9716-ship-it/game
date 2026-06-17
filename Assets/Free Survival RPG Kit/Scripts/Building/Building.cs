using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 建筑组件 - 管理建筑状态和交互
/// </summary>
public class Building : MonoBehaviour, IInteractable
{
    [Header("建筑数据")]
    [SerializeField] private BuildingData buildingData;

    [Header("建筑状态")]
    [SerializeField] private float currentHealth;
    [SerializeField] private float currentDurability;
    [SerializeField] private bool isDestroyed;

    [Header("环境影响")]
    [SerializeField] private float weatherDamageRate = 0.1f;
    [SerializeField] private float decayRate = 0.05f;

    // 事件
    public UnityEvent<float> OnHealthChanged;
    public UnityEvent<float> OnDurabilityChanged;
    public UnityEvent OnDestroyed;
    public UnityEvent OnRepaired;

    // 组件引用
    private Collider buildingCollider;
    private Renderer buildingRenderer;

    // 状态
    private float weatherDamageTimer;
    private float decayTimer;

    private void Awake()
    {
        buildingCollider = GetComponent<Collider>();
        buildingRenderer = GetComponentInChildren<Renderer>();
    }

    private void Update()
    {
        if (isDestroyed) return;

        UpdateWeatherDamage();
        UpdateDecay();
    }

    /// <summary>
    /// 初始化建筑
    /// </summary>
    public void Initialize(BuildingData data)
    {
        buildingData = data;
        currentHealth = data.maxHealth;
        currentDurability = data.durability;
        isDestroyed = false;

        OnHealthChanged?.Invoke(currentHealth);
        OnDurabilityChanged?.Invoke(currentDurability);
    }

    /// <summary>
    /// 更新天气伤害
    /// </summary>
    private void UpdateWeatherDamage()
    {
        if (!buildingData.affectedByWeather) return;

        weatherDamageTimer += Time.deltaTime;

        // 每10秒检查一次天气
        if (weatherDamageTimer >= 10f)
        {
            weatherDamageTimer = 0f;

            // 获取天气系统
            WeatherSystem weatherSystem = FindFirstObjectByType<WeatherSystem>();
            if (weatherSystem != null)
            {
                float damage = weatherSystem.GetWeatherDamage() * weatherDamageRate;
                TakeDamage(damage);
            }
        }
    }

    /// <summary>
    /// 更新耐久度衰减
    /// </summary>
    private void UpdateDecay()
    {
        decayTimer += Time.deltaTime;

        // 每60秒衰减一次
        if (decayTimer >= 60f)
        {
            decayTimer = 0f;

            currentDurability -= decayRate;
            currentDurability = Mathf.Max(0f, currentDurability);
            OnDurabilityChanged?.Invoke(currentDurability);

            // 耐久度为0时建筑损坏
            if (currentDurability <= 0f)
            {
                DestroyBuilding();
            }
        }
    }

    /// <summary>
    /// 受到伤害
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (isDestroyed) return;
        if (!buildingData.canBeDamaged) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0f, currentHealth);
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth <= 0f)
        {
            DestroyBuilding();
        }
    }

    /// <summary>
    /// 修复建筑
    /// </summary>
    public void Repair(float amount)
    {
        if (isDestroyed) return;
        if (!buildingData.canBeRepaired) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, buildingData.maxHealth);
        OnHealthChanged?.Invoke(currentHealth);

        currentDurability += amount * 0.5f;
        currentDurability = Mathf.Min(currentDurability, buildingData.durability);
        OnDurabilityChanged?.Invoke(currentDurability);

        OnRepaired?.Invoke();
    }

    /// <summary>
    /// 摧毁建筑
    /// </summary>
    public void DestroyBuilding()
    {
        if (isDestroyed) return;

        isDestroyed = true;
        OnDestroyed?.Invoke();

        // 禁用碰撞器
        if (buildingCollider != null)
        {
            buildingCollider.enabled = false;
        }

        // 改变材质表示损坏
        if (buildingRenderer != null)
        {
            // 可以在这里应用损坏材质
        }

        // 延迟销毁
        Destroy(gameObject, 5f);
    }

    /// <summary>
    /// 交互
    /// </summary>
    public void Interact(PlayerController player)
    {
        if (isDestroyed) return;

        // 根据建筑类型执行不同交互
        switch (buildingData.buildingType)
        {
            case BuildingType.Storage:
                // 打开存储界面
                OpenStorageUI();
                break;
            case BuildingType.Workstation:
                // 打开制作界面
                OpenCraftingUI();
                break;
            case BuildingType.Door:
                // 开关门
                ToggleDoor();
                break;
            default:
                // 显示建筑信息
                ShowBuildingInfo();
                break;
        }
    }

    /// <summary>
    /// 打开存储界面
    /// </summary>
    private void OpenStorageUI()
    {
        Debug.Log("打开存储界面");
        // TODO: 实现存储界面
    }

    /// <summary>
    /// 打开制作界面
    /// </summary>
    private void OpenCraftingUI()
    {
        Debug.Log("打开制作界面");
        // TODO: 实现制作界面
    }

    /// <summary>
    /// 开关门
    /// </summary>
    private void ToggleDoor()
    {
        Debug.Log("开关门");
        // TODO: 实现门动画
    }

    /// <summary>
    /// 显示建筑信息
    /// </summary>
    private void ShowBuildingInfo()
    {
        Debug.Log($"建筑: {buildingData.buildingName}\n" +
                  $"生命值: {currentHealth}/{buildingData.maxHealth}\n" +
                  $"耐久度: {currentDurability}/{buildingData.durability}");
    }

    /// <summary>
    /// 获取建筑数据
    /// </summary>
    public BuildingData BuildingData => buildingData;

    /// <summary>
    /// 获取当前生命值
    /// </summary>
    public float CurrentHealth => currentHealth;

    /// <summary>
    /// 获取当前耐久度
    /// </summary>
    public float CurrentDurability => currentDurability;

    /// <summary>
    /// 是否已损坏
    /// </summary>
    public bool IsDestroyed => isDestroyed;
}