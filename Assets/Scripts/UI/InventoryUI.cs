using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 背包界面 - 管理物品显示和操作
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("背包设置")]
    [SerializeField] private Transform inventoryContainer;
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private int columns = 5;

    [Header("物品信息")]
    [SerializeField] private GameObject itemInfoPanel;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private TextMeshProUGUI itemStats;

    [Header("操作按钮")]
    [SerializeField] private Button useButton;
    [SerializeField] private Button dropButton;
    [SerializeField] private Button splitButton;

    // 组件引用
    private InventorySystem inventorySystem;

    // 槽位列表
    private List<InventorySlotUI> inventorySlots = new List<InventorySlotUI>();
    private InventorySlotUI selectedSlot;

    private void Start()
    {
        inventorySystem = FindObjectOfType<InventorySystem>();
        InitializeInventory();
        RegisterEvents();
    }

    /// <summary>
    /// 初始化背包界面
    /// </summary>
    private void InitializeInventory()
    {
        if (inventorySystem == null || inventoryContainer == null || inventorySlotPrefab == null)
            return;

        // 清空容器
        foreach (Transform child in inventoryContainer)
        {
            Destroy(child.gameObject);
        }

        inventorySlots.Clear();

        // 创建背包槽位
        for (int i = 0; i < inventorySystem.InventorySize; i++)
        {
            GameObject slotObj = Instantiate(inventorySlotPrefab, inventoryContainer);
            InventorySlotUI slot = slotObj.GetComponent<InventorySlotUI>();

            if (slot != null)
            {
                slot.Initialize(i, this);
                inventorySlots.Add(slot);
            }
        }

        // 隐藏物品信息面板
        HideItemInfo();
    }

    /// <summary>
    /// 注册事件
    /// </summary>
    private void RegisterEvents()
    {
        if (useButton != null)
        {
            useButton.onClick.AddListener(OnUseButtonClicked);
        }

        if (dropButton != null)
        {
            dropButton.onClick.AddListener(OnDropButtonClicked);
        }

        if (splitButton != null)
        {
            splitButton.onClick.AddListener(OnSplitButtonClicked);
        }
    }

    /// <summary>
    /// 刷新背包显示
    /// </summary>
    public void RefreshInventory()
    {
        if (inventorySystem == null) return;

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            InventorySlot slot = inventorySystem.GetInventorySlot(i);
            inventorySlots[i].UpdateSlot(slot);
        }
    }

    /// <summary>
    /// 选择槽位
    /// </summary>
    public void SelectSlot(InventorySlotUI slot)
    {
        // 取消选中当前槽位
        if (selectedSlot != null)
        {
            selectedSlot.SetSelected(false);
        }

        // 选中新槽位
        selectedSlot = slot;
        selectedSlot.SetSelected(true);

        // 显示物品信息
        ShowItemInfo(slot.CurrentSlot);
    }

    /// <summary>
    /// 显示物品信息
    /// </summary>
    private void ShowItemInfo(InventorySlot slot)
    {
        if (slot == null || slot.IsEmpty)
        {
            HideItemInfo();
            return;
        }

        if (itemInfoPanel != null)
        {
            itemInfoPanel.SetActive(true);
        }

        if (itemIcon != null)
        {
            itemIcon.sprite = slot.Item.icon;
            itemIcon.enabled = true;
        }

        if (itemName != null)
        {
            itemName.text = slot.Item.itemName;
        }

        if (itemDescription != null)
        {
            itemDescription.text = slot.Item.itemDescription;
        }

        if (itemStats != null)
        {
            itemStats.text = GetItemStatsText(slot.Item);
        }

        // 更新按钮状态
        UpdateButtonStates(slot.Item);
    }

    /// <summary>
    /// 隐藏物品信息
    /// </summary>
    private void HideItemInfo()
    {
        if (itemInfoPanel != null)
        {
            itemInfoPanel.SetActive(false);
        }

        if (itemIcon != null)
        {
            itemIcon.enabled = false;
        }

        if (itemName != null)
        {
            itemName.text = "";
        }

        if (itemDescription != null)
        {
            itemDescription.text = "";
        }

        if (itemStats != null)
        {
            itemStats.text = "";
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

        if (item.speedModifier != 0)
            stats += $"速度修正: {(item.speedModifier > 0 ? "+" : "")}{item.speedModifier}%\n";

        return stats.TrimEnd('\n');
    }

    /// <summary>
    /// 更新按钮状态
    /// </summary>
    private void UpdateButtonStates(ItemData item)
    {
        if (useButton != null)
        {
            useButton.interactable = item.isUsable || item.isEdible || item.isDrinkable || item.isEquipable;
        }

        if (dropButton != null)
        {
            dropButton.interactable = true;
        }

        if (splitButton != null)
        {
            splitButton.interactable = selectedSlot != null && selectedSlot.CurrentSlot != null && selectedSlot.CurrentSlot.Amount > 1;
        }
    }

    /// <summary>
    /// 使用按钮点击
    /// </summary>
    private void OnUseButtonClicked()
    {
        if (selectedSlot == null || selectedSlot.CurrentSlot == null || selectedSlot.CurrentSlot.IsEmpty)
            return;

        ItemData item = selectedSlot.CurrentSlot.Item;

        // 使用物品
        if (item.isEdible)
        {
            // 恢复饥饿值
            PlayerStats playerStats = FindObjectOfType<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.RestoreHunger(item.hungerRestore);
            }
        }
        else if (item.isDrinkable)
        {
            // 恢复口渴值
            PlayerStats playerStats = FindObjectOfType<PlayerStats>();
            if (playerStats != null)
            {
                playerStats.RestoreThirst(item.thirstRestore);
            }
        }

        // 移除物品
        inventorySystem.RemoveItemFromSlot(selectedSlot.SlotIndex, 1);
        RefreshInventory();
        ShowItemInfo(selectedSlot.CurrentSlot);
    }

    /// <summary>
    /// 丢弃按钮点击
    /// </summary>
    private void OnDropButtonClicked()
    {
        if (selectedSlot == null || selectedSlot.CurrentSlot == null || selectedSlot.CurrentSlot.IsEmpty)
            return;

        // 丢弃物品
        inventorySystem.RemoveItemFromSlot(selectedSlot.SlotIndex, 1);
        RefreshInventory();
        ShowItemInfo(selectedSlot.CurrentSlot);
    }

    /// <summary>
    /// 分割按钮点击
    /// </summary>
    private void OnSplitButtonClicked()
    {
        if (selectedSlot == null || selectedSlot.CurrentSlot == null || selectedSlot.CurrentSlot.Amount <= 1)
            return;

        // TODO: 实现物品分割
        Debug.Log("分割物品");
    }
}

/// <summary>
/// 背包槽位UI
/// </summary>
public class InventorySlotUI : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Image selectionBorder;
    [SerializeField] private Image rarityBorder;

    // 槽位信息
    private int slotIndex;
    private InventorySlot currentSlot;
    private InventoryUI inventoryUI;
    private bool isSelected;

    /// <summary>
    /// 初始化槽位
    /// </summary>
    public void Initialize(int index, InventoryUI ui)
    {
        slotIndex = index;
        inventoryUI = ui;
        UpdateSlot(null);
    }

    /// <summary>
    /// 更新槽位显示
    /// </summary>
    public void UpdateSlot(InventorySlot slot)
    {
        currentSlot = slot;

        if (slot == null || slot.IsEmpty)
        {
            // 清空槽位
            if (iconImage != null)
            {
                iconImage.sprite = null;
                iconImage.enabled = false;
            }

            if (amountText != null)
            {
                amountText.text = "";
            }

            if (rarityBorder != null)
            {
                rarityBorder.enabled = false;
            }
        }
        else
        {
            // 更新槽位
            if (iconImage != null)
            {
                iconImage.sprite = slot.Item.icon;
                iconImage.enabled = true;
            }

            if (amountText != null)
            {
                amountText.text = slot.Amount > 1 ? slot.Amount.ToString() : "";
            }

            // 更新稀有度边框
            if (rarityBorder != null)
            {
                rarityBorder.enabled = true;
                rarityBorder.color = GetRarityColor(slot.Item.rarity);
            }
        }
    }

    /// <summary>
    /// 获取稀有度颜色
    /// </summary>
    private Color GetRarityColor(ItemRarity rarity)
    {
        switch (rarity)
        {
            case ItemRarity.Common:
                return Color.white;
            case ItemRarity.Uncommon:
                return Color.green;
            case ItemRarity.Rare:
                return Color.blue;
            case ItemRarity.Epic:
                return new Color(0.5f, 0f, 0.5f); // 紫色
            case ItemRarity.Legendary:
                return new Color(1f, 0.5f, 0f); // 橙色
            default:
                return Color.white;
        }
    }

    /// <summary>
    /// 设置选中状态
    /// </summary>
    public void SetSelected(bool selected)
    {
        isSelected = selected;

        if (selectionBorder != null)
        {
            selectionBorder.enabled = selected;
        }
    }

    /// <summary>
    /// 点击事件
    /// </summary>
    public void OnClick()
    {
        inventoryUI?.SelectSlot(this);
    }

    // 属性访问器
    public int SlotIndex => slotIndex;
    public InventorySlot CurrentSlot => currentSlot;
}