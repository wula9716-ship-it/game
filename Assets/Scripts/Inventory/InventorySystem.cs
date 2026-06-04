using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// 背包系统 - 管理玩家物品
/// </summary>
public class InventorySystem : MonoBehaviour
{
    [Header("背包设置")]
    [SerializeField] private int inventorySize = 20;
    [SerializeField] private int hotbarSize = 8;

    [Header("物品数据库")]
    [SerializeField] private ItemDatabase itemDatabase;

    // 背包物品列表
    private List<InventorySlot> inventorySlots = new List<InventorySlot>();
    private List<InventorySlot> hotbarSlots = new List<InventorySlot>();

    // 事件
    public UnityEvent<int, InventorySlot> OnInventoryChanged;
    public UnityEvent<int, InventorySlot> OnHotbarChanged;
    public UnityEvent<ItemData, int> OnItemAdded;
    public UnityEvent<ItemData, int> OnItemRemoved;

    private void Awake()
    {
        InitializeInventory();
    }

    /// <summary>
    /// 初始化背包
    /// </summary>
    private void InitializeInventory()
    {
        // 初始化背包槽位
        for (int i = 0; i < inventorySize; i++)
        {
            inventorySlots.Add(new InventorySlot());
        }

        // 初始化快捷栏槽位
        for (int i = 0; i < hotbarSize; i++)
        {
            hotbarSlots.Add(new InventorySlot());
        }
    }

    /// <summary>
    /// 添加物品
    /// </summary>
    public bool AddItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;

        int remainingAmount = amount;

        // 首先尝试堆叠到已有物品
        if (item.isStackable)
        {
            foreach (var slot in inventorySlots)
            {
                if (slot.Item == item && slot.Amount < item.maxStackSize)
                {
                    int spaceAvailable = item.maxStackSize - slot.Amount;
                    int toAdd = Mathf.Min(remainingAmount, spaceAvailable);

                    slot.AddAmount(toAdd);
                    remainingAmount -= toAdd;

                    if (remainingAmount <= 0)
                    {
                        OnItemAdded?.Invoke(item, amount);
                        return true;
                    }
                }
            }
        }

        // 然后添加到空槽位
        foreach (var slot in inventorySlots)
        {
            if (slot.IsEmpty)
            {
                int toAdd = Mathf.Min(remainingAmount, item.maxStackSize);
                slot.SetItem(item, toAdd);
                remainingAmount -= toAdd;

                if (remainingAmount <= 0)
                {
                    OnItemAdded?.Invoke(item, amount);
                    return true;
                }
            }
        }

        // 背包已满
        if (remainingAmount < amount)
        {
            OnItemAdded?.Invoke(item, amount - remainingAmount);
            Debug.LogWarning("背包已满，部分物品无法添加");
            return false;
        }

        return false;
    }

    /// <summary>
    /// 移除物品
    /// </summary>
    public bool RemoveItem(ItemData item, int amount = 1)
    {
        if (item == null || amount <= 0) return false;

        int remainingAmount = amount;

        // 查找并移除物品
        foreach (var slot in inventorySlots)
        {
            if (slot.Item == item)
            {
                int toRemove = Mathf.Min(remainingAmount, slot.Amount);
                slot.RemoveAmount(toRemove);
                remainingAmount -= toRemove;

                if (remainingAmount <= 0)
                {
                    OnItemRemoved?.Invoke(item, amount);
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 从指定槽位移除物品
    /// </summary>
    public bool RemoveItemFromSlot(int slotIndex, int amount = 1)
    {
        if (slotIndex < 0 || slotIndex >= inventorySlots.Count) return false;

        var slot = inventorySlots[slotIndex];
        if (slot.IsEmpty) return false;

        ItemData item = slot.Item;
        int toRemove = Mathf.Min(amount, slot.Amount);

        slot.RemoveAmount(toRemove);
        OnItemRemoved?.Invoke(item, toRemove);
        OnInventoryChanged?.Invoke(slotIndex, slot);

        return true;
    }

    /// <summary>
    /// 检查是否有足够物品
    /// </summary>
    public bool HasItem(ItemData item, int amount = 1)
    {
        int totalCount = 0;

        foreach (var slot in inventorySlots)
        {
            if (slot.Item == item)
            {
                totalCount += slot.Amount;
            }
        }

        return totalCount >= amount;
    }

    /// <summary>
    /// 获取物品总数
    /// </summary>
    public int GetItemCount(ItemData item)
    {
        int totalCount = 0;

        foreach (var slot in inventorySlots)
        {
            if (slot.Item == item)
            {
                totalCount += slot.Amount;
            }
        }

        return totalCount;
    }

    /// <summary>
    /// 交换槽位
    /// </summary>
    public void SwapSlots(int fromIndex, int toIndex, bool isHotbarFrom = false, bool isHotbarTo = false)
    {
        var fromSlots = isHotbarFrom ? hotbarSlots : inventorySlots;
        var toSlots = isHotbarTo ? hotbarSlots : inventorySlots;

        if (fromIndex < 0 || fromIndex >= fromSlots.Count) return;
        if (toIndex < 0 || toIndex >= toSlots.Count) return;

        var fromSlot = fromSlots[fromIndex];
        var toSlot = toSlots[toIndex];

        // 如果目标槽位为空，直接移动
        if (toSlot.IsEmpty)
        {
            toSlot.SetItem(fromSlot.Item, fromSlot.Amount);
            fromSlot.Clear();
        }
        // 如果是同一物品且可堆叠
        else if (fromSlot.Item == toSlot.Item && fromSlot.Item.isStackable)
        {
            int spaceAvailable = toSlot.Item.maxStackSize - toSlot.Amount;
            int toMove = Mathf.Min(fromSlot.Amount, spaceAvailable);

            toSlot.AddAmount(toMove);
            fromSlot.RemoveAmount(toMove);

            if (fromSlot.Amount <= 0)
            {
                fromSlot.Clear();
            }
        }
        // 否则交换
        else
        {
            ItemData tempItem = fromSlot.Item;
            int tempAmount = fromSlot.Amount;

            fromSlot.SetItem(toSlot.Item, toSlot.Amount);
            toSlot.SetItem(tempItem, tempAmount);
        }

        // 触发事件
        if (isHotbarFrom)
            OnHotbarChanged?.Invoke(fromIndex, fromSlot);
        else
            OnInventoryChanged?.Invoke(fromIndex, fromSlot);

        if (isHotbarTo)
            OnHotbarChanged?.Invoke(toIndex, toSlot);
        else
            OnInventoryChanged?.Invoke(toIndex, toSlot);
    }

    /// <summary>
    /// 获取快捷栏选中的物品
    /// </summary>
    public ItemData GetSelectedItem(int hotbarIndex)
    {
        if (hotbarIndex < 0 || hotbarIndex >= hotbarSize) return null;
        return hotbarSlots[hotbarIndex].Item;
    }

    /// <summary>
    /// 获取背包槽位
    /// </summary>
    public InventorySlot GetInventorySlot(int index)
    {
        if (index < 0 || index >= inventorySlots.Count) return null;
        return inventorySlots[index];
    }

    /// <summary>
    /// 获取快捷栏槽位
    /// </summary>
    public InventorySlot GetHotbarSlot(int index)
    {
        if (index < 0 || index >= hotbarSize) return null;
        return hotbarSlots[index];
    }

    /// <summary>
    /// 获取背包大小
    /// </summary>
    public int InventorySize => inventorySize;

    /// <summary>
    /// 获取快捷栏大小
    /// </summary>
    public int HotbarSize => hotbarSize;
}

/// <summary>
/// 背包槽位
/// </summary>
[System.Serializable]
public class InventorySlot
{
    [SerializeField] private ItemData item;
    [SerializeField] private int amount;

    public ItemData Item => item;
    public int Amount => amount;
    public bool IsEmpty => item == null || amount <= 0;

    public void SetItem(ItemData newItem, int newAmount)
    {
        item = newItem;
        amount = newAmount;
    }

    public void AddAmount(int addAmount)
    {
        amount += addAmount;
    }

    public void RemoveAmount(int removeAmount)
    {
        amount -= removeAmount;
        if (amount <= 0)
        {
            Clear();
        }
    }

    public void Clear()
    {
        item = null;
        amount = 0;
    }
}