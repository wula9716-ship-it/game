using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// 建造系统 - 管理建筑放置和建造
/// </summary>
public class BuildingSystem : MonoBehaviour
{
    [Header("建造设置")]
    [SerializeField] private float buildDistance = 5f;
    [SerializeField] private float gridSnapSize = 1f;
    [SerializeField] private LayerMask buildableSurfaceLayer;
    [SerializeField] private LayerMask obstructionLayer;

    [Header("建造预览")]
    [SerializeField] private Material validPlacementMaterial;
    [SerializeField] private Material invalidPlacementMaterial;

    [Header("建筑数据库")]
    [SerializeField] private BuildingDatabase buildingDatabase;

    // 事件
    public UnityEvent<BuildingData> OnBuildingSelected;
    public UnityEvent<BuildingData, Vector3, Quaternion> OnBuildingPlaced;
    public UnityEvent OnBuildingCancelled;

    // 状态
    private BuildingData currentBuilding;
    private GameObject previewObject;
    private bool isPlacing;
    private bool canPlace;

    // 已放置的建筑
    private List<PlacedBuilding> placedBuildings = new List<PlacedBuilding>();

    private void Update()
    {
        if (isPlacing)
        {
            UpdatePreview();
            HandlePlacementInput();
        }
    }

    /// <summary>
    /// 选择建筑
    /// </summary>
    public void SelectBuilding(BuildingData building)
    {
        if (building == null) return;

        CancelPlacement();

        currentBuilding = building;
        isPlacing = true;

        // 创建预览对象
        if (building.previewPrefab != null)
        {
            previewObject = Instantiate(building.previewPrefab);
            SetPreviewMaterial(validPlacementMaterial);
        }

        OnBuildingSelected?.Invoke(building);
    }

    /// <summary>
    /// 取消放置
    /// </summary>
    public void CancelPlacement()
    {
        if (previewObject != null)
        {
            Destroy(previewObject);
            previewObject = null;
        }

        currentBuilding = null;
        isPlacing = false;
        canPlace = false;

        OnBuildingCancelled?.Invoke();
    }

    /// <summary>
    /// 更新预览位置
    /// </summary>
    private void UpdatePreview()
    {
        if (previewObject == null) return;

        // 射线检测放置位置
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, buildDistance, buildableSurfaceLayer))
        {
            // 对齐到网格
            Vector3 placementPosition = GetSnappedPosition(hit.point);

            // 检查是否可以放置
            canPlace = CanPlaceAtPosition(placementPosition);

            // 更新预览位置和旋转
            previewObject.transform.position = placementPosition;
            previewObject.transform.rotation = GetPlacementRotation(hit.normal);

            // 更新预览材质
            SetPreviewMaterial(canPlace ? validPlacementMaterial : invalidPlacementMaterial);
        }
    }

    /// <summary>
    /// 处理放置输入
    /// </summary>
    private void HandlePlacementInput()
    {
        // 左键放置
        if (Input.GetMouseButtonDown(0) && canPlace)
        {
            PlaceBuilding();
        }

        // 右键取消
        if (Input.GetMouseButtonDown(1))
        {
            CancelPlacement();
        }

        // R键旋转
        if (Input.GetKeyDown(KeyCode.R))
        {
            RotatePreview(90f);
        }

        // 滚轮旋转
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            RotatePreview(scroll * 90f);
        }
    }

    /// <summary>
    /// 放置建筑
    /// </summary>
    private void PlaceBuilding()
    {
        if (currentBuilding == null || previewObject == null) return;

        // 检查资源
        if (!HasRequiredResources())
        {
            Debug.Log("资源不足！");
            return;
        }

        // 消耗资源
        ConsumeResources();

        // 放置建筑
        Vector3 position = previewObject.transform.position;
        Quaternion rotation = previewObject.transform.rotation;

        GameObject buildingObject = Instantiate(currentBuilding.buildingPrefab, position, rotation);

        // 添加建筑组件
        Building building = buildingObject.GetComponent<Building>();
        if (building == null)
        {
            building = buildingObject.AddComponent<Building>();
        }
        building.Initialize(currentBuilding);

        // 记录已放置的建筑
        placedBuildings.Add(new PlacedBuilding
        {
            data = currentBuilding,
            position = position,
            rotation = rotation,
            buildingObject = buildingObject
        });

        OnBuildingPlaced?.Invoke(currentBuilding, position, rotation);

        // 如果不可堆叠，取消选择
        if (!currentBuilding.canStack)
        {
            CancelPlacement();
        }
    }

    /// <summary>
    /// 获取对齐后的位置
    /// </summary>
    private Vector3 GetSnappedPosition(Vector3 position)
    {
        return new Vector3(
            Mathf.Round(position.x / gridSnapSize) * gridSnapSize,
            Mathf.Round(position.y / gridSnapSize) * gridSnapSize,
            Mathf.Round(position.z / gridSnapSize) * gridSnapSize
        );
    }

    /// <summary>
    /// 获取放置旋转
    /// </summary>
    private Quaternion GetPlacementRotation(Vector3 normal)
    {
        // 根据法线方向计算旋转
        if (currentBuilding.alignToSurface)
        {
            return Quaternion.FromToRotation(Vector3.up, normal) * previewObject.transform.rotation;
        }
        return previewObject.transform.rotation;
    }

    /// <summary>
    /// 检查是否可以放置
    /// </summary>
    private bool CanPlaceAtPosition(Vector3 position)
    {
        if (currentBuilding == null) return false;

        // 检查障碍物
        Collider[] colliders = Physics.OverlapBox(
            position + currentBuilding.placementOffset,
            currentBuilding.placementSize * 0.5f,
            previewObject.transform.rotation,
            obstructionLayer
        );

        foreach (var collider in colliders)
        {
            // 忽略预览对象
            if (collider.gameObject == previewObject) continue;

            // 检查是否是建筑
            Building building = collider.GetComponent<Building>();
            if (building != null && !currentBuilding.canPlaceOnBuildings)
            {
                return false;
            }

            // 检查是否是地形
            if (!currentBuilding.canPlaceOnTerrain && collider.gameObject.layer == LayerMask.NameToLayer("Terrain"))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 检查是否有足够资源
    /// </summary>
    private bool HasRequiredResources()
    {
        InventorySystem inventory = FindObjectOfType<InventorySystem>();
        if (inventory == null) return false;

        foreach (var requirement in currentBuilding.requiredResources)
        {
            if (!inventory.HasItem(requirement.item, requirement.amount))
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 消耗资源
    /// </summary>
    private void ConsumeResources()
    {
        InventorySystem inventory = FindObjectOfType<InventorySystem>();
        if (inventory == null) return;

        foreach (var requirement in currentBuilding.requiredResources)
        {
            inventory.RemoveItem(requirement.item, requirement.amount);
        }
    }

    /// <summary>
    /// 旋转预览
    /// </summary>
    private void RotatePreview(float angle)
    {
        if (previewObject != null)
        {
            previewObject.transform.Rotate(Vector3.up, angle);
        }
    }

    /// <summary>
    /// 设置预览材质
    /// </summary>
    private void SetPreviewMaterial(Material material)
    {
        if (previewObject == null || material == null) return;

        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.material = material;
        }
    }

    /// <summary>
    /// 获取已放置的建筑列表
    /// </summary>
    public List<PlacedBuilding> GetPlacedBuildings()
    {
        return placedBuildings;
    }

    /// <summary>
    /// 是否正在放置
    /// </summary>
    public bool IsPlacing => isPlacing;
}

/// <summary>
/// 已放置的建筑
/// </summary>
[System.Serializable]
public class PlacedBuilding
{
    public BuildingData data;
    public Vector3 position;
    public Quaternion rotation;
    public GameObject buildingObject;
}