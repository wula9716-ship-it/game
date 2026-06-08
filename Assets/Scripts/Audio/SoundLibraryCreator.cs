using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 音效库创建器 - 创建预设的音效数据
/// </summary>
public class SoundLibraryCreator : MonoBehaviour
{
    [Header("音效库")]
    [SerializeField] private SoundLibrary soundLibrary;

    /// <summary>
    /// 创建所有预设音效
    /// </summary>
    public void CreateAllSounds()
    {
        if (soundLibrary == null)
        {
            Debug.LogError("音效库未设置！");
            return;
        }

        CreatePlayerSounds();
        CreateCombatSounds();
        CreateBuildingSounds();
        CreateCraftingSounds();
        CreateEnvironmentSounds();
        CreateUISounds();

        Debug.Log("所有音效创建完成！");
    }

    /// <summary>
    /// 创建玩家音效
    /// </summary>
    private void CreatePlayerSounds()
    {
        // 脚步声
        AddSound("FootstepGrass");
        AddSound("FootstepStone");
        AddSound("FootstepWood");
        AddSound("FootstepSand");
        AddSound("FootstepWater");

        // 跳跃声
        AddSound("Jump");
        AddSound("Land");

        // 受伤声
        AddSound("HurtLight");
        AddSound("HurtMedium");
        AddSound("HurtHeavy");

        // 死亡声
        AddSound("Death");

        // 进食声
        AddSound("Eat");
        AddSound("Drink");

        // 呼吸声
        AddSound("BreathingNormal");
        AddSound("BreathingHeavy");
    }

    /// <summary>
    /// 创建战斗音效
    /// </summary>
    private void CreateCombatSounds()
    {
        // 近战攻击
        AddSound("SwingSword");
        AddSound("SwingAxe");
        AddSound("SwingPickaxe");
        AddSound("SwingSpear");

        // 命中
        AddSound("HitFlesh");
        AddSound("HitWood");
        AddSound("HitStone");
        AddSound("HitMetal");

        // 远程攻击
        AddSound("BowDraw");
        AddSound("BowRelease");
        AddSound("ArrowHit");

        // 格挡
        AddSound("Block");
        AddSound("Parry");

        // 敌人
        AddSound("EnemyGrowl");
        AddSound("EnemyAttack");
        AddSound("EnemyDeath");
        AddSound("EnemyAlert");
    }

    /// <summary>
    /// 创建建造音效
    /// </summary>
    private void CreateBuildingSounds()
    {
        // 建造
        AddSound("BuildWood");
        AddSound("BuildStone");
        AddSound("BuildMetal");

        // 破坏
        AddSound("DestroyWood");
        AddSound("DestroyStone");
        AddSound("DestroyMetal");

        // 放置
        AddSound("PlaceWood");
        AddSound("PlaceStone");
        AddSound("PlaceItem");

        // 挖掘
        AddSound("DigDirt");
        AddSound("MineStone");
        AddSound("MineOre");

        // 砍伐
        AddSound("ChopWood");
        AddSound("TreeFall");
    }

    /// <summary>
    /// 创建制作音效
    /// </summary>
    private void CreateCraftingSounds()
    {
        // 制作
        AddSound("CraftStart");
        AddSound("CraftComplete");
        AddSound("CraftFail");

        // 熔炉
        AddSound("FurnaceFire");
        AddSound("FurnaceSmelt");

        // 工作台
        AddSound("WorkbenchUse");

        // 烹饪
        AddSound("CookingSizzle");
        AddSound("CookingBoil");
    }

    /// <summary>
    /// 创建环境音效
    /// </summary>
    private void CreateEnvironmentSounds()
    {
        // 天气
        AddSound("WindLight");
        AddSound("WindStrong");
        AddSound("RainLight");
        AddSound("RainHeavy");
        AddSound("Thunder");
        AddSound("Lightning");

        // 水
        AddSound("WaterFlow");
        AddSound("WaterSplash");
        AddSound("WaterSwim");
        AddSound("WaterDive");

        // 火
        AddSound("FireCrackle");
        AddSound("FireBurning");
        AddSound("TorchLit");

        // 动物
        AddSound("BirdChirp");
        AddSound("BirdFlying");
        AddSound("WolfHowl");
        AddSound("BearGrowl");
        AddSound("SnakeHiss");
        AddSound("InsectBuzz");

        // 环境
        AddSound("LeafRustle");
        AddSound("BranchSnap");
        AddSound("RockSlide");
        AddSound("CaveAmbience");
    }

    /// <summary>
    /// 创建UI音效
    /// </summary>
    private void CreateUISounds()
    {
        // 菜单
        AddSound("MenuOpen");
        AddSound("MenuClose");
        AddSound("MenuSelect");
        AddSound("MenuBack");
        AddSound("MenuHover");

        // 物品
        AddSound("ItemPickup");
        AddSound("ItemDrop");
        AddSound("ItemUse");
        AddSound("ItemEquip");
        AddSound("ItemUnequip");

        // 通知
        AddSound("Notification");
        AddSound("Warning");
        AddSound("Error");
        AddSound("Success");

        // 科技
        AddSound("TechUnlock");
        AddSound("TechResearch");
        AddSound("TechComplete");
    }

    /// <summary>
    /// 添加音效
    /// </summary>
    private void AddSound(string name)
    {
        // 这里只是添加占位，实际音效需要从资源加载
        // soundLibrary.AddSound(name, null);
        Debug.Log($"音效占位: {name}");
    }
}

/// <summary>
/// 音频触发器 - 在特定事件触发音效
/// </summary>
public class AudioTrigger : MonoBehaviour
{
    [Header("触发设置")]
    [SerializeField] private string soundName;
    [SerializeField] private float volume = 1f;
    [SerializeField] private float pitch = 1f;
    [SerializeField] private bool playOnStart = false;
    [SerializeField] private bool playOnEnable = false;
    [SerializeField] private bool playOnCollision = false;
    [SerializeField] private bool playOnTrigger = false;

    [Header("3D音效设置")]
    [SerializeField] private bool is3D = true;
    [SerializeField] private float minDistance = 1f;
    [SerializeField] private float maxDistance = 50f;

    private void Start()
    {
        if (playOnStart)
        {
            PlaySound();
        }
    }

    private void OnEnable()
    {
        if (playOnEnable)
        {
            PlaySound();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (playOnCollision)
        {
            PlaySound();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playOnTrigger)
        {
            PlaySound();
        }
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    public void PlaySound()
    {
        if (AudioManager.Instance == null) return;

        if (is3D)
        {
            AudioManager.Instance.PlaySFXAtPosition(soundName, transform.position, volume, pitch);
        }
        else
        {
            AudioManager.Instance.PlaySFX(soundName, volume, pitch);
        }
    }

    /// <summary>
    /// 设置音效名称
    /// </summary>
    public void SetSoundName(string name)
    {
        soundName = name;
    }
}

/// <summary>
/// 音乐区域 - 进入区域时播放特定音乐
/// </summary>
public class MusicZone : MonoBehaviour
{
    [Header("音乐设置")]
    [SerializeField] private AudioClip musicClip;
    [SerializeField] private float fadeTime = 1f;
    [SerializeField] private bool playOnEnter = true;
    [SerializeField] private bool stopOnExit = true;

    [Header("环境音效")]
    [SerializeField] private AudioClip ambientClip;
    [SerializeField] private float ambientVolume = 0.5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playOnEnter && musicClip != null)
            {
                AudioManager.Instance?.PlayMusic(musicClip, true);
            }

            if (ambientClip != null)
            {
                AudioManager.Instance?.PlayAmbient(ambientClip, ambientVolume);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (stopOnExit)
            {
                AudioManager.Instance?.StopMusic(true);
                AudioManager.Instance?.StopAmbient(true);
            }
        }
    }
}