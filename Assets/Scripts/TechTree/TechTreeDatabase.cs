using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 科技树数据库 - 存储所有科技数据
/// </summary>
[CreateAssetMenu(fileName = "TechTreeDatabase", menuName = "Survival Island/Tech Tree Database")]
public class TechTreeDatabase : ScriptableObject
{
    [Header("科技列表")]
    [SerializeField] private List<TechData> techs = new List<TechData>();

    // 科技字典（用于快速查找）
    private Dictionary<string, TechData> techDictionary;

    /// <summary>
    /// 初始化数据库
    /// </summary>
    public void Initialize()
    {
        techDictionary = new Dictionary<string, TechData>();

        foreach (var tech in techs)
        {
            if (tech != null && !techDictionary.ContainsKey(tech.techName))
            {
                techDictionary.Add(tech.techName, tech);
            }
        }
    }

    /// <summary>
    /// 根据名称获取科技
    /// </summary>
    public TechData GetTechByName(string techName)
    {
        if (techDictionary == null)
        {
            Initialize();
        }

        if (techDictionary.TryGetValue(techName, out TechData tech))
        {
            return tech;
        }

        return null;
    }

    /// <summary>
    /// 根据分类获取科技列表
    /// </summary>
    public List<TechData> GetTechsByCategory(TechCategory category)
    {
        return techs.Where(t => t != null && t.category == category).ToList();
    }

    /// <summary>
    /// 根据层级获取科技列表
    /// </summary>
    public List<TechData> GetTechsByTier(int tier)
    {
        return techs.Where(t => t != null && t.tier == tier).ToList();
    }

    /// <summary>
    /// 获取所有科技
    /// </summary>
    public List<TechData> GetAllTechs()
    {
        return techs.Where(t => t != null).ToList();
    }

    /// <summary>
    /// 添加科技
    /// </summary>
    public void AddTech(TechData tech)
    {
        if (tech != null && !techs.Contains(tech))
        {
            techs.Add(tech);

            if (techDictionary != null)
            {
                techDictionary.TryAdd(tech.techName, tech);
            }
        }
    }

    /// <summary>
    /// 移除科技
    /// </summary>
    public void RemoveTech(TechData tech)
    {
        if (tech != null && techs.Contains(tech))
        {
            techs.Remove(tech);

            if (techDictionary != null)
            {
                techDictionary.Remove(tech.techName);
            }
        }
    }

    /// <summary>
    /// 获取科技总数
    /// </summary>
    public int TechCount => techs.Count(t => t != null);
}

/// <summary>
/// 科技树创建器 - 创建预设的科技数据
/// </summary>
public class TechTreeCreator : MonoBehaviour
{
    [Header("科技数据库")]
    [SerializeField] private TechTreeDatabase techDatabase;

    [Header("物品数据库")]
    [SerializeField] private ItemDatabase itemDatabase;

    /// <summary>
    /// 创建所有预设科技
    /// </summary>
    public void CreateAllTechs()
    {
        if (techDatabase == null)
        {
            Debug.LogError("科技数据库未设置！");
            return;
        }

        CreateSurvivalTechs();
        CreateBuildingTechs();
        CreateExplorationTechs();
        CreateCombatTechs();
        CreateAlienTechs();

        Debug.Log("所有科技创建完成！");
    }

    /// <summary>
    /// 创建生存科技
    /// </summary>
    private void CreateSurvivalTechs()
    {
        // 基础采集
        CreateTech("基础采集", "提高采集效率", TechCategory.Survival, 1, 50f,
            new List<string>(), new List<ResourceRequirement>(),
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.StatBonus, stringValue = "采集速度", floatValue = 0.1f }
            });

        // 食物保存
        CreateTech("食物保存", "延长食物保质期", TechCategory.Survival, 1, 75f,
            new List<string> { "基础采集" }, new List<ResourceRequirement>
            {
                new ResourceRequirement { item = GetItem("盐"), amount = 5 }
            },
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.StatBonus, stringValue = "食物保质期", floatValue = 0.5f }
            });

        // 净水技术
        CreateTech("净水技术", "提高净水效率", TechCategory.Survival, 1, 60f,
            new List<string> { "基础采集" }, new List<ResourceRequirement>
            {
                new ResourceRequirement { item = GetItem("木炭"), amount = 3 }
            },
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.StatBonus, stringValue = "净水效率", floatValue = 0.2f }
            });

        // 草药学
        CreateTech("草药学", "解锁草药制作", TechCategory.Survival, 2, 120f,
            new List<string> { "基础采集" }, new List<ResourceRequirement>
            {
                new ResourceRequirement { item = GetItem("草药"), amount = 10 }
            },
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.UnlockRecipe, stringValue = "草药膏" },
                new TechEffect { type = TechEffectType.UnlockRecipe, stringValue = "解毒剂" }
            });

        // 狩猎专家
        CreateTech("狩猎专家", "提高狩猎伤害", TechCategory.Survival, 2, 150f,
            new List<string> { "基础采集" }, new List<ResourceRequirement>
            {
                new ResourceRequirement { item = GetItem("皮革"), amount = 5 }
            },
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.StatBonus, stringValue = "狩猎伤害", floatValue = 0.2f }
            });

        // 高级生存
        CreateTech("高级生存", "解锁高级生存技能", TechCategory.Survival, 3, 300f,
            new List<string> { "草药学", "狩猎专家" }, new List<ResourceRequirement>
            {
                new ResourceRequirement { item = GetItem("草药"), amount = 20 },
                new ResourceRequirement { item = GetItem("皮革"), amount = 10 }
            },
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.StatBonus, stringValue = "生命恢复", floatValue = 0.5f },
                new TechEffect { type = TechEffectType.UnlockRecipe, stringValue = "生命药水" }
            });
    }

    /// <summary>
    /// 创建建造科技
    /// </summary>
    private void CreateBuildingTechs()
    {
        // 基础建造
        CreateTech("基础建造", "解锁基础建筑", TechCategory.Building, 1, 50f,
            new List<string>(), new List<ResourceRequirement>(),
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.UnlockBuilding, stringValue = "木质地基" },
                new TechEffect { type = TechEffectType.UnlockBuilding, stringValue = "木质墙壁" }
            });

        // 石制建筑
        CreateTech("石制建筑", "解锁石制建筑", TechCategory.Building, 1, 80f,
            new List<string> { "基础建造" }, new List<ResourceRequirement>
            {
                new ResourceRequirement { item = GetItem("石头"), amount = 20 }
            },
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.UnlockBuilding, stringValue = "石质墙壁" },
                new TechEffect { type = TechEffectType.UnlockBuilding, stringValue = "石质地基" }
            });

        // 金属冶炼
        CreateTech("金属冶炼", "解锁金属冶炼", TechCategory.Building, 2, 150f,
            new List<string> { "石制建筑" }, new List<ResourceRequirement>
            {
                new ResourceRequirement { item = GetItem("铁矿石"), amount = 10 },
                new ResourceRequirement { item = GetItem("煤炭"), amount = 5 }
            },
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.UnlockRecipe, stringValue = "铁锭" },
                new TechEffect { type = TechEffectType.UnlockBuilding, stringValue = "熔炉" }
            });

        // 高级建筑
        CreateTech("高级建筑", "解锁高级建筑", TechCategory.Building, 2, 200f,
            new List<string> { "金属冶炼" }, new List<ResourceRequirement>
            {
                new ResourceRequirement { item = GetItem("铁锭"), amount = 15 }
            },
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.UnlockBuilding, stringValue = "铁质墙壁" },
                new TechEffect { type = TechEffectType.UnlockBuilding, stringValue = "铁门" },
                new TechEffect { type = TechEffectType.BuildingSpeed, floatValue = 0.2f }
            });

        // 自动化
        CreateTech("自动化", "解锁自动化设备", TechCategory.Building, 3, 400f,
            new List<string> { "高级建筑" }, new List<ResourceRequirement>
            {
                new ResourceRequirement { item = GetItem("铁锭"), amount = 30 },
                new ResourceRequirement { item = GetItem("铜锭"), amount = 10 }
            },
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.UnlockBuilding, stringValue = "传送带" },
                new TechEffect { type = TechEffectType.UnlockBuilding, stringValue = "自动门" },
                new TechEffect { type = TechEffectType.CraftingSpeed, floatValue = 0.3f }
            });
    }

    /// <summary>
    /// 创建探索科技
    /// </summary>
    private void CreateExplorationTechs()
    {
        // 地图绘制
        CreateTech("地图绘制", "解锁地图功能", TechCategory.Exploration, 1, 60f,
            new List<string>(), new List<ResourceRequirement>
            {
                new ResourceRequirement { item = GetItem("纸"), amount = 5 }
            },
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.NewAbility, stringValue = "地图显示" }
            });

        // 攀爬
        CreateTech("攀爬", "解锁攀爬能力", TechCategory.Exploration, 1, 70f,
            new List<string>(), new List<ResourceRequirement>
            {
                new ResourceRequirement { item = GetItem("绳索"), amount = 3 }
            },
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.NewAbility, stringValue = "攀爬悬崖" }
            });

        // 游泳
        CreateTech("游泳", "解锁游泳能力", TechCategory.Exploration, 1, 80f,
            new List<string>(), new List<ResourceRequirement>
            {
                new ResourceRequirement { item = GetItem("皮革"), amount = 5 }
            },
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.NewAbility, stringValue = "游泳" }
            });

        // 航海术
        CreateTech("航海术", "解锁航海能力", TechCategory.Exploration, 2, 200f,
            new List<string> { "游泳" }, new List<ResourceRequirement>
            {
                new ResourceRequirement { item = GetItem("木材"), amount = 30 },
                new ResourceRequirement { item = GetItem("布料"), amount = 10 }
            },
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.UnlockBuilding, stringValue = "木筏" },
                new TechEffect { type = TechEffectType.UnlockRecipe, stringValue = "船帆" }
            });

        // 潜水
        CreateTech("潜水", "解锁潜水能力", TechCategory.Exploration, 2, 180f,
            new List<string> { "游泳" }, new List<ResourceRequirement>
            {
                new ResourceRequirement { item = GetItem("玻璃"), amount = 5 },
                new ResourceRequirement { item = GetItem("铁锭"), amount = 3 }
            },
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.NewAbility, stringValue = "潜水" },
                new TechEffect { type = TechEffectType.UnlockRecipe, stringValue = "潜水镜" }
            });

        // 高级航海
        CreateTech("高级航海", "解锁高级航海", TechCategory.Exploration, 3, 400f,
            new List<string> { "航海术" }, new List<ResourceRequirement>
            {
                new ResourceRequirement { item = GetItem("木材"), amount = 50 },
                new ResourceRequirement { item = GetItem("铁锭"), amount = 20 },
                new ResourceRequirement { item = GetItem("布料"), amount = 15 }
            },
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.UnlockBuilding, stringValue = "帆船" },
                new TechEffect { type = TechEffectType.NewAbility, stringValue = "远洋航行" }
            });
    }

    /// <summary>
    /// 创建战斗科技
    /// </summary>
    private void CreateCombatTechs()
    {
        // 近战精通
        CreateTech("近战精通", "提高近战伤害", TechCategory.Combat, 1, 60f,
            new List<string>(), new List<ResourceRequirement>(),
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.StatBonus, stringValue = "近战伤害", floatValue = 0.15f }
            });

        // 远程精通
        CreateTech("远程精通", "提高远程伤害", TechCategory.Combat, 1, 60f,
            new List<string>(), new List<ResourceRequirement>(),
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.StatBonus, stringValue = "远程伤害", floatValue = 0.15f }
            });

        // 格挡
        CreateTech("格挡", "解锁格挡能力", TechCategory.Combat, 1, 80f,
            new List<string> { "近战精通" }, new List<ResourceRequirement>
            {
                new ResourceRequirement { item = GetItem("木材"), amount = 10 }
            },
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.NewAbility, stringValue = "格挡" },
                new TechEffect { type = TechEffectType.UnlockRecipe, stringValue = "木盾" }
            });

        // 武器锻造
        CreateTech("武器锻造", "解锁高级武器", TechCategory.Combat, 2, 150f,
            new List<string> { "近战精通" }, new List<ResourceRequirement>
            {
                new ResourceRequirement { item = GetItem("铁锭"), amount = 10 }
            },
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.UnlockRecipe, stringValue = "铁剑" },
                new TechEffect { type = TechEffectType.UnlockRecipe, stringValue = "铁矛" }
            });

        // 护甲打造
        CreateTech("护甲打造", "解锁高级护甲", TechCategory.Combat, 2, 150f,
            new List<string> { "格挡" }, new List<ResourceRequirement>
            {
                new ResourceRequirement { item = GetItem("铁锭"), amount = 15 },
                new ResourceRequirement { item = GetItem("皮革"), amount = 10 }
            },
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.UnlockRecipe, stringValue = "铁护甲" },
                new TechEffect { type = TechEffectType.UnlockRecipe, stringValue = "铁头盔" }
            });

        // 战斗大师
        CreateTech("战斗大师", "解锁战斗大师技能", TechCategory.Combat, 3, 350f,
            new List<string> { "武器锻造", "护甲打造" }, new List<ResourceRequirement>
            {
                new ResourceRequirement { item = GetItem("铁锭"), amount = 30 },
                new ResourceRequirement { item = GetItem("皮革"), amount = 20 }
            },
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.StatBonus, stringValue = "暴击率", floatValue = 0.1f },
                new TechEffect { type = TechEffectType.StatBonus, stringValue = "格挡率", floatValue = 0.15f }
            });
    }

    /// <summary>
    /// 创建外星科技
    /// </summary>
    private void CreateAlienTechs()
    {
        // 外星解码
        CreateTech("外星解码", "解读外星文字", TechCategory.Alien, 1, 200f,
            new List<string>(), new List<ResourceRequirement>
            {
                new ResourceRequirement { item = GetItem("外星碎片"), amount = 5 }
            },
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.NewAbility, stringValue = "解读外星文字" }
            });

        // 能量核心
        CreateTech("能量核心", "解锁能量核心", TechCategory.Alien, 2, 400f,
            new List<string> { "外星解码" }, new List<ResourceRequirement>
            {
                new ResourceRequirement { item = GetItem("能量水晶"), amount = 3 },
                new ResourceRequirement { item = GetItem("外星合金"), amount = 5 }
            },
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.UnlockBuilding, stringValue = "能量核心" },
                new TechEffect { type = TechEffectType.UnlockRecipe, stringValue = "能量电池" }
            });

        // 外星合金
        CreateTech("外星合金", "解锁外星合金制作", TechCategory.Alien, 2, 350f,
            new List<string> { "外星解码" }, new List<ResourceRequirement>
            {
                new ResourceRequirement { item = GetItem("外星碎片"), amount = 10 },
                new ResourceRequirement { item = GetItem("铁锭"), amount = 20 }
            },
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.UnlockRecipe, stringValue = "外星合金" },
                new TechEffect { type = TechEffectType.CraftingSpeed, floatValue = 0.2f }
            });

        // 能量武器
        CreateTech("能量武器", "解锁能量武器", TechCategory.Alien, 3, 600f,
            new List<string> { "能量核心", "外星合金" }, new List<ResourceRequirement>
            {
                new ResourceRequirement { item = GetItem("能量水晶"), amount = 5 },
                new ResourceRequirement { item = GetItem("外星合金"), amount = 10 }
            },
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.UnlockRecipe, stringValue = "能量剑" },
                new TechEffect { type = TechEffectType.UnlockRecipe, stringValue = "能量枪" },
                new TechEffect { type = TechEffectType.StatBonus, stringValue = "能量伤害", floatValue = 0.5f }
            });

        // 星际通讯
        CreateTech("星际通讯", "解锁星际通讯", TechCategory.Alien, 3, 800f,
            new List<string> { "能量核心" }, new List<ResourceRequirement>
            {
                new ResourceRequirement { item = GetItem("能量水晶"), amount = 10 },
                new ResourceRequirement { item = GetItem("外星合金"), amount = 15 },
                new ResourceRequirement { item = GetItem("外星芯片"), amount = 3 }
            },
            new List<TechEffect>
            {
                new TechEffect { type = TechEffectType.UnlockBuilding, stringValue = "星际通讯器" },
                new TechEffect { type = TechEffectType.NewAbility, stringValue = "联系外界" }
            });
    }

    /// <summary>
    /// 创建科技
    /// </summary>
    private void CreateTech(string name, string description, TechCategory category, int tier,
        float researchCost, List<string> prerequisites, List<ResourceRequirement> requirements,
        List<TechEffect> effects)
    {
        TechData tech = new TechData
        {
            techName = name,
            techDescription = description,
            category = category,
            tier = tier,
            researchCost = researchCost,
            prerequisites = new List<TechData>(),
            resourceRequirements = requirements,
            effects = effects
        };

        // 设置前置科技
        foreach (string prereqName in prerequisites)
        {
            TechData prereq = techDatabase.GetTechByName(prereqName);
            if (prereq != null)
            {
                tech.prerequisites.Add(prereq);
            }
        }

        techDatabase.AddTech(tech);
    }

    /// <summary>
    /// 获取物品
    /// </summary>
    private ItemData GetItem(string name)
    {
        return itemDatabase?.GetItemByName(name);
    }
}