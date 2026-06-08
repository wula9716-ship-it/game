using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 游戏HUD - 显示玩家状态和快捷栏
/// </summary>
public class GameHUD : MonoBehaviour
{
    [Header("状态条")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private Slider hungerBar;
    [SerializeField] private Slider thirstBar;
    [SerializeField] private Slider staminaBar;

    [Header("状态文本")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI hungerText;
    [SerializeField] private TextMeshProUGUI thirstText;
    [SerializeField] private TextMeshProUGUI staminaText;

    [Header("快捷栏")]
    [SerializeField] private Transform hotbarContainer;
    [SerializeField] private GameObject hotbarSlotPrefab;

    [Header("时间显示")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI weatherText;

    [Header("交互提示")]
    [SerializeField] private GameObject interactionPrompt;
    [SerializeField] private TextMeshProUGUI interactionText;

    // 组件引用
    private PlayerStats playerStats;
    private InventorySystem inventorySystem;
    private WeatherSystem weatherSystem;

    // 快捷栏槽位
    private HotbarSlot[] hotbarSlots;
    private int selectedSlot = 0;

    private void Start()
    {
        // 获取组件引用
        playerStats = FindObjectOfType<PlayerStats>();
        inventorySystem = FindObjectOfType<InventorySystem>();
        weatherSystem = FindObjectOfType<WeatherSystem>();

        // 初始化快捷栏
        InitializeHotbar();

        // 注册事件
        RegisterEvents();
    }

    private void Update()
    {
        UpdateStatusBars();
        UpdateHotbarSelection();
        UpdateTimeDisplay();
    }

    /// <summary>
    /// 初始化快捷栏
    /// </summary>
    private void InitializeHotbar()
    {
        if (inventorySystem == null || hotbarContainer == null || hotbarSlotPrefab == null)
            return;

        hotbarSlots = new HotbarSlot[inventorySystem.HotbarSize];

        for (int i = 0; i < inventorySystem.HotbarSize; i++)
        {
            GameObject slotObj = Instantiate(hotbarSlotPrefab, hotbarContainer);
            HotbarSlot slot = slotObj.GetComponent<HotbarSlot>();

            if (slot != null)
            {
                slot.Initialize(i);
                hotbarSlots[i] = slot;
            }
        }

        // 选中第一个槽位
        SelectSlot(0);
    }

    /// <summary>
    /// 注册事件
    /// </summary>
    private void RegisterEvents()
    {
        if (playerStats != null)
        {
            playerStats.OnHealthChanged.AddListener(UpdateHealthBar);
            playerStats.OnHungerChanged.AddListener(UpdateHungerBar);
            playerStats.OnThirstChanged.AddListener(UpdateThirstBar);
            playerStats.OnStaminaChanged.AddListener(UpdateStaminaBar);
        }

        if (inventorySystem != null)
        {
            inventorySystem.OnHotbarChanged.AddListener(OnHotbarChanged);
        }
    }

    /// <summary>
    /// 更新状态条
    /// </summary>
    private void UpdateStatusBars()
    {
        if (playerStats == null) return;

        UpdateHealthBar(playerStats.CurrentHealth);
        UpdateHungerBar(playerStats.CurrentHunger);
        UpdateThirstBar(playerStats.CurrentThirst);
        UpdateStaminaBar(playerStats.CurrentStamina);
    }

    /// <summary>
    /// 更新生命值条
    /// </summary>
    private void UpdateHealthBar(float value)
    {
        if (healthBar != null)
        {
            healthBar.value = value / playerStats.MaxHealth;
        }

        if (healthText != null)
        {
            healthText.text = $"{Mathf.CeilToInt(value)}/{playerStats.MaxHealth}";
        }
    }

    /// <summary>
    /// 更新饥饿值条
    /// </summary>
    private void UpdateHungerBar(float value)
    {
        if (hungerBar != null)
        {
            hungerBar.value = value / playerStats.MaxHunger;
        }

        if (hungerText != null)
        {
            hungerText.text = $"{Mathf.CeilToInt(value)}/{playerStats.MaxHunger}";
        }
    }

    /// <summary>
    /// 更新口渴值条
    /// </summary>
    private void UpdateThirstBar(float value)
    {
        if (thirstBar != null)
        {
            thirstBar.value = value / playerStats.MaxThirst;
        }

        if (thirstText != null)
        {
            thirstText.text = $"{Mathf.CeilToInt(value)}/{playerStats.MaxThirst}";
        }
    }

    /// <summary>
    /// 更新体力值条
    /// </summary>
    private void UpdateStaminaBar(float value)
    {
        if (staminaBar != null)
        {
            staminaBar.value = value / playerStats.MaxStamina;
        }

        if (staminaText != null)
        {
            staminaText.text = $"{Mathf.CeilToInt(value)}/{playerStats.MaxStamina}";
        }
    }

    /// <summary>
    /// 更新快捷栏选择
    /// </summary>
    private void UpdateHotbarSelection()
    {
        // 数字键1-9选择快捷栏
        for (int i = 0; i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectSlot(i);
                break;
            }
        }

        // 滚轮切换快捷栏
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0)
        {
            SelectSlot((selectedSlot - 1 + hotbarSlots.Length) % hotbarSlots.Length);
        }
        else if (scroll < 0)
        {
            SelectSlot((selectedSlot + 1) % hotbarSlots.Length);
        }
    }

    /// <summary>
    /// 选择快捷栏槽位
    /// </summary>
    private void SelectSlot(int index)
    {
        if (hotbarSlots == null || index < 0 || index >= hotbarSlots.Length)
            return;

        // 取消选中当前槽位
        if (selectedSlot >= 0 && selectedSlot < hotbarSlots.Length)
        {
            hotbarSlots[selectedSlot].SetSelected(false);
        }

        // 选中新槽位
        selectedSlot = index;
        hotbarSlots[selectedSlot].SetSelected(true);
    }

    /// <summary>
    /// 快捷栏改变回调
    /// </summary>
    private void OnHotbarChanged(int index, InventorySlot slot)
    {
        if (hotbarSlots != null && index >= 0 && index < hotbarSlots.Length)
        {
            hotbarSlots[index].UpdateSlot(slot);
        }
    }

    /// <summary>
    /// 更新时间显示
    /// </summary>
    private void UpdateTimeDisplay()
    {
        if (weatherSystem == null) return;

        // 更新时间
        if (timeText != null)
        {
            float currentTime = weatherSystem.CurrentTime;
            int hours = Mathf.FloorToInt(currentTime * 24);
            int minutes = Mathf.FloorToInt((currentTime * 24 - hours) * 60);
            timeText.text = $"{hours:00}:{minutes:00}";
        }

        // 更新天气
        if (weatherText != null)
        {
            var weather = weatherSystem.GetCurrentWeather();
            weatherText.text = weather != null ? weather.name : "晴天";
        }
    }

    /// <summary>
    /// 显示交互提示
    /// </summary>
    public void ShowInteractionPrompt(string message)
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(true);
        }

        if (interactionText != null)
        {
            interactionText.text = message;
        }
    }

    /// <summary>
    /// 隐藏交互提示
    /// </summary>
    public void HideInteractionPrompt()
    {
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    /// <summary>
    /// 获取选中的物品
    /// </summary>
    public ItemData GetSelectedItem()
    {
        if (inventorySystem == null) return null;
        return inventorySystem.GetSelectedItem(selectedSlot);
    }

    /// <summary>
    /// 获取选中的槽位索引
    /// </summary>
    public int GetSelectedSlotIndex()
    {
        return selectedSlot;
    }
}

/// <summary>
/// 快捷栏槽位
/// </summary>
public class HotbarSlot : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Image selectionBorder;

    // 槽位信息
    private int slotIndex;
    private bool isSelected;

    /// <summary>
    /// 初始化槽位
    /// </summary>
    public void Initialize(int index)
    {
        slotIndex = index;
        UpdateSlot(null);
    }

    /// <summary>
    /// 更新槽位显示
    /// </summary>
    public void UpdateSlot(InventorySlot slot)
    {
        if (slot == null || slot.IsEmpty)
        {
            // 清空槽位
            if (iconImage != null)
            {
                iconImage.sprite = null;
                iconImage.enabled = false;
            }

            if (amountText != null)
            {
                amountText.text = "";
            }
        }
        else
        {
            // 更新槽位
            if (iconImage != null)
            {
                iconImage.sprite = slot.Item.icon;
                iconImage.enabled = true;
            }

            if (amountText != null)
            {
                amountText.text = slot.Amount > 1 ? slot.Amount.ToString() : "";
            }
        }
    }

    /// <summary>
    /// 设置选中状态
    /// </summary>
    public void SetSelected(bool selected)
    {
        isSelected = selected;

        if (selectionBorder != null)
        {
            selectionBorder.enabled = selected;
        }
    }

    /// <summary>
    /// 获取槽位索引
    /// </summary>
    public int SlotIndex => slotIndex;
}