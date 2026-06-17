using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 制作数据库 - 存储所有制作配方
/// </summary>
[CreateAssetMenu(fileName = "CraftingDatabase", menuName = "Survival Island/Crafting Database")]
public class CraftingDatabase : ScriptableObject
{
    [Header("配方列表")]
    [SerializeField] private List<CraftingRecipe> recipes = new List<CraftingRecipe>();

    // 配方字典（用于快速查找）
    private Dictionary<string, CraftingRecipe> recipeDictionary;

    /// <summary>
    /// 初始化数据库
    /// </summary>
    public void Initialize()
    {
        recipeDictionary = new Dictionary<string, CraftingRecipe>();

        foreach (var recipe in recipes)
        {
            if (recipe != null && recipe.resultItem != null)
            {
                string key = recipe.resultItem.name;
                if (!recipeDictionary.ContainsKey(key))
                {
                    recipeDictionary.Add(key, recipe);
                }
            }
        }
    }

    /// <summary>
    /// 根据产物名称获取配方
    /// </summary>
    public CraftingRecipe GetRecipeByResult(string resultName)
    {
        if (recipeDictionary == null)
        {
            Initialize();
        }

        if (recipeDictionary.TryGetValue(resultName, out CraftingRecipe recipe))
        {
            return recipe;
        }

        return null;
    }

    /// <summary>
    /// 根据分类获取配方列表
    /// </summary>
    public List<CraftingRecipe> GetRecipesByCategory(CraftingCategory category)
    {
        return recipes.Where(r => r != null && r.category == category).ToList();
    }

    /// <summary>
    /// 根据制作站获取配方列表
    /// </summary>
    public List<CraftingRecipe> GetRecipesByStation(CraftingStationType station)
    {
        return recipes.Where(r => r != null && r.requiredStation == station).ToList();
    }

    /// <summary>
    /// 获取所有配方
    /// </summary>
    public List<CraftingRecipe> GetAllRecipes()
    {
        return recipes.Where(r => r != null).ToList();
    }

    /// <summary>
    /// 添加配方
    /// </summary>
    public void AddRecipe(CraftingRecipe recipe)
    {
        if (recipe != null && !recipes.Contains(recipe))
        {
            recipes.Add(recipe);

            if (recipeDictionary != null && recipe.resultItem != null)
            {
                recipeDictionary.TryAdd(recipe.resultItem.itemName, recipe);
            }
        }
    }

    /// <summary>
    /// 移除配方
    /// </summary>
    public void RemoveRecipe(CraftingRecipe recipe)
    {
        if (recipe != null && recipes.Contains(recipe))
        {
            recipes.Remove(recipe);

            if (recipeDictionary != null && recipe.resultItem != null)
            {
                recipeDictionary.Remove(recipe.resultItem.itemName);
            }
        }
    }

    /// <summary>
    /// 获取配方总数
    /// </summary>
    public int RecipeCount => recipes.Count(r => r != null);
}

/// <summary>
/// 制作配方（扩展版本）
/// </summary>
[System.Serializable]
public class CraftingRecipe
{
    [Header("基础信息")]
    public string recipeName;
    public string recipeDescription;
    public CraftingCategory category;

    [Header("产物")]
    public Item resultItem;
    public int resultAmount = 1;

    [Header("材料")]
    public List<CraftingIngredient> ingredients = new List<CraftingIngredient>();

    [Header("制作要求")]
    public CraftingStationType requiredStation;
    public float craftTime = 1f;
    public int requiredLevel = 0;

    [Header("解锁条件")]
    public List<TechTreeData> requiredTech = new List<TechTreeData>();

    [Header("制作效果")]
    public float successRate = 1f;
    public List<CraftingEffect> effects = new List<CraftingEffect>();
}

/// <summary>
/// 制作材料
/// </summary>
[System.Serializable]
public class CraftingIngredient
{
    public Item item;
    public int amount;
    public bool isConsumed = true; // 是否消耗
}

/// <summary>
/// 制作效果
/// </summary>
[System.Serializable]
public class CraftingEffect
{
    public EffectType type;
    public float value;
    public float duration;
}

/// <summary>
/// 效果类型
/// </summary>
public enum EffectType
{
    HealthRestore,      // 恢复生命
    HungerRestore,      // 恢复饥饿
    ThirstRestore,      // 恢复口渴
    StaminaRestore,     // 恢复体力
    DamageBoost,        // 伤害提升
    ArmorBoost,         // 护甲提升
    SpeedBoost,         // 速度提升
    Regeneration,       // 生命恢复
    Poison,             // 中毒
    Burn                // 燃烧
}