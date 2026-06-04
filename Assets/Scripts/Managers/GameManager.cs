using UnityEngine;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// 游戏管理器 - 管理游戏状态和流程
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("游戏设置")]
    [SerializeField] private GameDifficulty difficulty = GameDifficulty.Normal;
    [SerializeField] private bool enableMultiplayer = false;

    [Header("游戏状态")]
    [SerializeField] private GameState currentState = GameState.MainMenu;

    [Header("管理器引用")]
    [SerializeField] private WeatherSystem weatherSystem;
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private BuildingDatabase buildingDatabase;

    // 事件
    public UnityEvent<GameState> OnGameStateChanged;
    public UnityEvent<GameDifficulty> OnDifficultyChanged;
    public UnityEvent OnGamePaused;
    public UnityEvent OnGameResumed;
    public UnityEvent OnGameOver;

    // 单例
    public static GameManager Instance { get; private set; }

    // 游戏状态
    private bool isPaused;
    private float playTime;

    private void Awake()
    {
        // 单例模式
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
        InitializeDatabases();
    }

    private void Update()
    {
        if (currentState == GameState.Playing)
        {
            playTime += Time.deltaTime;
            HandlePauseInput();
        }
    }

    /// <summary>
    /// 初始化数据库
    /// </summary>
    private void InitializeDatabases()
    {
        if (itemDatabase != null)
        {
            itemDatabase.Initialize();
        }

        if (buildingDatabase != null)
        {
            buildingDatabase.Initialize();
        }
    }

    /// <summary>
    /// 处理暂停输入
    /// </summary>
    private void HandlePauseInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    /// <summary>
    /// 开始新游戏
    /// </summary>
    public void StartNewGame(GameDifficulty gameDifficulty = GameDifficulty.Normal)
    {
        difficulty = gameDifficulty;
        SetGameState(GameState.Playing);
        playTime = 0f;

        // 初始化游戏世界
        StartCoroutine(InitializeGameWorld());
    }

    /// <summary>
    /// 初始化游戏世界
    /// </summary>
    private IEnumerator InitializeGameWorld()
    {
        // 这里可以添加加载逻辑
        yield return new WaitForSeconds(1f);

        Debug.Log("游戏世界初始化完成");
    }

    /// <summary>
    /// 暂停游戏
    /// </summary>
    public void PauseGame()
    {
        if (currentState != GameState.Playing) return;

        isPaused = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        OnGamePaused?.Invoke();
    }

    /// <summary>
    /// 恢复游戏
    /// </summary>
    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        OnGameResumed?.Invoke();
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    public void GameOver()
    {
        SetGameState(GameState.GameOver);
        OnGameOver?.Invoke();
    }

    /// <summary>
    /// 返回主菜单
    /// </summary>
    public void ReturnToMainMenu()
    {
        SetGameState(GameState.MainMenu);
        Time.timeScale = 1f;
    }

    /// <summary>
    /// 设置游戏状态
    /// </summary>
    public void SetGameState(GameState newState)
    {
        currentState = newState;
        OnGameStateChanged?.Invoke(currentState);
    }

    /// <summary>
    /// 设置游戏难度
    /// </summary>
    public void SetDifficulty(GameDifficulty newDifficulty)
    {
        difficulty = newDifficulty;
        OnDifficultyChanged?.Invoke(difficulty);
    }

    /// <summary>
    /// 获取游戏状态
    /// </summary>
    public GameState CurrentState => currentState;

    /// <summary>
    /// 获取游戏难度
    /// </summary>
    public GameDifficulty Difficulty => difficulty;

    /// <summary>
    /// 是否暂停
    /// </summary>
    public bool IsPaused => isPaused;

    /// <summary>
    /// 获取游戏时间
    /// </summary>
    public float PlayTime => playTime;

    /// <summary>
    /// 获取天气系统
    /// </summary>
    public WeatherSystem WeatherSystem => weatherSystem;

    /// <summary>
    /// 获取物品数据库
    /// </summary>
    public ItemDatabase ItemDatabase => itemDatabase;

    /// <summary>
    /// 获取建筑数据库
    /// </summary>
    public BuildingDatabase BuildingDatabase => buildingDatabase;
}

/// <summary>
/// 游戏状态
/// </summary>
public enum GameState
{
    MainMenu,       // 主菜单
    Playing,        // 游戏中
    Paused,         // 暂停
    GameOver,       // 游戏结束
    Loading         // 加载中
}

/// <summary>
/// 游戏难度
/// </summary>
public enum GameDifficulty
{
    Easy,           // 简单
    Normal,         // 普通
    Hard,           // 困难
    Nightmare       // 噩梦
}