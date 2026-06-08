using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

/// <summary>
/// 调试控制台 - 游戏内调试工具
/// </summary>
public class DebugConsole : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private GameObject consolePanel;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TextMeshProUGUI outputText;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private Button submitButton;
    [SerializeField] private Button clearButton;

    [Header("设置")]
    [SerializeField] private int maxLines = 100;
    [SerializeField] private KeyCode toggleKey = KeyCode.BackQuote;

    // 命令历史
    private List<string> commandHistory = new List<string>();
    private int historyIndex = -1;

    // 命令处理器
    private Dictionary<string, CommandHandler> commands = new Dictionary<string, CommandHandler>();

    // 单例
    public static DebugConsole Instance { get; private set; }

    // 状态
    private bool isOpen = false;

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

        RegisterCommands();
    }

    private void Start()
    {
        // 注册UI事件
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(SubmitCommand);
        }

        if (clearButton != null)
        {
            clearButton.onClick.AddListener(ClearConsole);
        }

        if (inputField != null)
        {
            inputField.onSubmit.AddListener(OnInputSubmit);
        }

        // 隐藏控制台
        if (consolePanel != null)
        {
            consolePanel.SetActive(false);
        }
    }

    private void Update()
    {
        // 切换控制台显示
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleConsole();
        }

        // 命令历史导航
        if (isOpen && commandHistory.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                NavigateHistory(-1);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                NavigateHistory(1);
            }
        }
    }

    /// <summary>
    /// 注册命令
    /// </summary>
    private void RegisterCommands()
    {
        // 帮助命令
        RegisterCommand("help", "显示所有命令", (args) =>
        {
            Log("可用命令:");
            foreach (var cmd in commands)
            {
                Log($"  {cmd.Key} - {cmd.Value.description}");
            }
        });

        // 清除命令
        RegisterCommand("clear", "清除控制台", (args) =>
        {
            ClearConsole();
        });

        // 退出命令
        RegisterCommand("quit", "退出游戏", (args) =>
        {
            Application.Quit();
        });

        // 帧率命令
        RegisterCommand("fps", "显示帧率", (args) =>
        {
            float fps = 1f / Time.unscaledDeltaTime;
            Log($"FPS: {fps:F1}");
        });

        // 内存命令
        RegisterCommand("memory", "显示内存使用", (args) =>
        {
            long memory = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024);
            Log($"内存使用: {memory} MB");
        });

        // 时间命令
        RegisterCommand("time", "设置时间", (args) =>
        {
            if (args.Length > 0 && float.TryParse(args[0], out float time))
            {
                // 设置游戏时间
                Log($"时间设置为: {time}");
            }
            else
            {
                Log("用法: time <0-1>");
            }
        });

        // 天气命令
        RegisterCommand("weather", "设置天气", (args) =>
        {
            if (args.Length > 0)
            {
                Log($"天气设置为: {args[0]}");
            }
            else
            {
                Log("用法: weather <clear|rain|storm|fog>");
            }
        });

        // 传送命令
        RegisterCommand("tp", "传送玩家", (args) =>
        {
            if (args.Length >= 3 && 
                float.TryParse(args[0], out float x) &&
                float.TryParse(args[1], out float y) &&
                float.TryParse(args[2], out float z))
            {
                PlayerController player = FindObjectOfType<PlayerController>();
                if (player != null)
                {
                    player.transform.position = new Vector3(x, y, z);
                    Log($"传送到: ({x}, {y}, {z})");
                }
            }
            else
            {
                Log("用法: tp <x> <y> <z>");
            }
        });

        // 生命值命令
        RegisterCommand("heal", "恢复生命值", (args) =>
        {
            PlayerStats player = FindObjectOfType<PlayerStats>();
            if (player != null)
            {
                if (args.Length > 0 && float.TryParse(args[0], out float amount))
                {
                    player.Heal(amount);
                    Log($"恢复生命值: {amount}");
                }
                else
                {
                    player.Heal(player.MaxHealth);
                    Log("生命值已满");
                }
            }
        });

        // 饥饿值命令
        RegisterCommand("feed", "恢复饥饿值", (args) =>
        {
            PlayerStats player = FindObjectOfType<PlayerStats>();
            if (player != null)
            {
                if (args.Length > 0 && float.TryParse(args[0], out float amount))
                {
                    player.RestoreHunger(amount);
                    Log($"恢复饥饿值: {amount}");
                }
                else
                {
                    player.RestoreHunger(player.MaxHunger);
                    Log("饥饿值已满");
                }
            }
        });

        // 口渴值命令
        RegisterCommand("drink", "恢复口渴值", (args) =>
        {
            PlayerStats player = FindObjectOfType<PlayerStats>();
            if (player != null)
            {
                if (args.Length > 0 && float.TryParse(args[0], out float amount))
                {
                    player.RestoreThirst(amount);
                    Log($"恢复口渴值: {amount}");
                }
                else
                {
                    player.RestoreThirst(player.MaxThirst);
                    Log("口渴值已满");
                }
            }
        });

        // 无敌命令
        RegisterCommand("god", "切换无敌模式", (args) =>
        {
            Log("无敌模式切换");
            // TODO: 实现无敌模式
        });

        // 飞行命令
        RegisterCommand("fly", "切换飞行模式", (args) =>
        {
            Log("飞行模式切换");
            // TODO: 实现飞行模式
        });

        // 物品命令
        RegisterCommand("give", "添加物品", (args) =>
        {
            if (args.Length >= 1)
            {
                string itemName = args[0];
                int amount = 1;
                if (args.Length >= 2 && int.TryParse(args[1], out int parsedAmount))
                {
                    amount = parsedAmount;
                }

                InventorySystem inventory = FindObjectOfType<InventorySystem>();
                if (inventory != null)
                {
                    ItemDatabase database = GameManager.Instance?.ItemDatabase;
                    if (database != null)
                    {
                        ItemData item = database.GetItemByName(itemName);
                        if (item != null)
                        {
                            inventory.AddItem(item, amount);
                            Log($"添加物品: {itemName} x{amount}");
                        }
                        else
                        {
                            Log($"物品未找到: {itemName}");
                        }
                    }
                }
            }
            else
            {
                Log("用法: give <物品名称> [数量]");
            }
        });

        // 科技命令
        RegisterCommand("tech", "解锁科技", (args) =>
        {
            if (args.Length > 0)
            {
                string techName = args[0];
                TechTreeSystem techSystem = TechTreeSystem.Instance;
                if (techSystem != null)
                {
                    // 解锁科技
                    Log($"解锁科技: {techName}");
                }
            }
            else
            {
                Log("用法: tech <科技名称>");
            }
        });

        // 生成敌人命令
        RegisterCommand("spawn", "生成敌人", (args) =>
        {
            if (args.Length > 0)
            {
                string enemyName = args[0];
                Log($"生成敌人: {enemyName}");
                // TODO: 实现敌人生成
            }
            else
            {
                Log("用法: spawn <敌人名称>");
            }
        });

        // 保存命令
        RegisterCommand("save", "保存游戏", (args) =>
        {
            SaveSystem saveSystem = SaveSystem.Instance;
            if (saveSystem != null)
            {
                saveSystem.SaveGame();
                Log("游戏已保存");
            }
        });

        // 加载命令
        RegisterCommand("load", "加载游戏", (args) =>
        {
            SaveSystem saveSystem = SaveSystem.Instance;
            if (saveSystem != null)
            {
                if (saveSystem.LoadGame())
                {
                    Log("游戏已加载");
                }
                else
                {
                    Log("加载失败");
                }
            }
        });

        // 难度命令
        RegisterCommand("difficulty", "设置难度", (args) =>
        {
            if (args.Length > 0)
            {
                string difficultyName = args[0].ToLower();
                GameDifficulty difficulty;

                switch (difficultyName)
                {
                    case "easy":
                        difficulty = GameDifficulty.Easy;
                        break;
                    case "normal":
                        difficulty = GameDifficulty.Normal;
                        break;
                    case "hard":
                        difficulty = GameDifficulty.Hard;
                        break;
                    case "nightmare":
                        difficulty = GameDifficulty.Nightmare;
                        break;
                    default:
                        Log("用法: difficulty <easy|normal|hard|nightmare>");
                        return;
                }

                GameManager.Instance?.SetDifficulty(difficulty);
                Log($"难度设置为: {difficultyName}");
            }
            else
            {
                Log("用法: difficulty <easy|normal|hard|nightmare>");
            }
        });
    }

    /// <summary>
    /// 注册命令
    /// </summary>
    public void RegisterCommand(string command, string description, Action<string[]> handler)
    {
        commands[command.ToLower()] = new CommandHandler
        {
            description = description,
            handler = handler
        };
    }

    /// <summary>
    /// 切换控制台显示
    /// </summary>
    public void ToggleConsole()
    {
        isOpen = !isOpen;

        if (consolePanel != null)
        {
            consolePanel.SetActive(isOpen);
        }

        if (isOpen)
        {
            // 聚焦输入框
            if (inputField != null)
            {
                inputField.Select();
                inputField.ActivateInputField();
            }

            // 解锁鼠标
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // 恢复鼠标状态
            if (UIManager.Instance != null && !UIManager.Instance.IsUIOpen)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }

    /// <summary>
    /// 提交命令
    /// </summary>
    public void SubmitCommand()
    {
        if (inputField == null) return;

        string input = inputField.text.Trim();
        if (string.IsNullOrEmpty(input)) return;

        // 添加到历史
        commandHistory.Add(input);
        historyIndex = commandHistory.Count;

        // 显示命令
        Log($"> {input}");

        // 解析并执行命令
        ExecuteCommand(input);

        // 清空输入框
        inputField.text = "";
        inputField.Select();
        inputField.ActivateInputField();
    }

    /// <summary>
    /// 输入提交回调
    /// </summary>
    private void OnInputSubmit(string input)
    {
        SubmitCommand();
    }

    /// <summary>
    /// 执行命令
    /// </summary>
    private void ExecuteCommand(string input)
    {
        string[] parts = input.Split(' ');
        string commandName = parts[0].ToLower();
        string[] args = new string[parts.Length - 1];
        Array.Copy(parts, 1, args, 0, args.Length);

        if (commands.TryGetValue(commandName, out CommandHandler handler))
        {
            try
            {
                handler.handler.Invoke(args);
            }
            catch (Exception e)
            {
                LogError($"命令执行错误: {e.Message}");
            }
        }
        else
        {
            LogError($"未知命令: {commandName}");
        }
    }

    /// <summary>
    /// 导航命令历史
    /// </summary>
    private void NavigateHistory(int direction)
    {
        historyIndex += direction;
        historyIndex = Mathf.Clamp(historyIndex, 0, commandHistory.Count - 1);

        if (historyIndex >= 0 && historyIndex < commandHistory.Count)
        {
            inputField.text = commandHistory[historyIndex];
            inputField.caretPosition = inputField.text.Length;
        }
    }

    /// <summary>
    /// 记录日志
    /// </summary>
    public void Log(string message)
    {
        if (outputText == null) return;

        outputText.text += message + "\n";

        // 限制行数
        string[] lines = outputText.text.Split('\n');
        if (lines.Length > maxLines)
        {
            outputText.text = string.Join("\n", lines, lines.Length - maxLines, maxLines);
        }

        // 滚动到底部
        if (scrollRect != null)
        {
            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }

    /// <summary>
    /// 记录错误日志
    /// </summary>
    public void LogError(string message)
    {
        Log($"<color=red>{message}</color>");
    }

    /// <summary>
    /// 记录警告日志
    /// </summary>
    public void LogWarning(string message)
    {
        Log($"<color=yellow>{message}</color>");
    }

    /// <summary>
    /// 清除控制台
    /// </summary>
    public void ClearConsole()
    {
        if (outputText != null)
        {
            outputText.text = "";
        }
    }

    /// <summary>
    /// 是否打开
    /// </summary>
    public bool IsOpen => isOpen;
}

/// <summary>
/// 命令处理器
/// </summary>
public class CommandHandler
{
    public string description;
    public Action<string[]> handler;
}

/// <summary>
/// 性能监控UI - 显示性能数据
/// </summary>
public class PerformanceMonitorUI : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private TextMeshProUGUI fpsText;
    [SerializeField] private TextMeshProUGUI memoryText;
    [SerializeField] private TextMeshProUGUI drawCallsText;
    [SerializeField] private Slider fpsGraph;

    [Header("设置")]
    [SerializeField] private float updateInterval = 0.5f;
    [SerializeField] private bool showGraph = true;

    // 性能数据
    private float[] fpsHistory = new float[60];
    private int fpsIndex = 0;
    private float updateTimer;

    private void Update()
    {
        updateTimer += Time.deltaTime;

        if (updateTimer >= updateInterval)
        {
            updateTimer = 0;
            UpdateDisplay();
        }
    }

    /// <summary>
    /// 更新显示
    /// </summary>
    private void UpdateDisplay()
    {
        PerformanceOptimizer optimizer = PerformanceOptimizer.Instance;
        if (optimizer == null) return;

        // 更新FPS
        float fps = optimizer.GetFPS();
        if (fpsText != null)
        {
            fpsText.text = $"FPS: {fps:F1}";
            fpsText.color = GetFPSColor(fps);
        }

        // 更新内存
        long memory = optimizer.GetMemoryUsage();
        if (memoryText != null)
        {
            memoryText.text = $"内存: {memory} MB";
        }

        // 更新FPS图表
        if (showGraph && fpsGraph != null)
        {
            fpsHistory[fpsIndex] = fps;
            fpsIndex = (fpsIndex + 1) % fpsHistory.Length;

            float avgFPS = 0;
            for (int i = 0; i < fpsHistory.Length; i++)
            {
                avgFPS += fpsHistory[i];
            }
            avgFPS /= fpsHistory.Length;

            fpsGraph.value = avgFPS / 60f;
        }
    }

    /// <summary>
    /// 获取FPS颜色
    /// </summary>
    private Color GetFPSColor(float fps)
    {
        if (fps >= 50)
            return Color.green;
        else if (fps >= 30)
            return Color.yellow;
        else
            return Color.red;
    }
}