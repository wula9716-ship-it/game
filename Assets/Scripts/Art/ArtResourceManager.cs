using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 美术资源管理器 - 管理游戏美术资源
/// </summary>
public class ArtResourceManager : MonoBehaviour
{
    [Header("材质库")]
    [SerializeField] private MaterialLibrary materialLibrary;

    [Header("模型库")]
    [SerializeField] private ModelLibrary modelLibrary;

    [Header("预制体库")]
    [SerializeField] private PrefabLibrary prefabLibrary;

    // 单例
    public static ArtResourceManager Instance { get; private set; }

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

    /// <summary>
    /// 获取材质
    /// </summary>
    public Material GetMaterial(string name)
    {
        return materialLibrary?.GetMaterial(name);
    }

    /// <summary>
    /// 获取模型
    /// </summary>
    public GameObject GetModel(string name)
    {
        return modelLibrary?.GetModel(name);
    }

    /// <summary>
    /// 获取预制体
    /// </summary>
    public GameObject GetPrefab(string name)
    {
        return prefabLibrary?.GetPrefab(name);
    }
}

/// <summary>
/// 材质库
/// </summary>
[CreateAssetMenu(fileName = "MaterialLibrary", menuName = "Survival Island/Material Library")]
public class MaterialLibrary : ScriptableObject
{
    [Header("材质列表")]
    [SerializeField] private List<MaterialEntry> materials = new List<MaterialEntry>();

    // 材质字典
    private Dictionary<string, Material> materialDictionary;

    /// <summary>
    /// 初始化材质库
    /// </summary>
    public void Initialize()
    {
        materialDictionary = new Dictionary<string, Material>();

        foreach (var entry in materials)
        {
            if (entry.material != null && !materialDictionary.ContainsKey(entry.name))
            {
                materialDictionary.Add(entry.name, entry.material);
            }
        }
    }

    /// <summary>
    /// 获取材质
    /// </summary>
    public Material GetMaterial(string name)
    {
        if (materialDictionary == null)
        {
            Initialize();
        }

        if (materialDictionary.TryGetValue(name, out Material material))
        {
            return material;
        }

        Debug.LogWarning($"材质未找到: {name}");
        return null;
    }

    /// <summary>
    /// 添加材质
    /// </summary>
    public void AddMaterial(string name, Material material)
    {
        if (material != null)
        {
            materials.Add(new MaterialEntry { name = name, material = material });

            if (materialDictionary != null)
            {
                materialDictionary.TryAdd(name, material);
            }
        }
    }
}

/// <summary>
/// 材质条目
/// </summary>
[System.Serializable]
public class MaterialEntry
{
    public string name;
    public Material material;
}

/// <summary>
/// 模型库
/// </summary>
[CreateAssetMenu(fileName = "ModelLibrary", menuName = "Survival Island/Model Library")]
public class ModelLibrary : ScriptableObject
{
    [Header("模型列表")]
    [SerializeField] private List<ModelEntry> models = new List<ModelEntry>();

    // 模型字典
    private Dictionary<string, GameObject> modelDictionary;

    /// <summary>
    /// 初始化模型库
    /// </summary>
    public void Initialize()
    {
        modelDictionary = new Dictionary<string, GameObject>();

        foreach (var entry in models)
        {
            if (entry.model != null && !modelDictionary.ContainsKey(entry.name))
            {
                modelDictionary.Add(entry.name, entry.model);
            }
        }
    }

    /// <summary>
    /// 获取模型
    /// </summary>
    public GameObject GetModel(string name)
    {
        if (modelDictionary == null)
        {
            Initialize();
        }

        if (modelDictionary.TryGetValue(name, out GameObject model))
        {
            return model;
        }

        Debug.LogWarning($"模型未找到: {name}");
        return null;
    }

    /// <summary>
    /// 添加模型
    /// </summary>
    public void AddModel(string name, GameObject model)
    {
        if (model != null)
        {
            models.Add(new ModelEntry { name = name, model = model });

            if (modelDictionary != null)
            {
                modelDictionary.TryAdd(name, model);
            }
        }
    }
}

/// <summary>
/// 模型条目
/// </summary>
[System.Serializable]
public class ModelEntry
{
    public string name;
    public GameObject model;
}

/// <summary>
/// 预制体库
/// </summary>
[CreateAssetMenu(fileName = "PrefabLibrary", menuName = "Survival Island/Prefab Library")]
public class PrefabLibrary : ScriptableObject
{
    [Header("预制体列表")]
    [SerializeField] private List<PrefabEntry> prefabs = new List<PrefabEntry>();

    // 预制体字典
    private Dictionary<string, GameObject> prefabDictionary;

    /// <summary>
    /// 初始化预制体库
    /// </summary>
    public void Initialize()
    {
        prefabDictionary = new Dictionary<string, GameObject>();

        foreach (var entry in prefabs)
        {
            if (entry.prefab != null && !prefabDictionary.ContainsKey(entry.name))
            {
                prefabDictionary.Add(entry.name, entry.prefab);
            }
        }
    }

    /// <summary>
    /// 获取预制体
    /// </summary>
    public GameObject GetPrefab(string name)
    {
        if (prefabDictionary == null)
        {
            Initialize();
        }

        if (prefabDictionary.TryGetValue(name, out GameObject prefab))
        {
            return prefab;
        }

        Debug.LogWarning($"预制体未找到: {name}");
        return null;
    }

    /// <summary>
    /// 添加预制体
    /// </summary>
    public void AddPrefab(string name, GameObject prefab)
    {
        if (prefab != null)
        {
            prefabs.Add(new PrefabEntry { name = name, prefab = prefab });

            if (prefabDictionary != null)
            {
                prefabDictionary.TryAdd(name, prefab);
            }
        }
    }
}

/// <summary>
/// 预制体条目
/// </summary>
[System.Serializable]
public class PrefabEntry
{
    public string name;
    public GameObject prefab;
}

/// <summary>
/// 材质创建器 - 创建基础材质
/// </summary>
public class MaterialCreator : MonoBehaviour
{
    [Header("材质库")]
    [SerializeField] private MaterialLibrary materialLibrary;

    [Header("基础颜色")]
    [SerializeField] private Color woodColor = new Color(0.6f, 0.4f, 0.2f);
    [SerializeField] private Color stoneColor = new Color(0.5f, 0.5f, 0.5f);
    [SerializeField] private Color metalColor = new Color(0.7f, 0.7f, 0.7f);
    [SerializeField] private Color grassColor = new Color(0.2f, 0.6f, 0.2f);
    [SerializeField] private Color sandColor = new Color(0.9f, 0.8f, 0.6f);
    [SerializeField] private Color waterColor = new Color(0.2f, 0.4f, 0.8f, 0.7f);

    /// <summary>
    /// 创建所有基础材质
    /// </summary>
    public void CreateAllMaterials()
    {
        if (materialLibrary == null)
        {
            Debug.LogError("材质库未设置！");
            return;
        }

        CreateBasicMaterials();
        CreateNatureMaterials();
        CreateBuildingMaterials();
        CreateEffectMaterials();

        Debug.Log("所有材质创建完成！");
    }

    /// <summary>
    /// 创建基础材质
    /// </summary>
    private void CreateBasicMaterials()
    {
        // 木材材质
        Material woodMat = CreateMaterial("Wood", woodColor, 0.5f, 0.3f);
        materialLibrary.AddMaterial("Wood", woodMat);

        // 石头材质
        Material stoneMat = CreateMaterial("Stone", stoneColor, 0.3f, 0.1f);
        materialLibrary.AddMaterial("Stone", stoneMat);

        // 金属材质
        Material metalMat = CreateMaterial("Metal", metalColor, 0.8f, 0.6f);
        materialLibrary.AddMaterial("Metal", metalMat);

        // 皮革材质
        Material leatherMat = CreateMaterial("Leather", new Color(0.4f, 0.3f, 0.2f), 0.4f, 0.2f);
        materialLibrary.AddMaterial("Leather", leatherMat);

        // 布料材质
        Material clothMat = CreateMaterial("Cloth", new Color(0.8f, 0.8f, 0.7f), 0.2f, 0.1f);
        materialLibrary.AddMaterial("Cloth", clothMat);
    }

    /// <summary>
    /// 创建自然材质
    /// </summary>
    private void CreateNatureMaterials()
    {
        // 草地材质
        Material grassMat = CreateMaterial("Grass", grassColor, 0.3f, 0.1f);
        materialLibrary.AddMaterial("Grass", grassMat);

        // 沙地材质
        Material sandMat = CreateMaterial("Sand", sandColor, 0.2f, 0.1f);
        materialLibrary.AddMaterial("Sand", sandMat);

        // 水材质
        Material waterMat = CreateTransparentMaterial("Water", waterColor, 0.9f, 0.8f);
        materialLibrary.AddMaterial("Water", waterMat);

        // 泥土材质
        Material dirtMat = CreateMaterial("Dirt", new Color(0.4f, 0.3f, 0.2f), 0.3f, 0.1f);
        materialLibrary.AddMaterial("Dirt", dirtMat);

        // 雪地材质
        Material snowMat = CreateMaterial("Snow", new Color(0.9f, 0.9f, 0.95f), 0.2f, 0.1f);
        materialLibrary.AddMaterial("Snow", snowMat);
    }

    /// <summary>
    /// 创建建筑材质
    /// </summary>
    private void CreateBuildingMaterials()
    {
        // 木板材质
        Material plankMat = CreateMaterial("Plank", new Color(0.7f, 0.5f, 0.3f), 0.4f, 0.2f);
        materialLibrary.AddMaterial("Plank", plankMat);

        // 砖块材质
        Material brickMat = CreateMaterial("Brick", new Color(0.7f, 0.3f, 0.2f), 0.3f, 0.1f);
        materialLibrary.AddMaterial("Brick", brickMat);

        // 瓦片材质
        Material tileMat = CreateMaterial("Tile", new Color(0.6f, 0.6f, 0.6f), 0.5f, 0.3f);
        materialLibrary.AddMaterial("Tile", tileMat);

        // 玻璃材质
        Material glassMat = CreateTransparentMaterial("Glass", new Color(0.8f, 0.9f, 1f, 0.3f), 0.9f, 0.9f);
        materialLibrary.AddMaterial("Glass", glassMat);
    }

    /// <summary>
    /// 创建效果材质
    /// </summary>
    private void CreateEffectMaterials()
    {
        // 发光材质
        Material glowMat = CreateEmissiveMaterial("Glow", Color.white, 2f);
        materialLibrary.AddMaterial("Glow", glowMat);

        // 能量材质
        Material energyMat = CreateEmissiveMaterial("Energy", new Color(0f, 0.8f, 1f), 3f);
        materialLibrary.AddMaterial("Energy", energyMat);

        // 火焰材质
        Material fireMat = CreateEmissiveMaterial("Fire", new Color(1f, 0.5f, 0f), 4f);
        materialLibrary.AddMaterial("Fire", fireMat);

        // 毒液材质
        Material poisonMat = CreateEmissiveMaterial("Poison", new Color(0f, 1f, 0f), 2f);
        materialLibrary.AddMaterial("Poison", poisonMat);
    }

    /// <summary>
    /// 创建材质
    /// </summary>
    private Material CreateMaterial(string name, Color color, float metallic, float smoothness)
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.name = name;
        mat.color = color;
        mat.SetFloat("_Metallic", metallic);
        mat.SetFloat("_Glossiness", smoothness);
        return mat;
    }

    /// <summary>
    /// 创建透明材质
    /// </summary>
    private Material CreateTransparentMaterial(string name, Color color, float metallic, float smoothness)
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.name = name;
        mat.color = color;
        mat.SetFloat("_Metallic", metallic);
        mat.SetFloat("_Glossiness", smoothness);
        mat.SetFloat("_Mode", 3); // Transparent mode
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;
        return mat;
    }

    /// <summary>
    /// 创建发光材质
    /// </summary>
    private Material CreateEmissiveMaterial(string name, Color color, float intensity)
    {
        Material mat = new Material(Shader.Find("Standard"));
        mat.name = name;
        mat.color = color;
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", color * intensity);
        return mat;
    }
}

/// <summary>
/// 程序化地形生成器
/// </summary>
public class ProceduralTerrainGenerator : MonoBehaviour
{
    [Header("地形设置")]
    [SerializeField] private int width = 256;
    [SerializeField] private int height = 256;
    [SerializeField] private int depth = 20;
    [SerializeField] private float scale = 20f;

    [Header("噪声设置")]
    [SerializeField] private int octaves = 4;
    [SerializeField] private float persistence = 0.5f;
    [SerializeField] private float lacunarity = 2f;
    [SerializeField] private int seed = 42;

    [Header("材质")]
    [SerializeField] private Material terrainMaterial;

    /// <summary>
    /// 生成地形
    /// </summary>
    public void GenerateTerrain()
    {
        // 创建地形数据
        TerrainData terrainData = new TerrainData();
        terrainData.heightmapResolution = width + 1;
        terrainData.size = new Vector3(width, depth, height);

        // 生成高度图
        float[,] heights = GenerateHeights();
        terrainData.SetHeights(0, 0, heights);

        // 创建地形对象
        GameObject terrainObj = Terrain.CreateTerrainGameObject(terrainData);
        terrainObj.transform.SetParent(transform);

        // 设置材质
        if (terrainMaterial != null)
        {
            terrainObj.GetComponent<Terrain>().materialTemplate = terrainMaterial;
        }
    }

    /// <summary>
    /// 生成高度图
    /// </summary>
    private float[,] GenerateHeights()
    {
        float[,] heights = new float[width, height];

        // 设置随机种子
        Random.State oldState = Random.state;
        Random.InitState(seed);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float xCoord = (float)x / width * scale;
                float yCoord = (float)y / height * scale;

                float noiseValue = 0;
                float amplitude = 1;
                float frequency = 1;

                // 分形噪声
                for (int i = 0; i < octaves; i++)
                {
                    noiseValue += Mathf.PerlinNoise(xCoord * frequency, yCoord * frequency) * amplitude;
                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                heights[x, y] = noiseValue;
            }
        }

        // 恢复随机状态
        Random.state = oldState;

        return heights;
    }
}

/// <summary>
/// 程序化植被生成器
/// </summary>
public class ProceduralVegetationGenerator : MonoBehaviour
{
    [Header("植被设置")]
    [SerializeField] private VegetationType[] vegetationTypes;
    [SerializeField] private int maxInstances = 1000;
    [SerializeField] private float spawnRadius = 100f;

    [Header("生成设置")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float minSlope = 0f;
    [SerializeField] private float maxSlope = 45f;

    /// <summary>
    /// 生成植被
    /// </summary>
    public void GenerateVegetation()
    {
        if (vegetationTypes == null || vegetationTypes.Length == 0)
            return;

        for (int i = 0; i < maxInstances; i++)
        {
            // 随机位置
            Vector3 randomPos = transform.position + Random.insideUnitSphere * spawnRadius;
            randomPos.y = 100f; // 从高处射线检测

            // 射线检测地面
            RaycastHit hit;
            if (Physics.Raycast(randomPos, Vector3.down, out hit, 200f, groundLayer))
            {
                // 检查坡度
                float slope = Vector3.Angle(hit.normal, Vector3.up);
                if (slope >= minSlope && slope <= maxSlope)
                {
                    // 选择植被类型
                    VegetationType vegType = GetRandomVegetationType();
                    if (vegType != null && vegType.prefab != null)
                    {
                        // 生成植被
                        GameObject veg = Instantiate(vegType.prefab, hit.point, Quaternion.identity);
                        veg.transform.SetParent(transform);

                        // 随机旋转和缩放
                        veg.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                        float scale = Random.Range(vegType.minScale, vegType.maxScale);
                        veg.transform.localScale = Vector3.one * scale;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 获取随机植被类型
    /// </summary>
    private VegetationType GetRandomVegetationType()
    {
        // 加权随机选择
        float totalWeight = 0;
        foreach (var vegType in vegetationTypes)
        {
            totalWeight += vegType.spawnWeight;
        }

        float random = Random.Range(0, totalWeight);
        float current = 0;

        foreach (var vegType in vegetationTypes)
        {
            current += vegType.spawnWeight;
            if (random <= current)
            {
                return vegType;
            }
        }

        return vegetationTypes[0];
    }
}

/// <summary>
/// 植被类型
/// </summary>
[System.Serializable]
public class VegetationType
{
    public string name;
    public GameObject prefab;
    public float spawnWeight = 1f;
    public float minScale = 0.8f;
    public float maxScale = 1.2f;
}