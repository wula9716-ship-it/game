using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// 天气系统 - 管理游戏天气变化
/// </summary>
public class WeatherSystem : MonoBehaviour
{
    [Header("天气设置")]
    [SerializeField] private float weatherChangeInterval = 300f; // 5分钟
    [SerializeField] private float weatherTransitionTime = 30f; // 30秒过渡

    [Header("天气类型")]
    [SerializeField] private List<WeatherType> weatherTypes = new List<WeatherType>();

    [Header("时间设置")]
    [SerializeField] private float dayLength = 600f; // 10分钟一天
    [SerializeField] private float startTime = 0.25f; // 从早上6点开始

    // 事件
    public UnityEvent<WeatherType> OnWeatherChanged;
    public UnityEvent<float> OnTimeChanged;
    public UnityEvent OnDayStarted;
    public UnityEvent OnNightStarted;

    // 当前状态
    private WeatherType currentWeather;
    private WeatherType targetWeather;
    private float weatherTimer;
    private float weatherTransitionTimer;
    private bool isTransitioning;

    // 时间状态
    private float currentTime;
    private float dayTimer;
    private bool isDaytime;

    // 天气效果
    private float weatherDamage;
    private float visibilityModifier;
    private float temperatureModifier;

    private void Awake()
    {
        InitializeWeatherTypes();
    }

    private void Start()
    {
        // 初始化天气
        SetRandomWeather();

        // 初始化时间
        currentTime = startTime;
        UpdateTimeOfDay();
    }

    private void Update()
    {
        UpdateWeather();
        UpdateTime();
    }

    /// <summary>
    /// 初始化天气类型
    /// </summary>
    private void InitializeWeatherTypes()
    {
        if (weatherTypes.Count == 0)
        {
            // 添加默认天气类型
            weatherTypes.Add(new WeatherType
            {
                name = "晴天",
                weatherDamage = 0f,
                visibilityModifier = 1f,
                temperatureModifier = 0f,
                probability = 0.4f
            });

            weatherTypes.Add(new WeatherType
            {
                name = "阴天",
                weatherDamage = 0f,
                visibilityModifier = 0.8f,
                temperatureModifier = -2f,
                probability = 0.3f
            });

            weatherTypes.Add(new WeatherType
            {
                name = "小雨",
                weatherDamage = 0.5f,
                visibilityModifier = 0.6f,
                temperatureModifier = -5f,
                probability = 0.2f
            });

            weatherTypes.Add(new WeatherType
            {
                name = "大雨",
                weatherDamage = 1f,
                visibilityModifier = 0.4f,
                temperatureModifier = -8f,
                probability = 0.1f
            });
        }
    }

    /// <summary>
    /// 更新天气
    /// </summary>
    private void UpdateWeather()
    {
        weatherTimer += Time.deltaTime;

        // 检查是否需要改变天气
        if (weatherTimer >= weatherChangeInterval)
        {
            weatherTimer = 0f;
            SetRandomWeather();
        }

        // 天气过渡
        if (isTransitioning)
        {
            weatherTransitionTimer += Time.deltaTime;
            float t = weatherTransitionTimer / weatherTransitionTime;

            if (t >= 1f)
            {
                isTransitioning = false;
                currentWeather = targetWeather;
            }
        }
    }

    /// <summary>
    /// 更新时间
    /// </summary>
    private void UpdateTime()
    {
        dayTimer += Time.deltaTime;
        currentTime += Time.deltaTime / dayLength;

        if (currentTime >= 1f)
        {
            currentTime -= 1f;
        }

        // 更新时间状态
        UpdateTimeOfDay();

        OnTimeChanged?.Invoke(currentTime);
    }

    /// <summary>
    /// 更新时间状态
    /// </summary>
    private void UpdateTimeOfDay()
    {
        bool wasDaytime = isDaytime;
        isDaytime = currentTime >= 0.25f && currentTime < 0.75f; // 6点到18点

        if (isDaytime && !wasDaytime)
        {
            OnDayStarted?.Invoke();
        }
        else if (!isDaytime && wasDaytime)
        {
            OnNightStarted?.Invoke();
        }
    }

    /// <summary>
    /// 设置随机天气
    /// </summary>
    private void SetRandomWeather()
    {
        float random = Random.value;
        float cumulative = 0f;

        foreach (var weather in weatherTypes)
        {
            cumulative += weather.probability;

            if (random <= cumulative)
            {
                ChangeWeather(weather);
                break;
            }
        }
    }

    /// <summary>
    /// 改变天气
    /// </summary>
    public void ChangeWeather(WeatherType newWeather)
    {
        if (currentWeather == null)
        {
            currentWeather = newWeather;
            ApplyWeatherEffects();
            OnWeatherChanged?.Invoke(currentWeather);
            return;
        }

        targetWeather = newWeather;
        isTransitioning = true;
        weatherTransitionTimer = 0f;

        OnWeatherChanged?.Invoke(targetWeather);
    }

    /// <summary>
    /// 应用天气效果
    /// </summary>
    private void ApplyWeatherEffects()
    {
        if (currentWeather == null) return;

        weatherDamage = currentWeather.weatherDamage;
        visibilityModifier = currentWeather.visibilityModifier;
        temperatureModifier = currentWeather.temperatureModifier;
    }

    /// <summary>
    /// 获取天气伤害
    /// </summary>
    public float GetWeatherDamage()
    {
        return weatherDamage;
    }

    /// <summary>
    /// 获取能见度修正
    /// </summary>
    public float GetVisibilityModifier()
    {
        return visibilityModifier;
    }

    /// <summary>
    /// 获取温度修正
    /// </summary>
    public float GetTemperatureModifier()
    {
        return temperatureModifier;
    }

    /// <summary>
    /// 获取当前天气
    /// </summary>
    public WeatherType GetCurrentWeather()
    {
        return currentWeather;
    }

    /// <summary>
    /// 是否是白天
    /// </summary>
    public bool IsDaytime => isDaytime;

    /// <summary>
    /// 获取当前时间（0-1）
    /// </summary>
    public float CurrentTime => currentTime;
}

/// <summary>
/// 天气类型
/// </summary>
[System.Serializable]
public class WeatherType
{
    public string name;
    public float weatherDamage;
    public float visibilityModifier;
    public float temperatureModifier;
    public float probability;
}