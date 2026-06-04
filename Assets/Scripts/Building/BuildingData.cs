using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 建筑数据 - 定义建筑属性
/// </summary>
[CreateAssetMenu(fileName = "New Building", menuName = "Survival Island/Building Data")]
public class BuildingData : ScriptableObject
{
    [Header("基础信息")]
    public string buildingName;
    public string buildingDescription;
    public Sprite icon;
    public GameObject buildingPrefab;
    public GameObject previewPrefab;

    [Header("建筑类型")]
    public BuildingType buildingType;
    public BuildingCategory buildingCategory;

    [Header("放置设置")]
    public bool canPlaceOnTerrain = true;
    public bool canPlaceOnBuildings = false;
    public bool alignToSurface = false;
    public bool canStack = false;
    public Vector3 placementOffset;
    public Vector3 placementSize = Vector3.one;

    [Header("资源需求")]
    public List<ResourceRequirement> requiredResources = new List<ResourceRequirement>();

    [Header("建筑属性")]
    public float maxHealth = 100f;
    public float buildTime = 1f;
    public float durability = 100f;

    [Header("功能设置")]
    public bool providesShelter = false;
    public bool providesStorage = false;
    public int storageSlots = 0;
    public bool providesCrafting = false;
    public CraftingStationType craftingStationType;

    [Header("防御设置")]
    public float defenseValue = 0f;
    public bool isObstacle = false;

    [Header("环境互动")]
    public bool affectedByWeather = true;
    public bool canBeDamaged = true;
    public bool canBeRepaired = true;

    [Header("解锁条件")]
    public int requiredLevel = 0;
    public List<TechTreeData> requiredTech = new List<TechTreeData>();
}

/// <summary>
/// 建筑类型
/// </summary>
public enum BuildingType
{
    Foundation,     // 地基
    Wall,           // 墙壁
    Floor,          // 地板
    Roof,           // 屋顶
    Door,           // 门
    Window,         // 窗户
    Stairs,         // 楼梯
    Furniture,      // 家具
    Workstation,    // 工作站
    Storage,        // 存储
    Defense,        // 防御
    Decoration      // 装饰
}

/// <summary>
/// 工作站类型
/// </summary>
public enum CraftingStationType
{
    None,
    Workbench,      // 工作台
    Furnace,        // 熔炉
    Loom,           // 织布机
    ResearchTable,  // 研究台
    CookingStation, // 烹饪站
    AlchemyStation  // 炼金站
}

/// <summary>
/// 资源需求
/// </summary>
[System.Serializable]
public class ResourceRequirement
{
    public ItemData item;
    public int amount;
}

/// <summary>
/// 科技树数据（占位）
/// </summary>
[System.Serializable]
public class TechTreeData
{
    public string techName;
    public int techLevel;
}