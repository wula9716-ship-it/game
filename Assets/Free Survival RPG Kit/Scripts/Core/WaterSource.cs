using UnityEngine;

/// <summary>
/// 可饮用水源 — 靠近按E补水
/// </summary>
public class WaterSource : MonoBehaviour
{
    public float restoreAmount = 40f;
    public float cooldown = 2f;
    public string sourceName = "水源";
    private float lastUseTime;

    void Start()
    {
        // 创建发光水球外观
        GameObject visual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        visual.name = "WaterVisual";
        visual.transform.SetParent(transform);
        visual.transform.localPosition = new Vector3(0, 0.3f, 0);
        visual.transform.localScale = new Vector3(1.5f, 0.3f, 1.5f);

        Material mat = new Material(Shader.Find("Standard"));
        mat.color = new Color(0.15f, 0.4f, 0.75f, 0.7f); // 半透明蓝色
        if (mat.HasProperty("_EmissionColor"))
        {
            mat.EnableKeyword("_EMISSION");
            mat.SetColor("_EmissionColor", new Color(0.1f, 0.2f, 0.5f));
        }
        if (mat.HasProperty("_Glossiness")) mat.SetFloat("_Glossiness", 0.9f);
        // 透明度需要用透明shader
        mat.SetFloat("_Mode", 2); // Fade mode
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ENABLED");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = 3000;

        visual.GetComponent<Renderer>().material = mat;
        Destroy(visual.GetComponent<Collider>());
    }

    public bool TryUse()
    {
        if (Time.time - lastUseTime < cooldown) return false;

        PlayerStats stats = FindAnyObjectByType<PlayerStats>();
        if (stats == null) return false;
        if (stats.currentThirsty >= stats.maxThirsty.GetValue())
        {
            Debug.Log($"[{sourceName}] 你已经不渴了");
            return false;
        }

        stats.HealThirsty((int)restoreAmount);
        lastUseTime = Time.time;
        Debug.Log($"[{sourceName}] 补充水分 +{restoreAmount:F0} → 口渴值 {stats.currentThirsty}/{stats.maxThirsty.GetValue()}");
        return true;
    }
}
