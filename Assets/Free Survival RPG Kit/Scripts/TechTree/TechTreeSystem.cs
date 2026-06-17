using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 科技树系统 - 管理科技解锁和研究
/// </summary>
public class TechTreeSystem : MonoBehaviour
{
    [Header("科技树数据")]
    [SerializeField] private TechTreeDatabase techDatabase;

    [Header("研究设置")]
    [SerializeField] private float baseResearchSpeed = 1f;
    [SerializeField] private float researchSpeedMultiplier = 1f;

    // 事件
    public UnityEvent<TechData> OnTechUnlocked;
    public UnityEvent<TechData, float> OnResearchProgress;
    public UnityEvent<TechData> OnResearchCompleted;

    // 状态
    private Dictionary<string, TechData> unlockedTechs = new Dictionary<string, TechData>();
    private TechData currentResearch;
    private float researchProgress;
    private bool isResearching;

    // 单例
    public static TechTreeSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        if (isResearching && currentResearch != null)
        {
            UpdateResearch();
        }
    }

    /// <summary>
    /// 更新研究进度
    /// </summary>
    private void UpdateResearch()
    {
        // 检查研究站
        if (!HasResearchStation())
        {
            PauseResearch();
            return;
        }

        // 更新进度
        float researchSpeed = baseResearchSpeed * researchSpeedMultiplier;
        researchProgress += researchSpeed * Time.deltaTime;

        // 触发进度事件
        OnResearchProgress?.Invoke(currentResearch, researchProgress / currentResearch.researchCost);

        // 检查是否完成
        if (researchProgress >= currentResearch.researchCost)
        {
            CompleteResearch();
        }
    }

    /// <summary>
    /// 开始研究
    /// </summary>
    public bool StartResearch(TechData tech)
    {
        if (tech == null || IsTechUnlocked(tech))
            return false;

        // 检查前置科技
        if (!HasPrerequisites(tech))
            return false;

        // 检查资源
        if (!HasRequiredResources(tech))
            return false;

        // 开始研究
        currentResearch = tech;
        researchProgress = 0;
        isResearching = true;

        // 消耗资源
        ConsumeResources(tech);

        Debug.Log($"开始研究: {tech.techName}");
        return true;
    }

    /// <summary>
    /// 暂停研究
    /// </summary>
    public void PauseResearch()
    {
        isResearching = false;
        Debug.Log("研究暂停");
    }

    /// <summary>
    /// 恢复研究
    /// </summary>
    public void ResumeResearch()
    {
        if (currentResearch != null && HasResearchStation())
        {
            isResearching = true;
            Debug.Log("研究恢复");
        }
    }

    /// <summary>
    /// 取消研究
    /// </summary>
    public void CancelResearch()
    {
        if (currentResearch != null)
        {
            // 返还部分资源
            RefundResources(currentResearch, 0.5f);

            currentResearch = null;
            researchProgress = 0;
            isResearching = false;

            Debug.Log("研究取消");
        }
    }

    /// <summary>
    /// 完成研究
    /// </summary>
    private void CompleteResearch()
    {
        if (currentResearch == null)
            return;

        // 解锁科技
        UnlockTech(currentResearch);

        // 触发完成事件
        OnResearchCompleted?.Invoke(currentResearch);

        Debug.Log($"研究完成: {currentResearch.techName}");

        // 清除当前研究
        currentResearch = null;
        researchProgress = 0;
        isResearching = false;
    }

    /// <summary>
    /// 解锁科技
    /// </summary>
    public void UnlockTech(TechData tech)
    {
        if (tech == null || IsTechUnlocked(tech))
            return;

        unlockedTechs.Add(tech.techName, tech);
        OnTechUnlocked?.Invoke(tech);

        // 应用科技效果
        ApplyTechEffects(tech);
    }

    /// <summary>
    /// 检查科技是否已解锁
    /// </summary>
    public bool IsTechUnlocked(TechData tech)
    {
        if (tech == null) return false;
        return unlockedTechs.ContainsKey(tech.techName);
    }

    /// <summary>
    /// 检查科技是否已解锁（按名称）
    /// </summary>
    public bool IsTechUnlocked(string techName)
    {
        return unlockedTechs.ContainsKey(techName);
    }

    /// <summary>
    /// 检查是否有前置科技
    /// </summary>
    public bool HasPrerequisites(TechData tech)
    {
        if (tech == null) return false;

        foreach (var prereq in tech.prerequisites)
        {
            if (!IsTechUnlocked(prereq))
                return false;
        }

        return true;
    }

    /// <summary>
    /// 检查是否有足够资源
    /// </summary>
    public bool HasRequiredResources(TechData tech)
    {
        if (tech == null) return false;

        if (Inventory.instance == null) return false;

        foreach (var requirement in tech.resourceRequirements)
        {
            if (Inventory.instance.CountItem(requirement.item) < requirement.amount)
                return false;
        }

        return true;
    }

    /// <summary>
    /// 消耗资源
    /// </summary>
    private void ConsumeResources(TechData tech)
    {
        if (Inventory.instance == null) return;

        foreach (var requirement in tech.resourceRequirements)
        {
            Inventory.instance.RemoveItem(requirement.item, requirement.amount);
        }
    }

    /// <summary>
    /// 返还资源
    /// </summary>
    private void RefundResources(TechData tech, float refundRate)
    {
        if (Inventory.instance == null) return;

        foreach (var requirement in tech.resourceRequirements)
        {
            int refundAmount = Mathf.CeilToInt(requirement.amount * refundRate);
            Inventory.instance.AddItem(requirement.item, refundAmount);
        }
    }

    /// <summary>
    /// 检查是否有研究站
    /// </summary>
    private bool HasResearchStation()
    {
        // TODO: 检查玩家附近是否有研究站
        return true;
    }

    /// <summary>
    /// 应用科技效果
    /// </summary>
    private void ApplyTechEffects(TechData tech)
    {
        if (tech == null) return;

        foreach (var effect in tech.effects)
        {
            ApplyEffect(effect);
        }
    }

    /// <summary>
    /// 应用效果
    /// </summary>
    private void ApplyEffect(TechEffect effect)
    {
        switch (effect.type)
        {
            case TechEffectType.UnlockRecipe:
                // 解锁配方
                Debug.Log($"解锁配方: {effect.stringValue}");
                break;
            case TechEffectType.UnlockBuilding:
                // 解锁建筑
                Debug.Log($"解锁建筑: {effect.stringValue}");
                break;
            case TechEffectType.StatBonus:
                // 属性加成
                Debug.Log($"属性加成: {effect.stringValue} +{effect.floatValue}");
                break;
            case TechEffectType.NewAbility:
                // 新能力
                Debug.Log($"新能力: {effect.stringValue}");
                break;
        }
    }

    /// <summary>
    /// 获取已解锁科技列表
    /// </summary>
    public List<TechData> GetUnlockedTechs()
    {
        return unlockedTechs.Values.ToList();
    }

    /// <summary>
    /// 获取当前研究
    /// </summary>
    public TechData GetCurrentResearch()
    {
        return currentResearch;
    }

    /// <summary>
    /// 获取研究进度
    /// </summary>
    public float GetResearchProgress()
    {
        if (currentResearch == null) return 0;
        return researchProgress / currentResearch.researchCost;
    }

    /// <summary>
    /// 是否正在研究
    /// </summary>
    public bool IsResearching => isResearching;
}

/// <summary>
/// 科技数据
/// </summary>
[System.Serializable]
public class TechData
{
    [Header("基础信息")]
    public string techName;
    public string techDescription;
    public Sprite icon;
    public TechCategory category;
    public int tier;

    [Header("研究要求")]
    public float researchCost = 100f;
    public List<TechData> prerequisites = new List<TechData>();
    public List<ResourceRequirement> resourceRequirements = new List<ResourceRequirement>();

    [Header("科技效果")]
    public List<TechEffect> effects = new List<TechEffect>();

    [Header("解锁内容")]
    public List<Item> unlockedItems = new List<Item>();
    public List<BuildingData> unlockedBuildings = new List<BuildingData>();
    // public List<CraftingRecipe> unlockedRecipes = new List<CraftingRecipe>(); // TODO: integrate with template crafting
}

/// <summary>
/// 科技分类
/// </summary>
public enum TechCategory
{
    Survival,       // 生存
    Building,       // 建造
    Exploration,    // 探索
    Combat,         // 战斗
    Alien           // 外星科技
}

/// <summary>
/// 科技效果
/// </summary>
[System.Serializable]
public class TechEffect
{
    public TechEffectType type;
    public string stringValue;
    public float floatValue;
    public int intValue;
}

/// <summary>
/// 科技效果类型
/// </summary>
public enum TechEffectType
{
    UnlockRecipe,       // 解锁配方
    UnlockBuilding,     // 解锁建筑
    StatBonus,          // 属性加成
    NewAbility,         // 新能力
    ResourceBonus,      // 资源加成
    CraftingSpeed,      // 制作速度
    BuildingSpeed,      // 建造速度
    ResearchSpeed       // 研究速度
}