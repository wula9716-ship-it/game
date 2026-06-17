/// <summary>
/// 可交互接口 — Shared interface for interactable game objects
/// </summary>
public interface IInteractable
{
    void Interact(PlayerController player);
}

/// <summary>
/// 制作分类
/// </summary>
public enum CraftingCategory
{
    Tools,      // 工具
    Weapons,    // 武器
    Armor,      // 护甲
    Food,       // 食物
    Medicine,   // 药品
    Building    // 建筑
}
