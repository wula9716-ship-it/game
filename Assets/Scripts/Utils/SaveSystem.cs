using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// 保存系统 - 管理游戏存档
/// </summary>
public class SaveSystem : MonoBehaviour
{
    [Header("保存设置")]
    [SerializeField] private string saveFileName = "gamesave.dat";
    [SerializeField] private bool autoSave = true;
    [SerializeField] private float autoSaveInterval = 300f; // 5分钟

    // 保存路径
    private string SavePath => Path.Combine(Application.persistentDataPath, saveFileName);

    // 自动保存计时器
    private float autoSaveTimer;

    // 单例
    public static SaveSystem Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Update()
    {
        if (autoSave)
        {
            autoSaveTimer += Time.deltaTime;
            if (autoSaveTimer >= autoSaveInterval)
            {
                autoSaveTimer = 0;
                SaveGame();
            }
        }
    }

    /// <summary>
    /// 保存游戏
    /// </summary>
    public void SaveGame()
    {
        try
        {
            GameData data = CollectGameData();

            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(SavePath, FileMode.Create);

            formatter.Serialize(stream, data);
            stream.Close();

            Debug.Log($"游戏已保存: {SavePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"保存失败: {e.Message}");
        }
    }

    /// <summary>
    /// 加载游戏
    /// </summary>
    public bool LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("存档文件不存在");
            return false;
        }

        try
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(SavePath, FileMode.Open);

            GameData data = formatter.Deserialize(stream) as GameData;
            stream.Close();

            ApplyGameData(data);

            Debug.Log("游戏已加载");
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"加载失败: {e.Message}");
            return false;
        }
    }

    /// <summary>
    /// 删除存档
    /// </summary>
    public void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("存档已删除");
        }
    }

    /// <summary>
    /// 检查存档是否存在
    /// </summary>
    public bool HasSave()
    {
        return File.Exists(SavePath);
    }

    /// <summary>
    /// 收集游戏数据
    /// </summary>
    private GameData CollectGameData()
    {
        GameData data = new GameData();

        // 玩家数据
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats != null)
        {
            data.playerHealth = playerStats.CurrentHealth;
            data.playerHunger = playerStats.CurrentHunger;
            data.playerThirst = playerStats.CurrentThirst;
            data.playerStamina = playerStats.CurrentStamina;
        }

        // 玩家位置
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            data.playerPosition = playerController.transform.position;
            data.playerRotation = playerController.transform.rotation;
        }

        // 背包数据
        InventorySystem inventory = FindObjectOfType<InventorySystem>();
        if (inventory != null)
        {
            data.inventoryData = CollectInventoryData(inventory);
        }

        // 建筑数据
        BuildingSystem buildingSystem = FindObjectOfType<BuildingSystem>();
        if (buildingSystem != null)
        {
            data.buildingData = CollectBuildingData(buildingSystem);
        }

        // 科技数据
        TechTreeSystem techSystem = TechTreeSystem.Instance;
        if (techSystem != null)
        {
            data.techData = CollectTechData(techSystem);
        }

        // 游戏时间
        data.playTime = GameManager.Instance?.PlayTime ?? 0;
        data.gameDifficulty = GameManager.Instance?.Difficulty ?? GameDifficulty.Normal;

        return data;
    }

    /// <summary>
    /// 应用游戏数据
    /// </summary>
    private void ApplyGameData(GameData data)
    {
        // 玩家数据
        PlayerStats playerStats = FindObjectOfType<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.Heal(data.playerHealth - playerStats.CurrentHealth);
            playerStats.RestoreHunger(data.playerHunger - playerStats.CurrentHunger);
            playerStats.RestoreThirst(data.playerThirst - playerStats.CurrentThirst);
        }

        // 玩家位置
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            playerController.transform.position = data.playerPosition;
            playerController.transform.rotation = data.playerRotation;
        }

        // 背包数据
        InventorySystem inventory = FindObjectOfType<InventorySystem>();
        if (inventory != null && data.inventoryData != null)
        {
            ApplyInventoryData(inventory, data.inventoryData);
        }

        // 建筑数据
        BuildingSystem buildingSystem = FindObjectOfType<BuildingSystem>();
        if (buildingSystem != null && data.buildingData != null)
        {
            ApplyBuildingData(buildingSystem, data.buildingData);
        }

        // 科技数据
        TechTreeSystem techSystem = TechTreeSystem.Instance;
        if (techSystem != null && data.techData != null)
        {
            ApplyTechData(techSystem, data.techData);
        }

        // 游戏难度
        if (GameManager.Instance != null)
        {
            GameManager.Instance.SetDifficulty(data.gameDifficulty);
        }
    }

    /// <summary>
    /// 收集背包数据
    /// </summary>
    private InventoryData CollectInventoryData(InventorySystem inventory)
    {
        InventoryData data = new InventoryData();
        data.slots = new InventorySlotData[inventory.InventorySize];

        for (int i = 0; i < inventory.InventorySize; i++)
        {
            InventorySlot slot = inventory.GetInventorySlot(i);
            if (slot != null && !slot.IsEmpty)
            {
                data.slots[i] = new InventorySlotData
                {
                    itemName = slot.Item.itemName,
                    amount = slot.Amount
                };
            }
            else
            {
                data.slots[i] = new InventorySlotData
                {
                    itemName = "",
                    amount = 0
                };
            }
        }

        return data;
    }

    /// <summary>
    /// 应用背包数据
    /// </summary>
    private void ApplyInventoryData(InventorySystem inventory, InventoryData data)
    {
        ItemDatabase itemDatabase = GameManager.Instance?.ItemDatabase;
        if (itemDatabase == null) return;

        for (int i = 0; i < data.slots.Length && i < inventory.InventorySize; i++)
        {
            InventorySlotData slotData = data.slots[i];
            if (!string.IsNullOrEmpty(slotData.itemName) && slotData.amount > 0)
            {
                ItemData item = itemDatabase.GetItemByName(slotData.itemName);
                if (item != null)
                {
                    InventorySlot slot = inventory.GetInventorySlot(i);
                    slot.SetItem(item, slotData.amount);
                }
            }
        }
    }

    /// <summary>
    /// 收集建筑数据
    /// </summary>
    private BuildingDataCollection CollectBuildingData(BuildingSystem buildingSystem)
    {
        BuildingDataCollection data = new BuildingDataCollection();
        var placedBuildings = buildingSystem.GetPlacedBuildings();
        data.buildings = new BuildingSaveData[placedBuildings.Count];

        for (int i = 0; i < placedBuildings.Count; i++)
        {
            var building = placedBuildings[i];
            data.buildings[i] = new BuildingSaveData
            {
                buildingName = building.data.buildingName,
                position = building.position,
                rotation = building.rotation
            };
        }

        return data;
    }

    /// <summary>
    /// 应用建筑数据
    /// </summary>
    private void ApplyBuildingData(BuildingSystem buildingSystem, BuildingDataCollection data)
    {
        BuildingDatabase buildingDatabase = GameManager.Instance?.BuildingDatabase;
        if (buildingDatabase == null) return;

        foreach (var buildingSave in data.buildings)
        {
            BuildingData buildingData = buildingDatabase.GetBuildingByName(buildingSave.buildingName);
            if (buildingData != null && buildingData.buildingPrefab != null)
            {
                Instantiate(buildingData.buildingPrefab, buildingSave.position, buildingSave.rotation);
            }
        }
    }

    /// <summary>
    /// 收集科技数据
    /// </summary>
    private TechDataCollection CollectTechData(TechTreeSystem techSystem)
    {
        TechDataCollection data = new TechDataCollection();
        var unlockedTechs = techSystem.GetUnlockedTechs();
        data.unlockedTechs = new string[unlockedTechs.Count];

        for (int i = 0; i < unlockedTechs.Count; i++)
        {
            data.unlockedTechs[i] = unlockedTechs[i].techName;
        }

        return data;
    }

    /// <summary>
    /// 应用科技数据
    /// </summary>
    private void ApplyTechData(TechTreeSystem techSystem, TechDataCollection data)
    {
        TechTreeDatabase techDatabase = FindObjectOfType<GameManager>()?.GetComponent<TechTreeDatabase>();
        if (techDatabase == null) return;

        foreach (string techName in data.unlockedTechs)
        {
            TechData tech = techDatabase.GetTechByName(techName);
            if (tech != null)
            {
                techSystem.UnlockTech(tech);
            }
        }
    }
}

/// <summary>
/// 游戏数据
/// </summary>
[System.Serializable]
public class GameData
{
    // 玩家数据
    public float playerHealth;
    public float playerHunger;
    public float playerThirst;
    public float playerStamina;
    public Vector3 playerPosition;
    public Quaternion playerRotation;

    // 背包数据
    public InventoryData inventoryData;

    // 建筑数据
    public BuildingDataCollection buildingData;

    // 科技数据
    public TechDataCollection techData;

    // 游戏数据
    public float playTime;
    public GameDifficulty gameDifficulty;
}

/// <summary>
/// 背包数据
/// </summary>
[System.Serializable]
public class InventoryData
{
    public InventorySlotData[] slots;
}

/// <summary>
/// 背包槽位数据
/// </summary>
[System.Serializable]
public class InventorySlotData
{
    public string itemName;
    public int amount;
}

/// <summary>
/// 建筑数据集合
/// </summary>
[System.Serializable]
public class BuildingDataCollection
{
    public BuildingSaveData[] buildings;
}

/// <summary>
/// 建筑保存数据
/// </summary>
[System.Serializable]
public class BuildingSaveData
{
    public string buildingName;
    public Vector3 position;
    public Quaternion rotation;
}

/// <summary>
/// 科技数据集合
/// </summary>
[System.Serializable]
public class TechDataCollection
{
    public string[] unlockedTechs;
}