using UnityEngine;

/// <summary>
/// 测试游戏管理器 - 简化版本，用于快速测试
/// </summary>
public class TestGameManager : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private bool enableDebugConsole = true;
    [SerializeField] private bool enableQuickSetup = true;
    [SerializeField] private bool giveStarterItems = true;

    [Header("数据库")]
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private BuildingDatabase buildingDatabase;

    // 单例
    public static TestGameManager Instance { get; private set; }

    // 属性
    public ItemDatabase ItemDatabase => itemDatabase;
    public BuildingDatabase BuildingDatabase => buildingDatabase;

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

        // 初始化数据库
        if (itemDatabase != null)
        {
            itemDatabase.Initialize();
        }

        if (buildingDatabase != null)
        {
            buildingDatabase.Initialize();
        }
    }

    private void Start()
    {
        if (enableQuickSetup)
        {
            // 添加快速测试设置
            if (GetComponent<QuickTestSetup>() == null)
            {
                gameObject.AddComponent<QuickTestSetup>();
            }
        }

        if (enableDebugConsole)
        {
            // 添加调试控制台
            if (FindObjectOfType<DebugConsole>() == null)
            {
                GameObject consoleObj = new GameObject("DebugConsole");
                consoleObj.AddComponent<DebugConsole>();
            }
        }

        Debug.Log("测试游戏管理器初始化完成");
        Debug.Log("按 ~ 键打开调试控制台");
        Debug.Log("输入 help 查看所有可用命令");
    }

    /// <summary>
    /// 获取物品数据库
    /// </summary>
    public ItemDatabase GetItemDatabase()
    {
        return itemDatabase;
    }

    /// <summary>
    /// 获取建筑数据库
    /// </summary>
    public BuildingDatabase GetBuildingDatabase()
    {
        return buildingDatabase;
    }
}