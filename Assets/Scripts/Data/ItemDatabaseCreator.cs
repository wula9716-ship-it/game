using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 物品数据库创建器 - 创建预设的物品数据
/// </summary>
public class ItemDatabaseCreator : MonoBehaviour
{
    [Header("物品数据库")]
    [SerializeField] private ItemDatabase itemDatabase;

    /// <summary>
    /// 创建所有预设物品
    /// </summary>
    public void CreateAllItems()
    {
        if (itemDatabase == null)
        {
            Debug.LogError("物品数据库未设置！");
            return;
        }

        CreateBasicResources();
        CreateFoodItems();
        CreateMaterialItems();
        CreateToolItems();
        CreateWeaponItems();
        CreateArmorItems();
        CreateBuildingItems();
        CreateSpecialItems();

        Debug.Log("所有物品创建完成！");
    }

    /// <summary>
    /// 创建基础资源
    /// </summary>
    private void CreateBasicResources()
    {
        // 木材
        CreateItem("木材", "基础建造材料", ItemType.Material, ItemRarity.Common,
            true, 99, 0, 0, 0, 0, 0, 0, 0, true, false, false, true, true);

        // 石头
        CreateItem("石头", "基础建造材料", ItemType.Material, ItemRarity.Common,
            true, 99, 0, 0, 0, 0, 0, 0, 0, true, false, false, true, true);

        // 植物纤维
        CreateItem("植物纤维", "基础制作材料", ItemType.Material, ItemRarity.Common,
            true, 99, 0, 0, 0, 0, 0, 0, 0, true, false, false, false, true);

        // 沙子
        CreateItem("沙子", "玻璃制作材料", ItemType.Material, ItemRarity.Common,
            true, 99, 0, 0, 0, 0, 0, 0, 0, true, false, false, false, true);

        // 粘土
        CreateItem("粘土", "陶器制作材料", ItemType.Material, ItemRarity.Common,
            true, 99, 0, 0, 0, 0, 0, 0, 0, true, false, false, false, true);

        // 绳索
        CreateItem("绳索", "基础工具材料", ItemType.Material, ItemRarity.Common,
            true, 99, 0, 0, 0, 0, 0, 0, 0, true, false, false, false, true);

        // 布料
        CreateItem("布料", "衣物制作材料", ItemType.Material, ItemRarity.Common,
            true, 99, 0, 0, 0, 0, 0, 0, 0, true, false, false, false, true);

        // 皮革
        CreateItem("皮革", "护甲制作材料", ItemType.Material, ItemRarity.Common,
            true, 99, 0, 0, 0, 0, 0, 0, 0, true, false, false, false, true);

        // 羽毛
        CreateItem("羽毛", "箭矢制作材料", ItemType.Material, ItemRarity.Common,
            true, 99, 0, 0, 0, 0, 0, 0, 0, true, false, false, false, true);

        // 纸
        CreateItem("纸", "书写材料", ItemType.Material, ItemRarity.Common,
            true, 99, 0, 0, 0, 0, 0, 0, 0, true, false, false, false, true);
    }

    /// <summary>
    /// 创建食物物品
    /// </summary>
    private void CreateFoodItems()
    {
        // 浆果
        CreateItem("浆果", "可食用的浆果", ItemType.Consumable, ItemRarity.Common,
            true, 20, 10, 5, 0, 0, 0, 0, 0, false, true, false, false, false);

        // 蘑菇
        CreateItem("蘑菇", "可食用的蘑菇", ItemType.Consumable, ItemRarity.Common,
            true, 20, 15, 5, 0, 0, 0, 0, 0, false, true, false, false, false);

        // 椰子
        CreateItem("椰子", "可食用的椰子", ItemType.Consumable, ItemRarity.Common,
            true, 10, 20, 15, 0, 0, 0, 0, 0, false, true, true, false, false);

        // 生肉
        CreateItem("生肉", "需要烹饪的肉类", ItemType.Consumable, ItemRarity.Common,
            true, 10, 0, 0, 0, 0, 0, 0, 0, false, true, false, false, false);

        // 生鱼
        CreateItem("生鱼", "需要烹饪的鱼类", ItemType.Consumable, ItemRarity.Common,
            true, 10, 0, 0, 0, 0, 0, 0, 0, false, true, false, false, false);

        // 烤肉
        CreateItem("烤肉", "烹饪好的肉类", ItemType.Consumable, ItemRarity.Common,
            true, 10, 40, 0, 0, 0, 0, 0, 0, false, true, false, false, false);

        // 烤鱼
        CreateItem("烤鱼", "烹饪好的鱼类", ItemType.Consumable, ItemRarity.Common,
            true, 10, 35, 0, 0, 0, 0, 0, 0, false, true, false, false, false);

        // 炖菜
        CreateItem("炖菜", "营养丰富的炖菜", ItemType.Consumable, ItemRarity.Uncommon,
            true, 5, 60, 20, 0, 0, 0, 0, 0, false, true, false, false, false);

        // 肉汤
        CreateItem("肉汤", "温暖的肉汤", ItemType.Consumable, ItemRarity.Common,
            true, 5, 30, 40, 0, 0, 0, 0, 0, false, true, true, false, false);

        // 蜂蜜
        CreateItem("蜂蜜", "甜美的蜂蜜", ItemType.Consumable, ItemRarity.Uncommon,
            true, 10, 25, 10, 0, 0, 0, 0, 0, false, true, false, false, false);
    }

    /// <summary>
    /// 创建材料物品
    /// </summary>
    private void CreateMaterialItems()
    {
        // 铁矿石
        CreateItem("铁矿石", "冶炼铁锭的原料", ItemType.Material, ItemRarity.Common,
            true, 99, 0, 0, 0, 0, 0, 0, 0, true, false, false, false, true);

        // 铜矿石
        CreateItem("铜矿石", "冶炼铜锭的原料", ItemType.Material, ItemRarity.Common,
            true, 99, 0, 0, 0, 0, 0, 0, 0, true, false, false, false, true);

        // 煤炭
        CreateItem("煤炭", "冶炼燃料", ItemType.Material, ItemRarity.Common,
            true, 99, 0, 0, 0, 0, 0, 0, 0, true, false, false, false, true);

        // 铁锭
        CreateItem("铁锭", "基础金属材料", ItemType.Material, ItemRarity.Common,
            true, 99, 0, 0, 0, 0, 0, 0, 0, true, false, false, false, true);

        // 铜锭
        CreateItem("铜锭", "导电金属材料", ItemType.Material, ItemRarity.Common,
            true, 99, 0, 0, 0, 0, 0, 0, 0, true, false, false, false, true);

        // 钢锭
        CreateItem("钢锭", "高级金属材料", ItemType.Material, ItemRarity.Uncommon,
            true, 99, 0, 0, 0, 0, 0, 0, 0, true, false, false, false, true);

        // 玻璃
        CreateItem("玻璃", "透明材料", ItemType.Material, ItemRarity.Common,
            true, 99, 0, 0, 0, 0, 0, 0, 0, true, false, false, false, true);

        // 木炭
        CreateItem("木炭", "燃料和过滤材料", ItemType.Material, ItemRarity.Common,
            true, 99, 0, 0, 0, 0, 0, 0, 0, true, false, false, false, true);

        // 盐
        CreateItem("盐", "食物保存材料", ItemType.Material, ItemRarity.Common,
            true, 99, 0, 0, 0, 0, 0, 0, 0, true, false, false, false, true);

        // 草药
        CreateItem("草药", "医疗材料", ItemType.Material, ItemRarity.Common,
            true, 99, 0, 0, 0, 0, 0, 0, 0, true, false, false, false, true);
    }

    /// <summary>
    /// 创建工具物品
    /// </summary>
    private void CreateToolItems()
    {
        // 石斧
        CreateItem("石斧", "基础砍伐工具", ItemType.Tool, ItemRarity.Common,
            false, 1, 0, 0, 0, 0, 0, 15, 0, false, false, true, false, false);

        // 石镐
        CreateItem("石镐", "基础挖掘工具", ItemType.Tool, ItemRarity.Common,
            false, 1, 0, 0, 0, 0, 0, 12, 0, false, false, true, false, false);

        // 石矛
        CreateItem("石矛", "基础狩猎工具", ItemType.Tool, ItemRarity.Common,
            false, 1, 0, 0, 0, 0, 0, 18, 0, false, false, true, false, false);

        // 铁斧
        CreateItem("铁斧", "高级砍伐工具", ItemType.Tool, ItemRarity.Uncommon,
            false, 1, 0, 0, 0, 0, 0, 25, 0, false, false, true, false, false);

        // 铁镐
        CreateItem("铁镐", "高级挖掘工具", ItemType.Tool, ItemRarity.Uncommon,
            false, 1, 0, 0, 0, 0, 0, 22, 0, false, false, true, false, false);

        // 铁剑
        CreateItem("铁剑", "高级近战武器", ItemType.Tool, ItemRarity.Uncommon,
            false, 1, 0, 0, 0, 0, 0, 30, 0, false, false, true, false, false);

        // 铁矛
        CreateItem("铁矛", "高级狩猎工具", ItemType.Tool, ItemRarity.Uncommon,
            false, 1, 0, 0, 0, 0, 0, 28, 0, false, false, true, false, false);
    }

    /// <summary>
    /// 创建武器物品
    /// </summary>
    private void CreateWeaponItems()
    {
        // 弓
        CreateItem("弓", "远程武器", ItemType.Weapon, ItemRarity.Common,
            false, 1, 0, 0, 0, 0, 0, 20, 0, false, false, true, false, false);

        // 箭矢
        CreateItem("箭矢", "弓箭弹药", ItemType.Weapon, ItemRarity.Common,
            true, 50, 0, 0, 0, 0, 0, 10, 0, false, false, false, false, false);

        // 木盾
        CreateItem("木盾", "基础防御装备", ItemType.Weapon, ItemRarity.Common,
            false, 1, 0, 0, 0, 0, 15, 0, 0, false, false, true, false, false);

        // 铁盾
        CreateItem("铁盾", "高级防御装备", ItemType.Weapon, ItemRarity.Uncommon,
            false, 1, 0, 0, 0, 0, 25, 0, 0, false, false, true, false, false);
    }

    /// <summary>
    /// 创建护甲物品
    /// </summary>
    private void CreateArmorItems()
    {
        // 皮革护甲
        CreateItem("皮革护甲", "基础护甲", ItemType.Equipment, ItemRarity.Common,
            false, 1, 0, 0, 0, 0, 20, 0, 0, false, false, true, false, false);

        // 铁护甲
        CreateItem("铁护甲", "高级护甲", ItemType.Equipment, ItemRarity.Uncommon,
            false, 1, 0, 0, 0, 0, 35, 0, 0, false, false, true, false, false);

        // 铁头盔
        CreateItem("铁头盔", "头部防护", ItemType.Equipment, ItemRarity.Uncommon,
            false, 1, 0, 0, 0, 0, 15, 0, 0, false, false, true, false, false);

        // 铁靴子
        CreateItem("铁靴子", "足部防护", ItemType.Equipment, ItemRarity.Uncommon,
            false, 1, 0, 0, 0, 0, 10, 0, 0, false, false, true, false, false);

        // 潜水镜
        CreateItem("潜水镜", "水下视觉装备", ItemType.Equipment, ItemRarity.Uncommon,
            false, 1, 0, 0, 0, 0, 0, 0, 0, false, false, true, false, false);
    }

    /// <summary>
    /// 创建建筑物品
    /// </summary>
    private void CreateBuildingItems()
    {
        // 木质地基
        CreateItem("木质地基", "基础建筑组件", ItemType.Building, ItemRarity.Common,
            true, 20, 0, 0, 0, 0, 0, 0, 0, true, false, false, true, false);

        // 木质墙壁
        CreateItem("木质墙壁", "基础墙壁", ItemType.Building, ItemRarity.Common,
            true, 20, 0, 0, 0, 0, 0, 0, 0, true, false, false, true, false);

        // 石质地基
        CreateItem("石质地基", "坚固的建筑组件", ItemType.Building, ItemRarity.Common,
            true, 20, 0, 0, 0, 0, 0, 0, 0, true, false, false, true, false);

        // 石质墙壁
        CreateItem("石质墙壁", "坚固的墙壁", ItemType.Building, ItemRarity.Common,
            true, 20, 0, 0, 0, 0, 0, 0, 0, true, false, false, true, false);

        // 铁质墙壁
        CreateItem("铁质墙壁", "高级墙壁", ItemType.Building, ItemRarity.Uncommon,
            true, 20, 0, 0, 0, 0, 0, 0, 0, true, false, false, true, false);

        // 铁门
        CreateItem("铁门", "高级门", ItemType.Building, ItemRarity.Uncommon,
            true, 10, 0, 0, 0, 0, 0, 0, 0, true, false, false, true, false);

        // 木筏
        CreateItem("木筏", "基础水上载具", ItemType.Building, ItemRarity.Common,
            false, 1, 0, 0, 0, 0, 0, 0, 0, true, false, false, true, false);

        // 船帆
        CreateItem("船帆", "船只组件", ItemType.Building, ItemRarity.Common,
            false, 1, 0, 0, 0, 0, 0, 0, 0, true, false, false, true, false);

        // 帆船
        CreateItem("帆船", "高级水上载具", ItemType.Building, ItemRarity.Rare,
            false, 1, 0, 0, 0, 0, 0, 0, 0, true, false, false, true, false);
    }

    /// <summary>
    /// 创建特殊物品
    /// </summary>
    private void CreateSpecialItems()
    {
        // 草药膏
        CreateItem("草药膏", "恢复生命值", ItemType.Consumable, ItemRarity.Common,
            true, 10, 30, 0, 0, 0, 0, 0, 0, false, false, true, false, false);

        // 绷带
        CreateItem("绷带", "恢复少量生命值", ItemType.Consumable, ItemRarity.Common,
            true, 20, 15, 0, 0, 0, 0, 0, 0, false, false, true, false, false);

        // 解毒剂
        CreateItem("解毒剂", "解除中毒状态", ItemType.Consumable, ItemRarity.Uncommon,
            true, 5, 0, 0, 0, 0, 0, 0, 0, false, false, true, false, false);

        // 生命药水
        CreateItem("生命药水", "恢复大量生命值", ItemType.Consumable, ItemRarity.Rare,
            true, 3, 80, 0, 0, 0, 0, 0, 0, false, false, true, false, false);

        // 水
        CreateItem("水", "基础饮用水", ItemType.Consumable, ItemRarity.Common,
            true, 10, 0, 0, 30, 0, 0, 0, 0, false, false, true, false, false);

        // 外星碎片
        CreateItem("外星碎片", "外星文明遗物", ItemType.Material, ItemRarity.Rare,
            true, 50, 0, 0, 0, 0, 0, 0, 0, true, false, false, false, false);

        // 能量水晶
        CreateItem("能量水晶", "蕴含能量的水晶", ItemType.Material, ItemRarity.Epic,
            true, 20, 0, 0, 0, 0, 0, 0, 0, true, false, false, false, false);

        // 外星合金
        CreateItem("外星合金", "外星科技材料", ItemType.Material, ItemRarity.Epic,
            true, 20, 0, 0, 0, 0, 0, 0, 0, true, false, false, false, false);

        // 外星芯片
        CreateItem("外星芯片", "外星科技核心", ItemType.Material, ItemRarity.Legendary,
            true, 5, 0, 0, 0, 0, 0, 0, 0, true, false, false, false, false);

        // 能量电池
        CreateItem("能量电池", "能量存储设备", ItemType.Material, ItemRarity.Rare,
            true, 10, 0, 0, 0, 0, 0, 0, 0, true, false, false, false, false);

        // 能量剑
        CreateItem("能量剑", "外星科技武器", ItemType.Weapon, ItemRarity.Epic,
            false, 1, 0, 0, 0, 0, 0, 50, 0, false, false, true, false, false);

        // 能量枪
        CreateItem("能量枪", "外星科技远程武器", ItemType.Weapon, ItemRarity.Epic,
            false, 1, 0, 0, 0, 0, 0, 45, 0, false, false, true, false, false);

        // 星际通讯器
        CreateItem("星际通讯器", "联系外界的设备", ItemType.Misc, ItemRarity.Legendary,
            false, 1, 0, 0, 0, 0, 0, 0, 0, true, false, false, false, false);
    }

    /// <summary>
    /// 创建物品
    /// </summary>
    private void CreateItem(string name, string description, ItemType type, ItemRarity rarity,
        bool stackable, int maxStack, float health, float hunger, float thirst, float stamina,
        float armor, float damage, float speed, bool building, bool edible, bool equipable,
        bool usable, bool placeable)
    {
        ItemData item = ScriptableObject.CreateInstance<ItemData>();
        item.itemName = name;
        item.itemDescription = description;
        item.itemType = type;
        item.rarity = rarity;
        item.isStackable = stackable;
        item.maxStackSize = maxStack;
        item.healthRestore = health;
        item.hungerRestore = hunger;
        item.thirstRestore = thirst;
        item.staminaRestore = stamina;
        item.armorValue = armor;
        item.damageValue = damage;
        item.speedModifier = speed;
        item.isBuildingMaterial = building;
        item.isEdible = edible;
        item.isEquipable = equipable;
        item.isUsable = usable;
        item.isPlaceable = placeable;

        itemDatabase.AddItem(item);
    }
}