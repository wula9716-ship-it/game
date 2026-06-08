using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 建造界面 - 显示建筑列表和选择
/// </summary>
public class BuildingUI : MonoBehaviour
{
    [Header("建筑列表")]
    [SerializeField] private Transform buildingContainer;
    [SerializeField] private GameObject buildingSlotPrefab;

    [Header("分类标签")]
    [SerializeField] private Transform categoryContainer;
    [SerializeField] private GameObject categoryButtonPrefab;

    [Header("建筑信息")]
    [SerializeField] private GameObject buildingInfoPanel;
    [SerializeField] private Image buildingIcon;
    [SerializeField] private TextMeshProUGUI buildingName;
    [SerializeField] private TextMeshProUGUI buildingDescription;
    [SerializeField] private TextMeshProUGUI buildingStats;
    [SerializeField] private Transform requirementContainer;
    [SerializeField] private GameObject requirementPrefab;

    [Header("建造按钮")]
    [SerializeField] private Button buildButton;

    // 组件引用
    private BuildingSystem buildingSystem;
    private BuildingDatabase buildingDatabase;

    // 当前状态
    private BuildingCategory currentCategory;
    private BuildingData selectedBuilding;
    private List<BuildingSlotUI> buildingSlots = new List<BuildingSlotUI>();

    private void Start()
    {
        buildingSystem = FindObjectOfType<BuildingSystem>();
        buildingDatabase = FindObjectOfType<GameManager>()?.BuildingDatabase;

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
        foreach (BuildingCategory category in System.Enum.GetValues(typeof(BuildingCategory)))
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
        OnCategoryClicked(BuildingCategory.Foundation);
    }

    /// <summary>
    /// 注册事件
    /// </summary>
    private void RegisterEvents()
    {
        if (buildButton != null)
        {
            buildButton.onClick.AddListener(OnBuildButtonClicked);
        }
    }

    /// <summary>
    /// 刷新建筑列表
    /// </summary>
    public void RefreshBuildings()
    {
        if (buildingDatabase == null)
            return;

        // 清空建筑列表
        foreach (Transform child in buildingContainer)
        {
            Destroy(child.gameObject);
        }
        buildingSlots.Clear();

        // 获取当前分类的建筑
        List<BuildingData> buildings = buildingDatabase.GetBuildingsByCategory(currentCategory);

        // 创建建筑槽位
        foreach (BuildingData building in buildings)
        {
            GameObject slotObj = Instantiate(buildingSlotPrefab, buildingContainer);
            BuildingSlotUI slot = slotObj.GetComponent<BuildingSlotUI>();

            if (slot != null)
            {
                slot.Initialize(building, this);
                buildingSlots.Add(slot);
            }
        }

        // 隐藏建筑信息
        HideBuildingInfo();
    }

    /// <summary>
    /// 分类点击事件
    /// </summary>
    private void OnCategoryClicked(BuildingCategory category)
    {
        currentCategory = category;
        RefreshBuildings();
    }

    /// <summary>
    /// 选择建筑
    /// </summary>
    public void SelectBuilding(BuildingData building)
    {
        selectedBuilding = building;
        ShowBuildingInfo(building);
    }

    /// <summary>
    /// 显示建筑信息
    /// </summary>
    private void ShowBuildingInfo(BuildingData building)
    {
        if (building == null)
        {
            HideBuildingInfo();
            return;
        }

        if (buildingInfoPanel != null)
        {
            buildingInfoPanel.SetActive(true);
        }

        if (buildingIcon != null)
        {
            buildingIcon.sprite = building.icon;
            buildingIcon.enabled = true;
        }

        if (buildingName != null)
        {
            buildingName.text = building.buildingName;
        }

        if (buildingDescription != null)
        {
            buildingDescription.text = building.buildingDescription;
        }

        if (buildingStats != null)
        {
            buildingStats.text = GetBuildingStatsText(building);
        }

        // 显示资源需求
        ShowRequirements(building.requiredResources);

        // 更新建造按钮状态
        UpdateBuildButton();
    }

    /// <summary>
    /// 隐藏建筑信息
    /// </summary>
    private void HideBuildingInfo()
    {
        if (buildingInfoPanel != null)
        {
            buildingInfoPanel.SetActive(false);
        }

        if (buildingIcon != null)
        {
            buildingIcon.enabled = false;
        }

        if (buildingName != null)
        {
            buildingName.text = "";
        }

        if (buildingDescription != null)
        {
            buildingDescription.text = "";
        }

        if (buildingStats != null)
        {
            buildingStats.text = "";
        }

        // 清空资源需求
        if (requirementContainer != null)
        {
            foreach (Transform child in requirementContainer)
            {
                Destroy(child.gameObject);
            }
        }
    }

    /// <summary>
    /// 获取建筑属性文本
    /// </summary>
    private string GetBuildingStatsText(BuildingData building)
    {
        string stats = "";

        stats += $"生命值: {building.maxHealth}\n";
        stats += $"耐久度: {building.durability}\n";
        stats += $"建造时间: {building.buildTime}秒\n";

        if (building.defenseValue > 0)
            stats += $"防御值: +{building.defenseValue}\n";

        if (building.providesShelter)
            stats += "提供庇护\n";

        if (building.providesStorage)
            stats += $"存储槽位: {building.storageSlots}\n";

        if (building.providesCrafting)
            stats += $"制作站: {GetCraftingStationName(building.craftingStationType)}\n";

        return stats.TrimEnd('\n');
    }

    /// <summary>
    /// 显示资源需求
    /// </summary>
    private void ShowRequirements(List<ResourceRequirement> requirements)
    {
        if (requirementContainer == null || requirementPrefab == null)
            return;

        // 清空容器
        foreach (Transform child in requirementContainer)
        {
            Destroy(child.gameObject);
        }

        // 创建需求项
        foreach (var requirement in requirements)
        {
            GameObject reqObj = Instantiate(requirementPrefab, requirementContainer);
            RequirementUI reqUI = reqObj.GetComponent<RequirementUI>();

            if (reqUI != null)
            {
                reqUI.Initialize(requirement.item, requirement.amount);
            }
        }
    }

    /// <summary>
    /// 更新建造按钮状态
    /// </summary>
    private void UpdateBuildButton()
    {
        if (buildButton == null || selectedBuilding == null)
            return;

        // 检查是否有足够资源
        InventorySystem inventory = FindObjectOfType<InventorySystem>();
        bool canBuild = true;

        if (inventory != null)
        {
            foreach (var requirement in selectedBuilding.requiredResources)
            {
                if (!inventory.HasItem(requirement.item, requirement.amount))
                {
                    canBuild = false;
                    break;
                }
            }
        }

        buildButton.interactable = canBuild;
    }

    /// <summary>
    /// 建造按钮点击事件
    /// </summary>
    private void OnBuildButtonClicked()
    {
        if (selectedBuilding == null)
            return;

        // 选择建筑进行放置
        buildingSystem?.SelectBuilding(selectedBuilding);

        // 关闭建造界面
        UIManager.Instance?.HidePanel("Building");
    }

    /// <summary>
    /// 获取分类名称
    /// </summary>
    private string GetCategoryName(BuildingCategory category)
    {
        switch (category)
        {
            case BuildingCategory.Foundation:
                return "地基";
            case BuildingCategory.Wall:
                return "墙壁";
            case BuildingCategory.Floor:
                return "地板";
            case BuildingCategory.Roof:
                return "屋顶";
            case BuildingCategory.Door:
                return "门";
            case BuildingCategory.Window:
                return "窗户";
            case BuildingCategory.Furniture:
                return "家具";
            case BuildingCategory.Decoration:
                return "装饰";
            default:
                return category.ToString();
        }
    }

    /// <summary>
    /// 获取制作站名称
    /// </summary>
    private string GetCraftingStationName(CraftingStationType type)
    {
        switch (type)
        {
            case CraftingStationType.Workbench:
                return "工作台";
            case CraftingStationType.Furnace:
                return "熔炉";
            case CraftingStationType.Loom:
                return "织布机";
            case CraftingStationType.ResearchTable:
                return "研究台";
            case CraftingStationType.CookingStation:
                return "烹饪站";
            case CraftingStationType.AlchemyStation:
                return "炼金站";
            default:
                return "无";
        }
    }
}

/// <summary>
/// 建筑槽位UI
/// </summary>
public class BuildingSlotUI : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image selectionBorder;

    // 建筑信息
    private BuildingData buildingData;
    private BuildingUI buildingUI;

    /// <summary>
    /// 初始化槽位
    /// </summary>
    public void Initialize(BuildingData building, BuildingUI ui)
    {
        buildingData = building;
        buildingUI = ui;

        if (iconImage != null)
        {
            iconImage.sprite = building.icon;
        }

        if (nameText != null)
        {
            nameText.text = building.buildingName;
        }
    }

    /// <summary>
    /// 点击事件
    /// </summary>
    public void OnClick()
    {
        buildingUI?.SelectBuilding(buildingData);
    }
}

/// <summary>
/// 资源需求UI
/// </summary>
public class RequirementUI : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI amountText;

    /// <summary>
    /// 初始化需求
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