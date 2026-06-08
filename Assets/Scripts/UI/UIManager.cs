using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// UI管理器 - 管理所有UI界面
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("UI面板")]
    [SerializeField] private GameObject hudPanel;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject buildingPanel;
    [SerializeField] private GameObject craftingPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;

    [Header("UI组件")]
    [SerializeField] private GameHUD gameHUD;
    [SerializeField] private InventoryUI inventoryUI;
    [SerializeField] private BuildingUI buildingUI;
    [SerializeField] private CraftingUI craftingUI;

    // UI状态
    private Dictionary<string, GameObject> uiPanels = new Dictionary<string, GameObject>();
    private Stack<GameObject> panelStack = new Stack<GameObject>();
    private bool isUIOpen;

    // 单例
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeUI();
    }

    private void Start()
    {
        // 注册事件
        RegisterEvents();
    }

    private void Update()
    {
        HandleUIInput();
    }

    /// <summary>
    /// 初始化UI
    /// </summary>
    private void InitializeUI()
    {
        // 注册UI面板
        RegisterPanel("HUD", hudPanel);
        RegisterPanel("Inventory", inventoryPanel);
        RegisterPanel("Building", buildingPanel);
        RegisterPanel("Crafting", craftingPanel);
        RegisterPanel("Pause", pausePanel);
        RegisterPanel("GameOver", gameOverPanel);

        // 初始状态：只显示HUD
        ShowHUD();
    }

    /// <summary>
    /// 注册事件
    /// </summary>
    private void RegisterEvents()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameStateChanged.AddListener(OnGameStateChanged);
            GameManager.Instance.OnGamePaused.AddListener(OnGamePaused);
            GameManager.Instance.OnGameResumed.AddListener(OnGameResumed);
            GameManager.Instance.OnGameOver.AddListener(OnGameOver);
        }
    }

    /// <summary>
    /// 处理UI输入
    /// </summary>
    private void HandleUIInput()
    {
        // Tab键打开/关闭背包
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleInventory();
        }

        // B键打开/关闭建造界面
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleBuilding();
        }

        // C键打开/关闭制作界面
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCrafting();
        }

        // Escape键关闭当前UI或暂停
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (panelStack.Count > 0)
            {
                CloseTopPanel();
            }
            else
            {
                GameManager.Instance?.PauseGame();
            }
        }
    }

    /// <summary>
    /// 注册UI面板
    /// </summary>
    public void RegisterPanel(string panelName, GameObject panel)
    {
        if (panel != null && !uiPanels.ContainsKey(panelName))
        {
            uiPanels.Add(panelName, panel);
            panel.SetActive(false);
        }
    }

    /// <summary>
    /// 显示UI面板
    /// </summary>
    public void ShowPanel(string panelName)
    {
        if (uiPanels.TryGetValue(panelName, out GameObject panel))
        {
            panel.SetActive(true);
            panelStack.Push(panel);
            isUIOpen = true;

            // 更新鼠标状态
            UpdateCursorState();
        }
    }

    /// <summary>
    /// 隐藏UI面板
    /// </summary>
    public void HidePanel(string panelName)
    {
        if (uiPanels.TryGetValue(panelName, out GameObject panel))
        {
            panel.SetActive(false);

            // 从栈中移除
            if (panelStack.Count > 0 && panelStack.Peek() == panel)
            {
                panelStack.Pop();
            }

            // 更新UI状态
            isUIOpen = panelStack.Count > 0;
            UpdateCursorState();
        }
    }

    /// <summary>
    /// 关闭顶部面板
    /// </summary>
    public void CloseTopPanel()
    {
        if (panelStack.Count > 0)
        {
            GameObject topPanel = panelStack.Pop();
            topPanel.SetActive(false);

            isUIOpen = panelStack.Count > 0;
            UpdateCursorState();
        }
    }

    /// <summary>
    /// 关闭所有面板
    /// </summary>
    public void CloseAllPanels()
    {
        foreach (var panel in uiPanels.Values)
        {
            panel.SetActive(false);
        }

        panelStack.Clear();
        isUIOpen = false;
        UpdateCursorState();
    }

    /// <summary>
    /// 显示HUD
    /// </summary>
    public void ShowHUD()
    {
        if (hudPanel != null)
        {
            hudPanel.SetActive(true);
        }
    }

    /// <summary>
    /// 隐藏HUD
    /// </summary>
    public void HideHUD()
    {
        if (hudPanel != null)
        {
            hudPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 切换背包界面
    /// </summary>
    public void ToggleInventory()
    {
        if (IsPanelOpen("Inventory"))
        {
            HidePanel("Inventory");
        }
        else
        {
            ShowPanel("Inventory");
            inventoryUI?.RefreshInventory();
        }
    }

    /// <summary>
    /// 切换建造界面
    /// </summary>
    public void ToggleBuilding()
    {
        if (IsPanelOpen("Building"))
        {
            HidePanel("Building");
        }
        else
        {
            ShowPanel("Building");
            buildingUI?.RefreshBuildings();
        }
    }

    /// <summary>
    /// 切换制作界面
    /// </summary>
    public void ToggleCrafting()
    {
        if (IsPanelOpen("Crafting"))
        {
            HidePanel("Crafting");
        }
        else
        {
            ShowPanel("Crafting");
            craftingUI?.RefreshRecipes();
        }
    }

    /// <summary>
    /// 检查面板是否打开
    /// </summary>
    public bool IsPanelOpen(string panelName)
    {
        if (uiPanels.TryGetValue(panelName, out GameObject panel))
        {
            return panel.activeSelf;
        }
        return false;
    }

    /// <summary>
    /// 更新鼠标状态
    /// </summary>
    private void UpdateCursorState()
    {
        if (isUIOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    /// <summary>
    /// 游戏状态改变回调
    /// </summary>
    private void OnGameStateChanged(GameState newState)
    {
        switch (newState)
        {
            case GameState.MainMenu:
                CloseAllPanels();
                HideHUD();
                break;
            case GameState.Playing:
                ShowHUD();
                break;
            case GameState.GameOver:
                ShowPanel("GameOver");
                break;
        }
    }

    /// <summary>
    /// 游戏暂停回调
    /// </summary>
    private void OnGamePaused()
    {
        ShowPanel("Pause");
    }

    /// <summary>
    /// 游戏恢复回调
    /// </summary>
    private void OnGameResumed()
    {
        HidePanel("Pause");
    }

    /// <summary>
    /// 游戏结束回调
    /// </summary>
    private void OnGameOver()
    {
        ShowPanel("GameOver");
    }

    // 属性访问器
    public bool IsUIOpen => isUIOpen;
    public GameHUD GameHUD => gameHUD;
    public InventoryUI InventoryUI => inventoryUI;
    public BuildingUI BuildingUI => buildingUI;
    public CraftingUI CraftingUI => craftingUI;
}