using UnityEngine;

/// <summary>
/// 快速测试设置 - 自动创建测试环境
/// </summary>
public class QuickTestSetup : MonoBehaviour
{
    [Header("测试设置")]
    [SerializeField] private bool autoSetup = true;
    [SerializeField] private bool spawnPlayer = true;
    [SerializeField] private bool spawnGround = true;
    [SerializeField] private bool spawnLight = true;
    [SerializeField] private bool spawnUI = true;

    [Header("玩家设置")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Vector3 playerSpawnPosition = new Vector3(0, 1, 0);

    [Header("地面设置")]
    [SerializeField] private Vector3 groundScale = new Vector3(20, 1, 20);
    [SerializeField] private Material groundMaterial;

    [Header("测试物品")]
    [SerializeField] private bool giveStarterItems = true;

    private void Start()
    {
        if (autoSetup)
        {
            SetupTestEnvironment();
        }
    }

    /// <summary>
    /// 设置测试环境
    /// </summary>
    public void SetupTestEnvironment()
    {
        Debug.Log("开始设置测试环境...");

        if (spawnGround)
        {
            CreateGround();
        }

        if (spawnLight)
        {
            CreateLight();
        }

        if (spawnPlayer)
        {
            CreatePlayer();
        }

        if (spawnUI)
        {
            CreateUI();
        }

        if (giveStarterItems)
        {
            GiveStarterItems();
        }

        Debug.Log("测试环境设置完成！");
    }

    /// <summary>
    /// 创建地面
    /// </summary>
    private void CreateGround()
    {
        // 检查是否已存在地面
        if (GameObject.Find("Ground") != null)
            return;

        GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ground.name = "Ground";
        ground.transform.position = Vector3.zero;
        ground.transform.localScale = groundScale;

        // 设置 Layer
        ground.layer = LayerMask.NameToLayer("Default");

        // 设置材质
        if (groundMaterial != null)
        {
            ground.GetComponent<Renderer>().material = groundMaterial;
        }
        else
        {
            // 创建简单的草地材质
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.3f, 0.6f, 0.3f);
            ground.GetComponent<Renderer>().material = mat;
        }

        Debug.Log("地面创建完成");
    }

    /// <summary>
    /// 创建灯光
    /// </summary>
    private void CreateLight()
    {
        // 检查是否已存在灯光
        if (GameObject.Find("Directional Light") != null)
            return;

        GameObject lightObj = new GameObject("Directional Light");
        Light light = lightObj.AddComponent<Light>();
        light.type = LightType.Directional;
        light.color = Color.white;
        light.intensity = 1f;
        lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);

        Debug.Log("灯光创建完成");
    }

    /// <summary>
    /// 创建玩家
    /// </summary>
    private void CreatePlayer()
    {
        // 检查是否已存在玩家
        if (GameObject.FindGameObjectWithTag("Player") != null)
            return;

        GameObject player;

        if (playerPrefab != null)
        {
            player = Instantiate(playerPrefab, playerSpawnPosition, Quaternion.identity);
        }
        else
        {
            // 创建简单的玩家对象
            player = new GameObject("Player");
            player.transform.position = playerSpawnPosition;

            // 添加胶囊体（视觉表示）
            GameObject capsule = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            capsule.transform.SetParent(player.transform);
            capsule.transform.localPosition = Vector3.zero;
            capsule.GetComponent<Renderer>().material.color = Color.blue;

            // 添加 Character Controller
            CharacterController controller = player.AddComponent<CharacterController>();
            controller.height = 2f;
            controller.radius = 0.5f;

            // 添加玩家脚本
            player.AddComponent<PlayerController>();
            player.AddComponent<PlayerStats>();
            player.AddComponent<InventorySystem>();

            // 设置 Tag
            player.tag = "Player";

            // 添加相机
            GameObject cameraObj = new GameObject("PlayerCamera");
            cameraObj.transform.SetParent(player.transform);
            cameraObj.transform.localPosition = new Vector3(0, 1.6f, 0);
            Camera camera = cameraObj.AddComponent<Camera>();
            camera.fieldOfView = 60f;
        }

        Debug.Log("玩家创建完成");
    }

    /// <summary>
    /// 创建 UI
    /// </summary>
    private void CreateUI()
    {
        // 检查是否已存在 Canvas
        if (GameObject.Find("Canvas") != null)
            return;

        // 创建 Canvas
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

        // 添加 UI 管理器
        canvasObj.AddComponent<UIManager>();

        // 创建 EventSystem
        if (GameObject.Find("EventSystem") == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        }

        Debug.Log("UI 创建完成");
    }

    /// <summary>
    /// 给予初始物品
    /// </summary>
    private void GiveStarterItems()
    {
        InventorySystem inventory = FindObjectOfType<InventorySystem>();
        if (inventory == null) return;

        // 获取物品数据库
        ItemDatabase database = FindObjectOfType<GameManager>()?.ItemDatabase;
        if (database == null)
        {
            // 如果没有数据库，创建简单的测试物品
            Debug.LogWarning("物品数据库未找到，跳过初始物品");
            return;
        }

        // 给予初始物品
        GiveItem(inventory, database, "木材", 20);
        GiveItem(inventory, database, "石头", 10);
        GiveItem(inventory, database, "浆果", 5);
        GiveItem(inventory, database, "水", 3);

        Debug.Log("初始物品给予完成");
    }

    /// <summary>
    /// 给予物品
    /// </summary>
    private void GiveItem(InventorySystem inventory, ItemDatabase database, string itemName, int amount)
    {
        ItemData item = database.GetItemByName(itemName);
        if (item != null)
        {
            inventory.AddItem(item, amount);
        }
    }

    /// <summary>
    /// 重置测试环境
    /// </summary>
    public void ResetTestEnvironment()
    {
        // 删除所有测试对象
        Destroy(GameObject.Find("Ground"));
        Destroy(GameObject.Find("Directional Light"));
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        Destroy(GameObject.Find("Canvas"));
        Destroy(GameObject.Find("EventSystem"));

        // 重新创建
        SetupTestEnvironment();
    }
}