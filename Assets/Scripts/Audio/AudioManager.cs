using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 音频管理器 - 管理所有音效和音乐
/// </summary>
public class AudioManager : MonoBehaviour
{
    [Header("音频混合器")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private AudioMixerGroup musicGroup;
    [SerializeField] private AudioMixerGroup sfxGroup;
    [SerializeField] private AudioMixerGroup ambientGroup;

    [Header("音频源")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource ambientSource;

    [Header("音量设置")]
    [SerializeField] private float masterVolume = 1f;
    [SerializeField] private float musicVolume = 0.7f;
    [SerializeField] private float sfxVolume = 1f;
    [SerializeField] private float ambientVolume = 0.5f;

    [Header("音效库")]
    [SerializeField] private SoundLibrary soundLibrary;

    [Header("音乐设置")]
    [SerializeField] private float musicFadeTime = 1f;
    [SerializeField] private float musicCrossfadeTime = 2f;

    // 单例
    public static AudioManager Instance { get; private set; }

    // 当前音乐
    private AudioClip currentMusic;
    private Coroutine musicFadeCoroutine;

    // 环境音效
    private Dictionary<string, AudioSource> ambientSounds = new Dictionary<string, AudioSource>();

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

        InitializeAudio();
    }

    private void Start()
    {
        LoadVolumeSettings();
    }

    /// <summary>
    /// 初始化音频系统
    /// </summary>
    private void InitializeAudio()
    {
        // 创建音频源（如果未设置）
        if (musicSource == null)
        {
            musicSource = CreateAudioSource("MusicSource", musicGroup);
            musicSource.loop = true;
        }

        if (sfxSource == null)
        {
            sfxSource = CreateAudioSource("SFXSource", sfxGroup);
        }

        if (ambientSource == null)
        {
            ambientSource = CreateAudioSource("AmbientSource", ambientGroup);
            ambientSource.loop = true;
        }
    }

    /// <summary>
    /// 创建音频源
    /// </summary>
    private AudioSource CreateAudioSource(string name, AudioMixerGroup group)
    {
        GameObject audioObj = new GameObject(name);
        audioObj.transform.SetParent(transform);
        AudioSource source = audioObj.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = group;
        source.playOnAwake = false;
        return source;
    }

    /// <summary>
    /// 加载音量设置
    /// </summary>
    private void LoadVolumeSettings()
    {
        masterVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1f);
        ambientVolume = PlayerPrefs.GetFloat("AmbientVolume", 0.5f);

        ApplyVolumeSettings();
    }

    /// <summary>
    /// 保存音量设置
    /// </summary>
    private void SaveVolumeSettings()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.SetFloat("AmbientVolume", ambientVolume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 应用音量设置
    /// </summary>
    private void ApplyVolumeSettings()
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolume) * 20);
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(musicVolume) * 20);
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolume) * 20);
            audioMixer.SetFloat("AmbientVolume", Mathf.Log10(ambientVolume) * 20);
        }
    }

    #region 音乐控制

    /// <summary>
    /// 播放音乐
    /// </summary>
    public void PlayMusic(AudioClip clip, bool fade = true)
    {
        if (clip == null || clip == currentMusic)
            return;

        currentMusic = clip;

        if (fade)
        {
            if (musicFadeCoroutine != null)
            {
                StopCoroutine(musicFadeCoroutine);
            }
            musicFadeCoroutine = StartCoroutine(CrossfadeMusic(clip));
        }
        else
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
    }

    /// <summary>
    /// 停止音乐
    /// </summary>
    public void StopMusic(bool fade = true)
    {
        if (fade)
        {
            if (musicFadeCoroutine != null)
            {
                StopCoroutine(musicFadeCoroutine);
            }
            musicFadeCoroutine = StartCoroutine(FadeOutMusic());
        }
        else
        {
            musicSource.Stop();
            currentMusic = null;
        }
    }

    /// <summary>
    /// 暂停音乐
    /// </summary>
    public void PauseMusic()
    {
        musicSource.Pause();
    }

    /// <summary>
    /// 恢复音乐
    /// </summary>
    public void ResumeMusic()
    {
        musicSource.UnPause();
    }

    /// <summary>
    /// 交叉淡入淡出音乐
    /// </summary>
    private IEnumerator CrossfadeMusic(AudioClip newClip)
    {
        float timer = 0;
        float startVolume = musicSource.volume;

        // 淡出当前音乐
        while (timer < musicCrossfadeTime * 0.5f)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0, timer / (musicCrossfadeTime * 0.5f));
            yield return null;
        }

        // 切换音乐
        musicSource.clip = newClip;
        musicSource.Play();

        // 淡入新音乐
        timer = 0;
        while (timer < musicCrossfadeTime * 0.5f)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(0, startVolume, timer / (musicCrossfadeTime * 0.5f));
            yield return null;
        }

        musicSource.volume = startVolume;
        musicFadeCoroutine = null;
    }

    /// <summary>
    /// 淡出音乐
    /// </summary>
    private IEnumerator FadeOutMusic()
    {
        float timer = 0;
        float startVolume = musicSource.volume;

        while (timer < musicFadeTime)
        {
            timer += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0, timer / musicFadeTime);
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume;
        currentMusic = null;
        musicFadeCoroutine = null;
    }

    #endregion

    #region 音效控制

    /// <summary>
    /// 播放音效
    /// </summary>
    public void PlaySFX(AudioClip clip, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;

        sfxSource.pitch = pitch;
        sfxSource.PlayOneShot(clip, volume);
    }

    /// <summary>
    /// 播放音效（按名称）
    /// </summary>
    public void PlaySFX(string soundName, float volume = 1f, float pitch = 1f)
    {
        if (soundLibrary == null) return;

        AudioClip clip = soundLibrary.GetSound(soundName);
        if (clip != null)
        {
            PlaySFX(clip, volume, pitch);
        }
    }

    /// <summary>
    /// 在指定位置播放音效
    /// </summary>
    public void PlaySFXAtPosition(AudioClip clip, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        if (clip == null) return;

        // 创建临时音频源
        GameObject audioObj = new GameObject("TempAudio");
        audioObj.transform.position = position;
        AudioSource source = audioObj.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.spatialBlend = 1f; // 3D音效
        source.outputAudioMixerGroup = sfxGroup;
        source.Play();

        // 播放完成后销毁
        Destroy(audioObj, clip.length / pitch);
    }

    /// <summary>
    /// 在指定位置播放音效（按名称）
    /// </summary>
    public void PlaySFXAtPosition(string soundName, Vector3 position, float volume = 1f, float pitch = 1f)
    {
        if (soundLibrary == null) return;

        AudioClip clip = soundLibrary.GetSound(soundName);
        if (clip != null)
        {
            PlaySFXAtPosition(clip, position, volume, pitch);
        }
    }

    #endregion

    #region 环境音效控制

    /// <summary>
    /// 播放环境音效
    /// </summary>
    public void PlayAmbient(AudioClip clip, float volume = 1f, bool loop = true)
    {
        if (clip == null) return;

        ambientSource.clip = clip;
        ambientSource.volume = volume;
        ambientSource.loop = loop;
        ambientSource.Play();
    }

    /// <summary>
    /// 停止环境音效
    /// </summary>
    public void StopAmbient(bool fade = true)
    {
        if (fade)
        {
            StartCoroutine(FadeOutAmbient());
        }
        else
        {
            ambientSource.Stop();
        }
    }

    /// <summary>
    /// 淡出环境音效
    /// </summary>
    private IEnumerator FadeOutAmbient()
    {
        float timer = 0;
        float startVolume = ambientSource.volume;

        while (timer < musicFadeTime)
        {
            timer += Time.deltaTime;
            ambientSource.volume = Mathf.Lerp(startVolume, 0, timer / musicFadeTime);
            yield return null;
        }

        ambientSource.Stop();
        ambientSource.volume = startVolume;
    }

    /// <summary>
    /// 播放循环环境音效
    /// </summary>
    public void PlayLoopingAmbient(string id, AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;

        // 检查是否已存在
        if (ambientSounds.ContainsKey(id))
        {
            ambientSounds[id].volume = volume;
            return;
        }

        // 创建新的音频源
        GameObject audioObj = new GameObject($"Ambient_{id}");
        audioObj.transform.SetParent(transform);
        AudioSource source = audioObj.AddComponent<AudioSource>();
        source.clip = clip;
        source.volume = volume;
        source.loop = true;
        source.outputAudioMixerGroup = ambientGroup;
        source.Play();

        ambientSounds.Add(id, source);
    }

    /// <summary>
    /// 停止循环环境音效
    /// </summary>
    public void StopLoopingAmbient(string id, bool fade = true)
    {
        if (!ambientSounds.ContainsKey(id))
            return;

        AudioSource source = ambientSounds[id];
        if (fade)
        {
            StartCoroutine(FadeOutLoopingAmbient(id, source));
        }
        else
        {
            source.Stop();
            Destroy(source.gameObject);
            ambientSounds.Remove(id);
        }
    }

    /// <summary>
    /// 淡出循环环境音效
    /// </summary>
    private IEnumerator FadeOutLoopingAmbient(string id, AudioSource source)
    {
        float timer = 0;
        float startVolume = source.volume;

        while (timer < musicFadeTime)
        {
            timer += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0, timer / musicFadeTime);
            yield return null;
        }

        source.Stop();
        Destroy(source.gameObject);
        ambientSounds.Remove(id);
    }

    #endregion

    #region 音量控制

    /// <summary>
    /// 设置主音量
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        ApplyVolumeSettings();
        SaveVolumeSettings();
    }

    /// <summary>
    /// 设置音乐音量
    /// </summary>
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        ApplyVolumeSettings();
        SaveVolumeSettings();
    }

    /// <summary>
    /// 设置音效音量
    /// </summary>
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        ApplyVolumeSettings();
        SaveVolumeSettings();
    }

    /// <summary>
    /// 设置环境音量
    /// </summary>
    public void SetAmbientVolume(float volume)
    {
        ambientVolume = Mathf.Clamp01(volume);
        ApplyVolumeSettings();
        SaveVolumeSettings();
    }

    /// <summary>
    /// 获取主音量
    /// </summary>
    public float GetMasterVolume() => masterVolume;

    /// <summary>
    /// 获取音乐音量
    /// </summary>
    public float GetMusicVolume() => musicVolume;

    /// <summary>
    /// 获取音效音量
    /// </summary>
    public float GetSFXVolume() => sfxVolume;

    /// <summary>
    /// 获取环境音量
    /// </summary>
    public float GetAmbientVolume() => ambientVolume;

    #endregion
}

/// <summary>
/// 音效库
/// </summary>
[CreateAssetMenu(fileName = "SoundLibrary", menuName = "Survival Island/Sound Library")]
public class SoundLibrary : ScriptableObject
{
    [Header("音效列表")]
    [SerializeField] private List<SoundEntry> sounds = new List<SoundEntry>();

    // 音效字典
    private Dictionary<string, AudioClip> soundDictionary;

    /// <summary>
    /// 初始化音效库
    /// </summary>
    public void Initialize()
    {
        soundDictionary = new Dictionary<string, AudioClip>();

        foreach (var entry in sounds)
        {
            if (entry.clip != null && !soundDictionary.ContainsKey(entry.name))
            {
                soundDictionary.Add(entry.name, entry.clip);
            }
        }
    }

    /// <summary>
    /// 获取音效
    /// </summary>
    public AudioClip GetSound(string name)
    {
        if (soundDictionary == null)
        {
            Initialize();
        }

        if (soundDictionary.TryGetValue(name, out AudioClip clip))
        {
            return clip;
        }

        Debug.LogWarning($"音效未找到: {name}");
        return null;
    }

    /// <summary>
    /// 添加音效
    /// </summary>
    public void AddSound(string name, AudioClip clip)
    {
        if (clip != null)
        {
            sounds.Add(new SoundEntry { name = name, clip = clip });

            if (soundDictionary != null)
            {
                soundDictionary.TryAdd(name, clip);
            }
        }
    }
}

/// <summary>
/// 音效条目
/// </summary>
[System.Serializable]
public class SoundEntry
{
    public string name;
    public AudioClip clip;
}