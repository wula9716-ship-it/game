using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 制作配方创建器 - 创建预设的配方数据
/// </summary>
public class CraftingRecipeCreator : MonoBehaviour
{
    [Header("物品数据库")]
    [SerializeField] private ItemDatabase itemDatabase;

    [Header("配方数据库")]
    [SerializeField] private CraftingDatabase craftingDatabase;

    /// <summary>
    /// 创建所有预设配方
    /// </summary>
    public void CreateAllRecipes()
    {
        if (itemDatabase == null || craftingDatabase == null)
        {
            Debug.LogError("数据库未设置！");
            return;
        }

        CreateToolRecipes();
        CreateWeaponRecipes();
        CreateArmorRecipes();
        CreateFoodRecipes();
        CreateMedicineRecipes();
        CreateBuildingRecipes();

        Debug.Log("所有配方创建完成！");
    }

    /// <summary>
    /// 创建工具配方
    /// </summary>
    private void CreateToolRecipes()
    {
        // 石斧
        CreateRecipe("石斧", CraftingCategory.Tools, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("石头"), amount = 3 },
                new CraftingIngredient { item = GetItem("木材"), amount = 2 },
                new CraftingIngredient { item = GetItem("植物纤维"), amount = 2 }
            },
            CraftingStationType.None, 5f);

        // 石镐
        CreateRecipe("石镐", CraftingCategory.Tools, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("石头"), amount = 3 },
                new CraftingIngredient { item = GetItem("木材"), amount = 3 },
                new CraftingIngredient { item = GetItem("植物纤维"), amount = 2 }
            },
            CraftingStationType.None, 5f);

        // 石矛
        CreateRecipe("石矛", CraftingCategory.Tools, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("石头"), amount = 2 },
                new CraftingIngredient { item = GetItem("木材"), amount = 4 },
                new CraftingIngredient { item = GetItem("植物纤维"), amount = 2 }
            },
            CraftingStationType.None, 5f);

        // 铁斧
        CreateRecipe("铁斧", CraftingCategory.Tools, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("铁锭"), amount = 3 },
                new CraftingIngredient { item = GetItem("木材"), amount = 2 },
                new CraftingIngredient { item = GetItem("皮革"), amount = 1 }
            },
            CraftingStationType.Workbench, 10f);

        // 铁镐
        CreateRecipe("铁镐", CraftingCategory.Tools, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("铁锭"), amount = 3 },
                new CraftingIngredient { item = GetItem("木材"), amount = 3 },
                new CraftingIngredient { item = GetItem("皮革"), amount = 1 }
            },
            CraftingStationType.Workbench, 10f);

        // 铁剑
        CreateRecipe("铁剑", CraftingCategory.Tools, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("铁锭"), amount = 4 },
                new CraftingIngredient { item = GetItem("木材"), amount = 1 },
                new CraftingIngredient { item = GetItem("皮革"), amount = 2 }
            },
            CraftingStationType.Workbench, 15f);
    }

    /// <summary>
    /// 创建武器配方
    /// </summary>
    private void CreateWeaponRecipes()
    {
        // 弓
        CreateRecipe("弓", CraftingCategory.Weapons, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("木材"), amount = 5 },
                new CraftingIngredient { item = GetItem("植物纤维"), amount = 3 }
            },
            CraftingStationType.Workbench, 10f);

        // 箭矢
        CreateRecipe("箭矢", CraftingCategory.Weapons, 10,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("木材"), amount = 1 },
                new CraftingIngredient { item = GetItem("石头"), amount = 1 },
                new CraftingIngredient { item = GetItem("羽毛"), amount = 1 }
            },
            CraftingStationType.None, 5f);

        // 铁矛
        CreateRecipe("铁矛", CraftingCategory.Weapons, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("铁锭"), amount = 3 },
                new CraftingIngredient { item = GetItem("木材"), amount = 4 },
                new CraftingIngredient { item = GetItem("皮革"), amount = 1 }
            },
            CraftingStationType.Workbench, 12f);

        // 铁盾
        CreateRecipe("铁盾", CraftingCategory.Weapons, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("铁锭"), amount = 5 },
                new CraftingIngredient { item = GetItem("木材"), amount = 3 },
                new CraftingIngredient { item = GetItem("皮革"), amount = 2 }
            },
            CraftingStationType.Workbench, 15f);
    }

    /// <summary>
    /// 创建护甲配方
    /// </summary>
    private void CreateArmorRecipes()
    {
        // 皮革护甲
        CreateRecipe("皮革护甲", CraftingCategory.Armor, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("皮革"), amount = 5 },
                new CraftingIngredient { item = GetItem("植物纤维"), amount = 3 }
            },
            CraftingStationType.Workbench, 10f);

        // 铁护甲
        CreateRecipe("铁护甲", CraftingCategory.Armor, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("铁锭"), amount = 8 },
                new CraftingIngredient { item = GetItem("皮革"), amount = 3 },
                new CraftingIngredient { item = GetItem("布料"), amount = 2 }
            },
            CraftingStationType.Workbench, 20f);

        // 铁头盔
        CreateRecipe("铁头盔", CraftingCategory.Armor, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("铁锭"), amount = 4 },
                new CraftingIngredient { item = GetItem("皮革"), amount = 1 }
            },
            CraftingStationType.Workbench, 12f);

        // 铁靴子
        CreateRecipe("铁靴子", CraftingCategory.Armor, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("铁锭"), amount = 3 },
                new CraftingIngredient { item = GetItem("皮革"), amount = 2 }
            },
            CraftingStationType.Workbench, 10f);
    }

    /// <summary>
    /// 创建食物配方
    /// </summary>
    private void CreateFoodRecipes()
    {
        // 烤肉
        CreateRecipe("烤肉", CraftingCategory.Food, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("生肉"), amount = 1 }
            },
            CraftingStationType.CookingStation, 5f);

        // 烤鱼
        CreateRecipe("烤鱼", CraftingCategory.Food, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("生鱼"), amount = 1 }
            },
            CraftingStationType.CookingStation, 5f);

        // 炖菜
        CreateRecipe("炖菜", CraftingCategory.Food, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("生肉"), amount = 1 },
                new CraftingIngredient { item = GetItem("浆果"), amount = 2 },
                new CraftingIngredient { item = GetItem("蘑菇"), amount = 1 }
            },
            CraftingStationType.CookingStation, 10f);

        // 肉汤
        CreateRecipe("肉汤", CraftingCategory.Food, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("生肉"), amount = 2 },
                new CraftingIngredient { item = GetItem("水"), amount = 1 }
            },
            CraftingStationType.CookingStation, 8f);

        // 烤椰子
        CreateRecipe("烤椰子", CraftingCategory.Food, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("椰子"), amount = 1 }
            },
            CraftingStationType.None, 3f);
    }

    /// <summary>
    /// 创建药品配方
    /// </summary>
    private void CreateMedicineRecipes()
    {
        // 草药膏
        CreateRecipe("草药膏", CraftingCategory.Medicine, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("草药"), amount = 3 },
                new CraftingIngredient { item = GetItem("水"), amount = 1 }
            },
            CraftingStationType.None, 5f);

        // 绷带
        CreateRecipe("绷带", CraftingCategory.Medicine, 3,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("布料"), amount = 2 }
            },
            CraftingStationType.None, 3f);

        // 解毒剂
        CreateRecipe("解毒剂", CraftingCategory.Medicine, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("草药"), amount = 5 },
                new CraftingIngredient { item = GetItem("蘑菇"), amount = 2 },
                new CraftingIngredient { item = GetItem("水"), amount = 1 }
            },
            CraftingStationType.AlchemyStation, 15f);

        // 生命药水
        CreateRecipe("生命药水", CraftingCategory.Medicine, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("草药"), amount = 10 },
                new CraftingIngredient { item = GetItem("蜂蜜"), amount = 2 },
                new CraftingIngredient { item = GetItem("水"), amount = 2 }
            },
            CraftingStationType.AlchemyStation, 20f);
    }

    /// <summary>
    /// 创建建筑配方
    /// </summary>
    private void CreateBuildingRecipes()
    {
        // 工作台
        CreateRecipe("工作台", CraftingCategory.Building, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("木材"), amount = 10 },
                new CraftingIngredient { item = GetItem("石头"), amount = 5 }
            },
            CraftingStationType.None, 15f);

        // 熔炉
        CreateRecipe("熔炉", CraftingCategory.Building, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("石头"), amount = 20 },
                new CraftingIngredient { item = GetItem("粘土"), amount = 10 }
            },
            CraftingStationType.Workbench, 20f);

        // 织布机
        CreateRecipe("织布机", CraftingCategory.Building, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("木材"), amount = 15 },
                new CraftingIngredient { item = GetItem("植物纤维"), amount = 10 }
            },
            CraftingStationType.Workbench, 15f);

        // 研究台
        CreateRecipe("研究台", CraftingCategory.Building, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("木材"), amount = 20 },
                new CraftingIngredient { item = GetItem("石头"), amount = 10 },
                new CraftingIngredient { item = GetItem("铁锭"), amount = 5 }
            },
            CraftingStationType.Workbench, 25f);

        // 烹饪站
        CreateRecipe("烹饪站", CraftingCategory.Building, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("石头"), amount = 15 },
                new CraftingIngredient { item = GetItem("粘土"), amount = 8 },
                new CraftingIngredient { item = GetItem("铁锭"), amount = 3 }
            },
            CraftingStationType.Workbench, 18f);

        // 炼金站
        CreateRecipe("炼金站", CraftingCategory.Building, 1,
            new List<CraftingIngredient>
            {
                new CraftingIngredient { item = GetItem("石头"), amount = 25 },
                new CraftingIngredient { item = GetItem("铁锭"), amount = 10 },
                new CraftingIngredient { item = GetItem("玻璃"), amount = 5 }
            },
            CraftingStationType.Workbench, 30f);
    }

    /// <summary>
    /// 创建配方
    /// </summary>
    private void CreateRecipe(string name, CraftingCategory category, int resultAmount,
        List<CraftingIngredient> ingredients, CraftingStationType station, float craftTime)
    {
        ItemData resultItem = GetItem(name);
        if (resultItem == null)
        {
            Debug.LogWarning($"物品未找到: {name}");
            return;
        }

        CraftingRecipe recipe = new CraftingRecipe
        {
            recipeName = name,
            category = category,
            resultItem = resultItem,
            resultAmount = resultAmount,
            ingredients = ingredients,
            requiredStation = station,
            craftTime = craftTime
        };

        craftingDatabase.AddRecipe(recipe);
    }

    /// <summary>
    /// 获取物品
    /// </summary>
    private ItemData GetItem(string name)
    {
        return itemDatabase?.GetItemByName(name);
    }
}