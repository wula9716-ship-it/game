using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 科技树界面 - 显示科技树和研究进度
/// </summary>
public class TechTreeUI : MonoBehaviour
{
    [Header("科技树显示")]
    [SerializeField] private Transform techContainer;
    [SerializeField] private GameObject techSlotPrefab;

    [Header("分类标签")]
    [SerializeField] private Transform categoryContainer;
    [SerializeField] private GameObject categoryButtonPrefab;

    [Header("科技信息")]
    [SerializeField] private GameObject techInfoPanel;
    [SerializeField] private Image techIcon;
    [SerializeField] private TextMeshProUGUI techName;
    [SerializeField] private TextMeshProUGUI techDescription;
    [SerializeField] private TextMeshProUGUI techRequirements;
    [SerializeField] private TextMeshProUGUI techEffects;

    [Header("研究进度")]
    [SerializeField] private Slider progressBar;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Button researchButton;
    [SerializeField] private Button cancelButton;

    [Header("连接线")]
    [SerializeField] private Transform connectionContainer;
    [SerializeField] private GameObject connectionPrefab;

    // 组件引用
    private TechTreeSystem techSystem;
    private TechTreeDatabase techDatabase;

    // 当前状态
    private TechCategory currentCategory;
    private TechData selectedTech;
    private List<TechSlotUI> techSlots = new List<TechSlotUI>();

    private void Start()
    {
        techSystem = TechTreeSystem.Instance;
        techDatabase = FindObjectOfType<GameManager>()?.GetComponent<TechTreeDatabase>();

        InitializeCategories();
        RegisterEvents();
    }

    private void Update()
    {
        UpdateResearchProgress();
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
        foreach (TechCategory category in System.Enum.GetValues(typeof(TechCategory)))
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
        OnCategoryClicked(TechCategory.Survival);
    }

    /// <summary>
    /// 注册事件
    /// </summary>
    private void RegisterEvents()
    {
        if (researchButton != null)
        {
            researchButton.onClick.AddListener(OnResearchButtonClicked);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(OnCancelButtonClicked);
        }

        if (techSystem != null)
        {
            techSystem.OnTechUnlocked.AddListener(OnTechUnlocked);
            techSystem.OnResearchCompleted.AddListener(OnResearchCompleted);
        }
    }

    /// <summary>
    /// 刷新科技树显示
    /// </summary>
    public void RefreshTechTree()
    {
        if (techDatabase == null)
            return;

        // 清空科技列表
        foreach (Transform child in techContainer)
        {
            Destroy(child.gameObject);
        }
        techSlots.Clear();

        // 清空连接线
        foreach (Transform child in connectionContainer)
        {
            Destroy(child.gameObject);
        }

        // 获取当前分类的科技
        List<TechData> techs = techDatabase.GetTechsByCategory(currentCategory);

        // 创建科技槽位
        foreach (TechData tech in techs)
        {
            GameObject slotObj = Instantiate(techSlotPrefab, techContainer);
            TechSlotUI slot = slotObj.GetComponent<TechSlotUI>();

            if (slot != null)
            {
                slot.Initialize(tech, this);
                techSlots.Add(slot);
            }
        }

        // 绘制连接线
        DrawConnections(techs);

        // 隐藏科技信息
        HideTechInfo();
    }

    /// <summary>
    /// 绘制连接线
    /// </summary>
    private void DrawConnections(List<TechData> techs)
    {
        if (connectionContainer == null || connectionPrefab == null)
            return;

        foreach (TechData tech in techs)
        {
            foreach (TechData prereq in tech.prerequisites)
            {
                // 查找前置科技槽位
                TechSlotUI fromSlot = techSlots.Find(s => s.TechData == prereq);
                TechSlotUI toSlot = techSlots.Find(s => s.TechData == tech);

                if (fromSlot != null && toSlot != null)
                {
                    // 创建连接线
                    GameObject connectionObj = Instantiate(connectionPrefab, connectionContainer);
                    TechConnection connection = connectionObj.GetComponent<TechConnection>();

                    if (connection != null)
                    {
                        connection.Initialize(fromSlot.transform, toSlot.transform);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 分类点击事件
    /// </summary>
    private void OnCategoryClicked(TechCategory category)
    {
        currentCategory = category;
        RefreshTechTree();
    }

    /// <summary>
    /// 选择科技
    /// </summary>
    public void SelectTech(TechData tech)
    {
        selectedTech = tech;
        ShowTechInfo(tech);
    }

    /// <summary>
    /// 显示科技信息
    /// </summary>
    private void ShowTechInfo(TechData tech)
    {
        if (tech == null)
        {
            HideTechInfo();
            return;
        }

        if (techInfoPanel != null)
        {
            techInfoPanel.SetActive(true);
        }

        if (techIcon != null)
        {
            techIcon.sprite = tech.icon;
            techIcon.enabled = true;
        }

        if (techName != null)
        {
            techName.text = tech.techName;
        }

        if (techDescription != null)
        {
            techDescription.text = tech.techDescription;
        }

        if (techRequirements != null)
        {
            techRequirements.text = GetRequirementsText(tech);
        }

        if (techEffects != null)
        {
            techEffects.text = GetEffectsText(tech);
        }

        // 更新研究按钮状态
        UpdateResearchButton();
    }

    /// <summary>
    /// 隐藏科技信息
    /// </summary>
    private void HideTechInfo()
    {
        if (techInfoPanel != null)
        {
            techInfoPanel.SetActive(false);
        }

        if (techIcon != null)
        {
            techIcon.enabled = false;
        }

        if (techName != null)
        {
            techName.text = "";
        }

        if (techDescription != null)
        {
            techDescription.text = "";
        }

        if (techRequirements != null)
        {
            techRequirements.text = "";
        }

        if (techEffects != null)
        {
            techEffects.text = "";
        }
    }

    /// <summary>
    /// 获取需求文本
    /// </summary>
    private string GetRequirementsText(TechData tech)
    {
        string text = "";

        // 前置科技
        if (tech.prerequisites.Count > 0)
        {
            text += "前置科技:\n";
            foreach (var prereq in tech.prerequisites)
            {
                bool unlocked = techSystem != null && techSystem.IsTechUnlocked(prereq);
                string status = unlocked ? "✓" : "✗";
                text += $"  {status} {prereq.techName}\n";
            }
        }

        // 资源需求
        if (tech.resourceRequirements.Count > 0)
        {
            text += "\n资源需求:\n";
            foreach (var req in tech.resourceRequirements)
            {
                InventorySystem inventory = FindObjectOfType<InventorySystem>();
                int current = inventory != null ? inventory.GetItemCount(req.item) : 0;
                bool hasEnough = current >= req.amount;
                string status = hasEnough ? "✓" : "✗";
                text += $"  {status} {req.item.itemName}: {current}/{req.amount}\n";
            }
        }

        return text.TrimEnd('\n');
    }

    /// <summary>
    /// 获取效果文本
    /// </summary>
    private string GetEffectsText(TechData tech)
    {
        string text = "科技效果:\n";

        foreach (var effect in tech.effects)
        {
            switch (effect.type)
            {
                case TechEffectType.UnlockRecipe:
                    text += $"  解锁配方: {effect.stringValue}\n";
                    break;
                case TechEffectType.UnlockBuilding:
                    text += $"  解锁建筑: {effect.stringValue}\n";
                    break;
                case TechEffectType.StatBonus:
                    text += $"  {effect.stringValue}: +{effect.floatValue * 100}%\n";
                    break;
                case TechEffectType.NewAbility:
                    text += $"  新能力: {effect.stringValue}\n";
                    break;
                case TechEffectType.CraftingSpeed:
                    text += $"  制作速度: +{effect.floatValue * 100}%\n";
                    break;
                case TechEffectType.BuildingSpeed:
                    text += $"  建造速度: +{effect.floatValue * 100}%\n";
                    break;
                case TechEffectType.ResearchSpeed:
                    text += $"  研究速度: +{effect.floatValue * 100}%\n";
                    break;
            }
        }

        return text.TrimEnd('\n');
    }

    /// <summary>
    /// 更新研究按钮状态
    /// </summary>
    private void UpdateResearchButton()
    {
        if (researchButton == null || selectedTech == null)
            return;

        bool canResearch = techSystem != null &&
                          !techSystem.IsTechUnlocked(selectedTech) &&
                          techSystem.HasPrerequisites(selectedTech) &&
                          techSystem.HasRequiredResources(selectedTech) &&
                          !techSystem.IsResearching;

        researchButton.interactable = canResearch;
    }

    /// <summary>
    /// 更新研究进度
    /// </summary>
    private void UpdateResearchProgress()
    {
        if (techSystem == null) return;

        if (techSystem.IsResearching)
        {
            TechData currentResearch = techSystem.GetCurrentResearch();
            float progress = techSystem.GetResearchProgress();

            if (progressBar != null)
            {
                progressBar.value = progress;
                progressBar.gameObject.SetActive(true);
            }

            if (progressText != null)
            {
                progressText.text = $"{currentResearch.techName}: {progress * 100:F1}%";
                progressText.gameObject.SetActive(true);
            }

            if (cancelButton != null)
            {
                cancelButton.gameObject.SetActive(true);
            }
        }
        else
        {
            if (progressBar != null)
            {
                progressBar.gameObject.SetActive(false);
            }

            if (progressText != null)
            {
                progressText.gameObject.SetActive(false);
            }

            if (cancelButton != null)
            {
                cancelButton.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 研究按钮点击事件
    /// </summary>
    private void OnResearchButtonClicked()
    {
        if (selectedTech == null || techSystem == null)
            return;

        techSystem.StartResearch(selectedTech);
        UpdateResearchButton();
    }

    /// <summary>
    /// 取消按钮点击事件
    /// </summary>
    private void OnCancelButtonClicked()
    {
        if (techSystem == null)
            return;

        techSystem.CancelResearch();
        UpdateResearchButton();
    }

    /// <summary>
    /// 科技解锁回调
    /// </summary>
    private void OnTechUnlocked(TechData tech)
    {
        RefreshTechTree();

        if (selectedTech != null)
        {
            ShowTechInfo(selectedTech);
        }
    }

    /// <summary>
    /// 研究完成回调
    /// </summary>
    private void OnResearchCompleted(TechData tech)
    {
        Debug.Log($"研究完成: {tech.techName}");
    }

    /// <summary>
    /// 获取分类名称
    /// </summary>
    private string GetCategoryName(TechCategory category)
    {
        switch (category)
        {
            case TechCategory.Survival:
                return "生存";
            case TechCategory.Building:
                return "建造";
            case TechCategory.Exploration:
                return "探索";
            case TechCategory.Combat:
                return "战斗";
            case TechCategory.Alien:
                return "外星科技";
            default:
                return category.ToString();
        }
    }
}

/// <summary>
/// 科技槽位UI
/// </summary>
public class TechSlotUI : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image selectionBorder;
    [SerializeField] private Image lockOverlay;
    [SerializeField] private Image unlockOverlay;

    // 科技信息
    private TechData techData;
    private TechTreeUI techTreeUI;

    /// <summary>
    /// 初始化槽位
    /// </summary>
    public void Initialize(TechData tech, TechTreeUI ui)
    {
        techData = tech;
        techTreeUI = ui;

        if (iconImage != null)
        {
            iconImage.sprite = tech.icon;
        }

        if (nameText != null)
        {
            nameText.text = tech.techName;
        }

        UpdateState();
    }

    /// <summary>
    /// 更新状态
    /// </summary>
    public void UpdateState()
    {
        TechTreeSystem techSystem = TechTreeSystem.Instance;
        if (techSystem == null) return;

        bool unlocked = techSystem.IsTechUnlocked(techData);
        bool canResearch = techSystem.HasPrerequisites(techData) && techSystem.HasRequiredResources(techData);

        if (lockOverlay != null)
        {
            lockOverlay.enabled = !unlocked && !canResearch;
        }

        if (unlockOverlay != null)
        {
            unlockOverlay.enabled = unlocked;
        }
    }

    /// <summary>
    /// 点击事件
    /// </summary>
    public void OnClick()
    {
        techTreeUI?.SelectTech(techData);
    }

    // 属性访问器
    public TechData TechData => techData;
}

/// <summary>
/// 科技连接线
/// </summary>
public class TechConnection : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private RectTransform lineRect;
    [SerializeField] private Image lineImage;

    /// <summary>
    /// 初始化连接线
    /// </summary>
    public void Initialize(Transform from, Transform to)
    {
        if (lineRect == null) return;

        // 计算位置和旋转
        Vector2 fromPos = from.GetComponent<RectTransform>().anchoredPosition;
        Vector2 toPos = to.GetComponent<RectTransform>().anchoredPosition;

        Vector2 direction = toPos - fromPos;
        float distance = direction.magnitude;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 设置位置和大小
        lineRect.anchoredPosition = fromPos + direction * 0.5f;
        lineRect.sizeDelta = new Vector2(distance, 2f);
        lineRect.rotation = Quaternion.Euler(0, 0, angle);
    }
}