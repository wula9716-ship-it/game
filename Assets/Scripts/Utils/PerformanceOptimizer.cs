using UnityEngine;
using UnityEngine.Profiling;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 性能优化器 - 优化游戏性能
/// </summary>
public class PerformanceOptimizer : MonoBehaviour
{
    [Header("渲染优化")]
    [SerializeField] private bool enableOcclusionCulling = true;
    [SerializeField] private bool enableLOD = true;
    [SerializeField] private int maxShadowCascades = 2;
    [SerializeField] private float shadowDistance = 50f;

    [Header("内存优化")]
    [SerializeField] private bool enableObjectPooling = true;
    [SerializeField] private bool enableAssetUnloading = true;
    [SerializeField] private float unloadInterval = 60f;

    [Header("帧率优化")]
    [SerializeField] private int targetFrameRate = 60;
    [SerializeField] private bool enableVSync = false;
    [SerializeField] private bool enableAdaptivePerformance = true;

    [Header("LOD设置")]
    [SerializeField] private float lodBias = 1f;
    [SerializeField] private float maximumLODLevel = 0;

    [Header("性能监控")]
    [SerializeField] private bool enablePerformanceMonitoring = true;
    [SerializeField] private float monitoringInterval = 1f;

    // 性能数据
    private float fps;
    private float frameTime;
    private long memoryUsage;
    private int drawCalls;
    private int triangles;

    // 监控计时器
    private float monitoringTimer;

    // 单例
    public static PerformanceOptimizer Instance { get; private set; }

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

        ApplyOptimizations();
    }

    private void Update()
    {
        if (enablePerformanceMonitoring)
        {
            UpdatePerformanceMonitoring();
        }
    }

    /// <summary>
    /// 应用优化设置
    /// </summary>
    private void ApplyOptimizations()
    {
        // 帧率设置
        Application.targetFrameRate = targetFrameRate;
        QualitySettings.vSyncCount = enableVSync ? 1 : 0;

        // 渲染优化
        QualitySettings.shadowCascades = maxShadowCascades;
        QualitySettings.shadowDistance = shadowDistance;
        QualitySettings.lodBias = lodBias;
        QualitySettings.maximumLODLevel = (int)maximumLODLevel;

        // 遮挡剔除
        if (enableOcclusionCulling)
        {
            // 遮挡剔除在Unity Editor中设置
        }

        // 开始内存优化协程
        if (enableAssetUnloading)
        {
            StartCoroutine(MemoryOptimizationCoroutine());
        }
    }

    /// <summary>
    /// 更新性能监控
    /// </summary>
    private void UpdatePerformanceMonitoring()
    {
        monitoringTimer += Time.deltaTime;

        if (monitoringTimer >= monitoringInterval)
        {
            monitoringTimer = 0;

            // 计算FPS
            fps = 1f / Time.unscaledDeltaTime;
            frameTime = Time.unscaledDeltaTime * 1000f;

            // 获取内存使用
            memoryUsage = Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024);

            // 自适应性能调整
            if (enableAdaptivePerformance)
            {
                AdjustPerformance();
            }
        }
    }

    /// <summary>
    /// 自适应性能调整
    /// </summary>
    private void AdjustPerformance()
    {
        // 如果帧率过低，降低质量
        if (fps < targetFrameRate * 0.8f)
        {
            // 降低阴影质量
            if (QualitySettings.shadowDistance > 20f)
            {
                QualitySettings.shadowDistance -= 5f;
            }

            // 降低LOD偏置
            if (QualitySettings.lodBias > 0.5f)
            {
                QualitySettings.lodBias -= 0.1f;
            }
        }
        // 如果帧率足够高，可以提高质量
        else if (fps > targetFrameRate * 1.2f)
        {
            // 提高阴影质量
            if (QualitySettings.shadowDistance < 100f)
            {
                QualitySettings.shadowDistance += 2f;
            }

            // 提高LOD偏置
            if (QualitySettings.lodBias < 2f)
            {
                QualitySettings.lodBias += 0.05f;
            }
        }
    }

    /// <summary>
    /// 内存优化协程
    /// </summary>
    private IEnumerator MemoryOptimizationCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(unloadInterval);

            // 卸载未使用的资源
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
    }

    /// <summary>
    /// 获取性能数据
    /// </summary>
    public PerformanceData GetPerformanceData()
    {
        return new PerformanceData
        {
            fps = fps,
            frameTime = frameTime,
            memoryUsage = memoryUsage,
            drawCalls = drawCalls,
            triangles = triangles
        };
    }

    /// <summary>
    /// 设置目标帧率
    /// </summary>
    public void SetTargetFrameRate(int frameRate)
    {
        targetFrameRate = frameRate;
        Application.targetFrameRate = targetFrameRate;
    }

    /// <summary>
    /// 设置垂直同步
    /// </summary>
    public void SetVSync(bool enabled)
    {
        enableVSync = enabled;
        QualitySettings.vSyncCount = enabled ? 1 : 0;
    }

    /// <summary>
    /// 设置阴影距离
    /// </summary>
    public void SetShadowDistance(float distance)
    {
        shadowDistance = distance;
        QualitySettings.shadowDistance = shadowDistance;
    }

    /// <summary>
    /// 设置LOD偏置
    /// </summary>
    public void SetLODBias(float bias)
    {
        lodBias = bias;
        QualitySettings.lodBias = lodBias;
    }

    /// <summary>
    /// 获取当前FPS
    /// </summary>
    public float GetFPS() => fps;

    /// <summary>
    /// 获取当前帧时间
    /// </summary>
    public float GetFrameTime() => frameTime;

    /// <summary>
    /// 获取内存使用量
    /// </summary>
    public long GetMemoryUsage() => memoryUsage;
}

/// <summary>
/// 性能数据
/// </summary>
[System.Serializable]
public struct PerformanceData
{
    public float fps;
    public float frameTime;
    public long memoryUsage;
    public int drawCalls;
    public int triangles;
}

/// <summary>
/// LOD控制器 - 管理LOD级别
/// </summary>
public class LODController : MonoBehaviour
{
    [Header("LOD设置")]
    [SerializeField] private LODGroup lodGroup;
    [SerializeField] private float lodBias = 1f;
    [SerializeField] private float screenRelativeTransitionHeight = 0.8f;

    [Header("LOD级别")]
    [SerializeField] private LODLevel[] lodLevels;

    private void Awake()
    {
        if (lodGroup == null)
        {
            lodGroup = GetComponent<LODGroup>();
        }

        SetupLOD();
    }

    /// <summary>
    /// 设置LOD
    /// </summary>
    private void SetupLOD()
    {
        if (lodGroup == null || lodLevels == null || lodLevels.Length == 0)
            return;

        LOD[] lods = new LOD[lodLevels.Length];

        for (int i = 0; i < lodLevels.Length; i++)
        {
            Renderer[] renderers = lodLevels[i].renderers;
            float screenRelativeTransitionHeight = lodLevels[i].screenRelativeTransitionHeight;

            lods[i] = new LOD(screenRelativeTransitionHeight, renderers);
        }

        lodGroup.SetLODs(lods);
        lodGroup.fadeMode = LODFadeMode.CrossFade;
    }

    /// <summary>
    /// 设置LOD偏置
    /// </summary>
    public void SetLODBias(float bias)
    {
        lodBias = bias;
        if (lodGroup != null)
        {
            lodGroup.size = lodBias;
        }
    }
}

/// <summary>
/// LOD级别
/// </summary>
[System.Serializable]
public class LODLevel
{
    public string name;
    public Renderer[] renderers;
    public float screenRelativeTransitionHeight = 0.8f;
}

/// <summary>
/// 对象池管理器 - 管理多个对象池
/// </summary>
public class PoolManager : MonoBehaviour
{
    [Header("对象池配置")]
    [SerializeField] private PoolConfig[] poolConfigs;

    // 对象池字典
    private Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();

    // 单例
    public static PoolManager Instance { get; private set; }

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

        InitializePools();
    }

    /// <summary>
    /// 初始化对象池
    /// </summary>
    private void InitializePools()
    {
        if (poolConfigs == null) return;

        foreach (var config in poolConfigs)
        {
            if (config.prefab != null)
            {
                CreatePool(config.poolName, config.prefab, config.initialSize, config.maxSize);
            }
        }
    }

    /// <summary>
    /// 创建对象池
    /// </summary>
    public ObjectPool CreatePool(string poolName, GameObject prefab, int initialSize = 10, int maxSize = 50)
    {
        if (pools.ContainsKey(poolName))
        {
            Debug.LogWarning($"对象池已存在: {poolName}");
            return pools[poolName];
        }

        GameObject poolObj = new GameObject($"Pool_{poolName}");
        poolObj.transform.SetParent(transform);
        ObjectPool pool = poolObj.AddComponent<ObjectPool>();

        // 设置对象池属性（通过反射或公共方法）
        // pool.Initialize(prefab, initialSize, maxSize);

        pools.Add(poolName, pool);
        return pool;
    }

    /// <summary>
    /// 获取对象池
    /// </summary>
    public ObjectPool GetPool(string poolName)
    {
        if (pools.TryGetValue(poolName, out ObjectPool pool))
        {
            return pool;
        }

        Debug.LogWarning($"对象池未找到: {poolName}");
        return null;
    }

    /// <summary>
    /// 从池中获取对象
    /// </summary>
    public GameObject GetObject(string poolName)
    {
        ObjectPool pool = GetPool(poolName);
        return pool?.GetObject();
    }

    /// <summary>
    /// 从池中获取对象并设置位置
    /// </summary>
    public GameObject GetObject(string poolName, Vector3 position, Quaternion rotation)
    {
        ObjectPool pool = GetPool(poolName);
        return pool?.GetObject(position, rotation);
    }

    /// <summary>
    /// 归还对象到池
    /// </summary>
    public void ReturnObject(string poolName, GameObject obj)
    {
        ObjectPool pool = GetPool(poolName);
        pool?.ReturnObject(obj);
    }

    /// <summary>
    /// 归还所有对象
    /// </summary>
    public void ReturnAllObjects()
    {
        foreach (var pool in pools.Values)
        {
            pool.ReturnAllObjects();
        }
    }
}

/// <summary>
/// 对象池配置
/// </summary>
[System.Serializable]
public class PoolConfig
{
    public string poolName;
    public GameObject prefab;
    public int initialSize = 10;
    public int maxSize = 50;
}