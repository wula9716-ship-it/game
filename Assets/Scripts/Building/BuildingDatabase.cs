using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 建筑数据库 - 存储所有建筑数据
/// </summary>
[CreateAssetMenu(fileName = "BuildingDatabase", menuName = "Survival Island/Building Database")]
public class BuildingDatabase : ScriptableObject
{
    [Header("建筑列表")]
    [SerializeField] private List<BuildingData> buildings = new List<BuildingData>();

    // 建筑字典（用于快速查找）
    private Dictionary<string, BuildingData> buildingDictionary;

    /// <summary>
    /// 初始化数据库
    /// </summary>
    public void Initialize()
    {
        buildingDictionary = new Dictionary<string, BuildingData>();

        foreach (var building in buildings)
        {
            if (building != null && !buildingDictionary.ContainsKey(building.buildingName))
            {
                buildingDictionary.Add(building.buildingName, building);
            }
        }
    }

    /// <summary>
    /// 根据名称获取建筑
    /// </summary>
    public BuildingData GetBuildingByName(string buildingName)
    {
        if (buildingDictionary == null)
        {
            Initialize();
        }

        if (buildingDictionary.TryGetValue(buildingName, out BuildingData building))
        {
            return building;
        }

        Debug.LogWarning($"建筑未找到: {buildingName}");
        return null;
    }

    /// <summary>
    /// 根据类型获取建筑列表
    /// </summary>
    public List<BuildingData> GetBuildingsByType(BuildingType type)
    {
        return buildings.Where(b => b != null && b.buildingType == type).ToList();
    }

    /// <summary>
    /// 根据分类获取建筑列表
    /// </summary>
    public List<BuildingData> GetBuildingsByCategory(BuildingCategory category)
    {
        return buildings.Where(b => b != null && b.buildingCategory == category).ToList();
    }

    /// <summary>
    /// 获取所有建筑
    /// </summary>
    public List<BuildingData> GetAllBuildings()
    {
        return buildings.Where(b => b != null).ToList();
    }

    /// <summary>
    /// 获取建筑总数
    /// </summary>
    public int BuildingCount => buildings.Count(b => b != null);

    /// <summary>
    /// 添加建筑到数据库
    /// </summary>
    public void AddBuilding(BuildingData building)
    {
        if (building != null && !buildings.Contains(building))
        {
            buildings.Add(building);

            if (buildingDictionary != null)
            {
                buildingDictionary.TryAdd(building.buildingName, building);
            }
        }
    }

    /// <summary>
    /// 从数据库移除建筑
    /// </summary>
    public void RemoveBuilding(BuildingData building)
    {
        if (building != null && buildings.Contains(building))
        {
            buildings.Remove(building);

            if (buildingDictionary != null)
            {
                buildingDictionary.Remove(building.buildingName);
            }
        }
    }
}