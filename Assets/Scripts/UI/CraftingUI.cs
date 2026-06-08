using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 制作界面 - 显示制作配方和制作操作
/// </summary>
public class CraftingUI : MonoBehaviour
{
    [Header("配方列表")]
    [SerializeField] private Transform recipeContainer;
    [SerializeField] private GameObject recipeSlotPrefab;

    [Header("分类标签")]
    [SerializeField] private Transform categoryContainer;
    [SerializeField] private GameObject categoryButtonPrefab;

    [Header("配方信息")]
    [SerializeField] private GameObject recipeInfoPanel;
    [SerializeField] private Image resultIcon;
    [SerializeField] private TextMeshProUGUI resultName;
    [SerializeField] private TextMeshProUGUI resultDescription;
    [SerializeField] private TextMeshProUGUI resultStats;
    [SerializeField] private Transform ingredientContainer;
    [SerializeField] private GameObject ingredientPrefab;

    [Header("制作按钮")]
    [SerializeField] private Button craftButton;
    [SerializeField] private TextMeshProUGUI craftButtonText;

    [Header("制作站")]
    [SerializeField] private CraftingStationType currentStation = CraftingStationType.None;

    // 当前状态
    private CraftingCategory currentCategory;
    private CraftingRecipe selectedRecipe;
    private List<RecipeSlotUI> recipeSlots = new List<RecipeSlotUI>();

    private void Start()
    {
        InitializeCategories();
        RegisterEvents();
    }

    /// <summary>
    /// 初始化分类标签
    /// </summary>
    private void InitializeCategories()
    {
        if (categoryContainer == null || categoryButtonPrefab == null)
            return;

        // 清空容器
        foreach (Transform child in categoryContainer)
        {
            Destroy(child.gameObject);
        }

        // 创建分类按钮
        foreach (CraftingCategory category in System.Enum.GetValues(typeof(CraftingCategory)))
        {
            GameObject buttonObj = Instantiate(categoryButtonPrefab, categoryContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();

            if (button != null && buttonText != null)
            {
                buttonText.text = GetCategoryName(category);
                button.onClick.AddListener(() => OnCategoryClicked(category));
            }
        }

        // 默认选择第一个分类
        OnCategoryClicked(CraftingCategory.Tools);
    }

    /// <summary>
    /// 注册事件
    /// </summary>
    private void RegisterEvents()
    {
        if (craftButton != null)
        {
            craftButton.onClick.AddListener(OnCraftButtonClicked);
        }
    }

    /// <summary>
    /// 刷新配方列表
    /// </summary>
    public void RefreshRecipes()
    {
        // 清空配方列表
        foreach (Transform child in recipeContainer)
        {
            Destroy(child.gameObject);
        }
        recipeSlots.Clear();

        // 获取当前分类的配方
        List<CraftingRecipe> recipes = GetRecipesByCategory(currentCategory);

        // 创建配方槽位
        foreach (CraftingRecipe recipe in recipes)
        {
            GameObject slotObj = Instantiate(recipeSlotPrefab, recipeContainer);
            RecipeSlotUI slot = slotObj.GetComponent<RecipeSlotUI>();

            if (slot != null)
            {
                slot.Initialize(recipe, this);
                recipeSlots.Add(slot);
            }
        }

        // 隐藏配方信息
        HideRecipeInfo();
    }

    /// <summary>
    /// 分类点击事件
    /// </summary>
    private void OnCategoryClicked(CraftingCategory category)
    {
        currentCategory = category;
        RefreshRecipes();
    }

    /// <summary>
    /// 选择配方
    /// </summary>
    public void SelectRecipe(CraftingRecipe recipe)
    {
        selectedRecipe = recipe;
        ShowRecipeInfo(recipe);
    }

    /// <summary>
    /// 显示配方信息
    /// </summary>
    private void ShowRecipeInfo(CraftingRecipe recipe)
    {
        if (recipe == null)
        {
            HideRecipeInfo();
            return;
        }

        if (recipeInfoPanel != null)
        {
            recipeInfoPanel.SetActive(true);
        }

        if (resultIcon != null)
        {
            resultIcon.sprite = recipe.resultItem.icon;
            resultIcon.enabled = true;
        }

        if (resultName != null)
        {
            resultName.text = recipe.resultItem.itemName;
        }

        if (resultDescription != null)
        {
            resultDescription.text = recipe.resultItem.itemDescription;
        }

        if (resultStats != null)
        {
            resultStats.text = GetItemStatsText(recipe.resultItem);
        }

        // 显示材料需求
        ShowIngredients(recipe.ingredients);

        // 更新制作按钮状态
        UpdateCraftButton();
    }

    /// <summary>
    /// 隐藏配方信息
    /// </summary>
    private void HideRecipeInfo()
    {
        if (recipeInfoPanel != null)
        {
            recipeInfoPanel.SetActive(false);
        }

        if (resultIcon != null)
        {
            resultIcon.enabled = false;
        }

        if (resultName != null)
        {
            resultName.text = "";
        }

        if (resultDescription != null)
        {
            resultDescription.text = "";
        }

        if (resultStats != null)
        {
            resultStats.text = "";
        }

        // 清空材料需求
        if (ingredientContainer != null)
        {
            foreach (Transform child in ingredientContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }

    /// <summary>
    /// 获取物品属性文本
    /// </summary>
    private string GetItemStatsText(ItemData item)
    {
        string stats = "";

        if (item.healthRestore > 0)
            stats += $"生命恢复: +{item.healthRestore}\n";

        if (item.hungerRestore > 0)
            stats += $"饥饿恢复: +{item.hungerRestore}\n";

        if (item.thirstRestore > 0)
            stats += $"口渴恢复: +{item.thirstRestore}\n";

        if (item.staminaRestore > 0)
            stats += $"体力恢复: +{item.staminaRestore}\n";

        if (item.armorValue > 0)
            stats += $"护甲值: +{item.armorValue}\n";

        if (item.damageValue > 0)
            stats += $"伤害值: +{item.damageValue}\n";

        return stats.TrimEnd('\n');
    }

    /// <summary>
    /// 显示材料需求
    /// </summary>
    private void ShowIngredients(List<CraftingIngredient> ingredients)
    {
        if (ingredientContainer == null || ingredientPrefab == null)
            return;

        // 清空容器
        foreach (Transform child in ingredientContainer)
        {
            Destroy(child.gameObject);
        }

        // 创建材料项
        foreach (var ingredient in ingredients)
        {
            GameObject ingredientObj = Instantiate(ingredientPrefab, ingredientContainer);
            IngredientUI ingredientUI = ingredientObj.GetComponent<IngredientUI>();

            if (ingredientUI != null)
            {
                ingredientUI.Initialize(ingredient.item, ingredient.amount);
            }
        }
    }

    /// <summary>
    /// 更新制作按钮状态
    /// </summary>
    private void UpdateCraftButton()
    {
        if (craftButton == null || selectedRecipe == null)
            return;

        // 检查是否有足够材料
        InventorySystem inventory = FindObjectOfType<InventorySystem>();
        bool canCraft = true;

        if (inventory != null)
        {
            foreach (var ingredient in selectedRecipe.ingredients)
            {
                if (!inventory.HasItem(ingredient.item, ingredient.amount))
                {
                    canCraft = false;
                    break;
                }
            }
        }

        // 检查制作站
        if (selectedRecipe.requiredStation != CraftingStationType.None && selectedRecipe.requiredStation != currentStation)
        {
            canCraft = false;
        }

        craftButton.interactable = canCraft;

        if (craftButtonText != null)
        {
            craftButtonText.text = canCraft ? "制作" : "材料不足";
        }
    }

    /// <summary>
    /// 制作按钮点击事件
    /// </summary>
    private void OnCraftButtonClicked()
    {
        if (selectedRecipe == null)
            return;

        // 检查材料
        InventorySystem inventory = FindObjectOfType<InventorySystem>();
        if (inventory == null)
            return;

        // 检查是否有足够材料
        foreach (var ingredient in selectedRecipe.ingredients)
        {
            if (!inventory.HasItem(ingredient.item, ingredient.amount))
            {
                Debug.Log("材料不足！");
                return;
            }
        }

        // 消耗材料
        foreach (var ingredient in selectedRecipe.ingredients)
        {
            inventory.RemoveItem(ingredient.item, ingredient.amount);
        }

        // 添加产物
        inventory.AddItem(selectedRecipe.resultItem, selectedRecipe.resultAmount);

        Debug.Log($"制作成功: {selectedRecipe.resultItem.itemName} x{selectedRecipe.resultAmount}");

        // 刷新界面
        RefreshRecipes();
        ShowRecipeInfo(selectedRecipe);
    }

    /// <summary>
    /// 设置制作站
    /// </summary>
    public void SetCraftingStation(CraftingStationType station)
    {
        currentStation = station;
        RefreshRecipes();
    }

    /// <summary>
    /// 获取分类名称
    /// </summary>
    private string GetCategoryName(CraftingCategory category)
    {
        switch (category)
        {
            case CraftingCategory.Tools:
                return "工具";
            case CraftingCategory.Weapons:
                return "武器";
            case CraftingCategory.Armor:
                return "护甲";
            case CraftingCategory.Food:
                return "食物";
            case CraftingCategory.Medicine:
                return "药品";
            case CraftingCategory.Building:
                return "建筑";
            case CraftingCategory.Misc:
                return "其他";
            default:
                return category.ToString();
        }
    }

    /// <summary>
    /// 获取配方列表
    /// </summary>
    private List<CraftingRecipe> GetRecipesByCategory(CraftingCategory category)
    {
        // TODO: 从数据库获取配方
        // 这里返回示例配方
        List<CraftingRecipe> recipes = new List<CraftingRecipe>();

        // 示例配方
        if (category == CraftingCategory.Tools)
        {
            recipes.Add(new CraftingRecipe
            {
                resultItem = null, // 需要实际的ItemData
                resultAmount = 1,
                ingredients = new List<CraftingIngredient>(),
                requiredStation = CraftingStationType.None
            });
        }

        return recipes;
    }
}

/// <summary>
/// 配方槽位UI
/// </summary>
public class RecipeSlotUI : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Image selectionBorder;

    // 配方信息
    private CraftingRecipe recipe;
    private CraftingUI craftingUI;

    /// <summary>
    /// 初始化槽位
    /// </summary>
    public void Initialize(CraftingRecipe recipeData, CraftingUI ui)
    {
        recipe = recipeData;
        craftingUI = ui;

        if (iconImage != null && recipeData.resultItem != null)
        {
            iconImage.sprite = recipeData.resultItem.icon;
        }

        if (nameText != null && recipeData.resultItem != null)
        {
            nameText.text = recipeData.resultItem.itemName;
        }

        if (amountText != null)
        {
            amountText.text = recipeData.resultAmount > 1 ? $"x{recipeData.resultAmount}" : "";
        }
    }

    /// <summary>
    /// 点击事件
    /// </summary>
    public void OnClick()
    {
        craftingUI?.SelectRecipe(recipe);
    }
}

/// <summary>
/// 材料UI
/// </summary>
public class IngredientUI : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI amountText;

    /// <summary>
    /// 初始化材料
    /// </summary>
    public void Initialize(ItemData item, int amount)
    {
        if (iconImage != null)
        {
            iconImage.sprite = item.icon;
        }

        if (amountText != null)
        {
            InventorySystem inventory = FindObjectOfType<InventorySystem>();
            int currentAmount = inventory != null ? inventory.GetItemCount(item) : 0;
            bool hasEnough = currentAmount >= amount;

            amountText.text = $"{currentAmount}/{amount}";
            amountText.color = hasEnough ? Color.green : Color.red;
        }
    }
}

/// <summary>
/// 制作配方
/// </summary>
[System.Serializable]
public class CraftingRecipe
{
    public ItemData resultItem;
    public int resultAmount = 1;
    public List<CraftingIngredient> ingredients = new List<CraftingIngredient>();
    public CraftingStationType requiredStation;
    public float craftTime = 1f;
}

/// <summary>
/// 制作材料
/// </summary>
[System.Serializable]
public class CraftingIngredient
{
    public ItemData item;
    public int amount;
}