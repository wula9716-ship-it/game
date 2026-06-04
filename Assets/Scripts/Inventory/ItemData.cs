using UnityEngine;

/// <summary>
/// 物品数据 - 定义物品属性
/// </summary>
[CreateAssetMenu(fileName = "New Item", menuName = "Survival Island/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("基础信息")]
    public string itemName;
    public string itemDescription;
    public Sprite icon;
    public GameObject prefab;

    [Header("物品类型")]
    public ItemType itemType;
    public ItemRarity rarity;

    [Header("堆叠设置")]
    public bool isStackable = true;
    public int maxStackSize = 99;

    [Header("使用效果")]
    public float healthRestore;
    public float hungerRestore;
    public float thirstRestore;
    public float staminaRestore;

    [Header("装备属性")]
    public float armorValue;
    public float damageValue;
    public float speedModifier;

    [Header("特殊效果")]
    public bool isEdible;
    public bool isDrinkable;
    public bool isEquipable;
    public bool isUsable;
    public bool isPlaceable;

    [Header("建造相关")]
    public bool isBuildingMaterial;
    public BuildingCategory buildingCategory;

    [Header("制作相关")]
    public bool isCraftingMaterial;
    public CraftingCategory craftingCategory;
}

/// <summary>
/// 物品类型
/// </summary>
public enum ItemType
{
    Consumable,     // 消耗品
    Equipment,      // 装备
    Material,       // 材料
    Tool,           // 工具
    Weapon,         // 武器
    Building,       // 建筑
    Misc            // 其他
}

/// <summary>
/// 物品稀有度
/// </summary>
public enum ItemRarity
{
    Common,         // 普通
    Uncommon,       // 非凡
    Rare,           // 稀有
    Epic,           // 史诗
    Legendary       // 传说
}

/// <summary>
/// 建筑分类
/// </summary>
public enum BuildingCategory
{
    Foundation,     // 地基
    Wall,           // 墙壁
    Floor,          // 地板
    Roof,           // 屋顶
    Door,           // 门
    Window,         // 窗户
    Furniture,      // 家具
    Decoration      // 装饰
}

/// <summary>
/// 制作分类
/// </summary>
public enum CraftingCategory
{
    Tools,          // 工具
    Weapons,        // 武器
    Armor,          // 护甲
    Food,           // 食物
    Medicine,       // 药品
    Building,       // 建筑
    Misc            // 其他
}