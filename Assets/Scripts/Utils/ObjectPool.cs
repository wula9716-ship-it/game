using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 对象池 - 优化频繁创建和销毁对象
/// </summary>
public class ObjectPool : MonoBehaviour
{
    [Header("池设置")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private int initialSize = 10;
    [SerializeField] private int maxSize = 50;
    [SerializeField] private bool autoExpand = true;

    // 对象池
    private Queue<GameObject> pool = new Queue<GameObject>();
    private List<GameObject> activeObjects = new List<GameObject>();

    // 单例
    private static Dictionary<string, ObjectPool> pools = new Dictionary<string, ObjectPool>();

    private void Awake()
    {
        InitializePool();
    }

    /// <summary>
    /// 初始化对象池
    /// </summary>
    private void InitializePool()
    {
        for (int i = 0; i < initialSize; i++)
        {
            CreateNewObject();
        }
    }

    /// <summary>
    /// 创建新对象
    /// </summary>
    private GameObject CreateNewObject()
    {
        GameObject obj = Instantiate(prefab, transform);
        obj.SetActive(false);
        pool.Enqueue(obj);
        return obj;
    }

    /// <summary>
    /// 从池中获取对象
    /// </summary>
    public GameObject GetObject()
    {
        GameObject obj = null;

        // 尝试从池中获取
        while (pool.Count > 0)
        {
            obj = pool.Dequeue();
            if (obj != null)
            {
                break;
            }
        }

        // 池为空时创建新对象
        if (obj == null)
        {
            if (autoExpand && activeObjects.Count < maxSize)
            {
                obj = CreateNewObject();
                obj = pool.Dequeue();
            }
            else
            {
                Debug.LogWarning("对象池已满，无法获取新对象");
                return null;
            }
        }

        // 激活对象
        obj.SetActive(true);
        activeObjects.Add(obj);

        return obj;
    }

    /// <summary>
    /// 从池中获取对象并设置位置
    /// </summary>
    public GameObject GetObject(Vector3 position, Quaternion rotation)
    {
        GameObject obj = GetObject();
        if (obj != null)
        {
            obj.transform.position = position;
            obj.transform.rotation = rotation;
        }
        return obj;
    }

    /// <summary>
    /// 归还对象到池
    /// </summary>
    public void ReturnObject(GameObject obj)
    {
        if (obj == null) return;

        // 检查对象是否属于此池
        if (!activeObjects.Contains(obj))
        {
            Debug.LogWarning("尝试归还不属于此池的对象");
            return;
        }

        // 停用对象
        obj.SetActive(false);
        obj.transform.SetParent(transform);

        // 从活跃列表移除
        activeObjects.Remove(obj);

        // 归还到池
        pool.Enqueue(obj);
    }

    /// <summary>
    /// 归还所有活跃对象
    /// </summary>
    public void ReturnAllObjects()
    {
        List<GameObject> objectsToReturn = new List<GameObject>(activeObjects);
        foreach (var obj in objectsToReturn)
        {
            ReturnObject(obj);
        }
    }

    /// <summary>
    /// 获取活跃对象数量
    /// </summary>
    public int ActiveCount => activeObjects.Count;

    /// <summary>
    /// 获取池中可用对象数量
    /// </summary>
    public int AvailableCount => pool.Count;

    /// <summary>
    /// 获取或创建对象池
    /// </summary>
    public static ObjectPool GetPool(string poolName, GameObject prefab, int initialSize = 10)
    {
        if (pools.TryGetValue(poolName, out ObjectPool existingPool))
        {
            return existingPool;
        }

        // 创建新池
        GameObject poolObj = new GameObject($"Pool_{poolName}");
        ObjectPool pool = poolObj.AddComponent<ObjectPool>();
        pool.prefab = prefab;
        pool.initialSize = initialSize;
        pool.InitializePool();

        pools.Add(poolName, pool);
        return pool;
    }

    /// <summary>
    /// 清除所有池
    /// </summary>
    public static void ClearAllPools()
    {
        foreach (var pool in pools.Values)
        {
            if (pool != null)
            {
                pool.ReturnAllObjects();
            }
        }
        pools.Clear();
    }
}