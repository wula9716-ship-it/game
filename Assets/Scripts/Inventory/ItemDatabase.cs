using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 物品数据库 - 存储所有物品数据
/// </summary>
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Survival Island/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [Header("物品列表")]
    [SerializeField] private List<ItemData> items = new List<ItemData>();

    // 物品字典（用于快速查找）
    private Dictionary<string, ItemData> itemDictionary;

    /// <summary>
    /// 初始化数据库
    /// </summary>
    public void Initialize()
    {
        itemDictionary = new Dictionary<string, ItemData>();

        foreach (var item in items)
        {
            if (item != null && !itemDictionary.ContainsKey(item.itemName))
            {
                itemDictionary.Add(item.itemName, item);
            }
        }
    }

    /// <summary>
    /// 根据名称获取物品
    /// </summary>
    public ItemData GetItemByName(string itemName)
    {
        if (itemDictionary == null)
        {
            Initialize();
        }

        if (itemDictionary.TryGetValue(itemName, out ItemData item))
        {
            return item;
        }

        Debug.LogWarning($"物品未找到: {itemName}");
        return null;
    }

    /// <summary>
    /// 根据类型获取物品列表
    /// </summary>
    public List<ItemData> GetItemsByType(ItemType type)
    {
        return items.Where(item => item != null && item.itemType == type).ToList();
    }

    /// <summary>
    /// 根据稀有度获取物品列表
    /// </summary>
    public List<ItemData> GetItemsByRarity(ItemRarity rarity)
    {
        return items.Where(item => item != null && item.rarity == rarity).ToList();
    }

    /// <summary>
    /// 获取所有物品
    /// </summary>
    public List<ItemData> GetAllItems()
    {
        return items.Where(item => item != null).ToList();
    }

    /// <summary>
    /// 获取物品总数
    /// </summary>
    public int ItemCount => items.Count(item => item != null);

    /// <summary>
    /// 添加物品到数据库
    /// </summary>
    public void AddItem(ItemData item)
    {
        if (item != null && !items.Contains(item))
        {
            items.Add(item);

            if (itemDictionary != null)
            {
                itemDictionary.TryAdd(item.itemName, item);
            }
        }
    }

    /// <summary>
    /// 从数据库移除物品
    /// </summary>
    public void RemoveItem(ItemData item)
    {
        if (item != null && items.Contains(item))
        {
            items.Remove(item);

            if (itemDictionary != null)
            {
                itemDictionary.Remove(item.itemName);
            }
        }
    }
}