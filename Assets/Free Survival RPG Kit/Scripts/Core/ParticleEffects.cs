using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 简易粒子特效系统 — 砍树碎屑、受击火花、走路灰尘
/// </summary>
public class ParticleEffects : MonoBehaviour
{
    private static ParticleEffects _instance;
    public static ParticleEffects Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("ParticleEffects");
                _instance = go.AddComponent<ParticleEffects>();
                Object.DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    private List<EffectParticle> particles = new List<EffectParticle>();

    class EffectParticle
    {
        public GameObject go;
        public Transform tr;
        public Vector3 velocity;
        public float life;
        public float maxLife;
        public Color color;
    }

    /// <summary>
    /// 在位置产生碎屑效果
    /// </summary>
    public void SpawnDebris(Vector3 pos, Color color, int count = 8)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject p = GameObject.CreatePrimitive(PrimitiveType.Cube);
            p.name = "Debris";
            float s = 0.03f + Random.value * 0.06f;
            p.transform.localScale = new Vector3(s, s, s);
            p.transform.position = pos;
            p.GetComponent<Renderer>().material = GameBootstrapColored(color);

            EffectParticle ep = new EffectParticle
            {
                go = p,
                tr = p.transform,
                velocity = Random.onUnitSphere * (2f + Random.value * 3f),
                life = 0.5f + Random.value * 0.5f,
                maxLife = 0.5f + Random.value * 0.5f,
                color = color
            };
            particles.Add(ep);
            Object.Destroy(p.GetComponent<Collider>());
        }

        // 清理过期粒子
        if (particles.Count > 200)
        {
            for (int i = particles.Count - 1; i >= 0; i--)
            {
                if (particles[i].life <= 0)
                {
                    Object.Destroy(particles[i].go);
                    particles.RemoveAt(i);
                }
            }
        }
    }

    /// <summary>
    /// 产生冲击波环
    /// </summary>
    public void SpawnImpactRing(Vector3 pos, float size = 0.5f)
    {
        GameObject ring = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        ring.name = "ImpactRing";
        ring.transform.position = pos;
        ring.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        ring.GetComponent<Renderer>().material = GameBootstrapColored(new Color(1f, 0.8f, 0.2f, 0.7f));
        Object.Destroy(ring.GetComponent<Collider>());
        ring.AddComponent<ImpactRingExpander>();
    }

    void Update()
    {
        for (int i = particles.Count - 1; i >= 0; i--)
        {
            EffectParticle ep = particles[i];
            ep.life -= Time.deltaTime;
            if (ep.life <= 0)
            {
                if (ep.go != null) Object.Destroy(ep.go);
                particles.RemoveAt(i);
                continue;
            }

            float t = ep.life / ep.maxLife;
            ep.velocity.y -= 9.8f * Time.deltaTime; // 重力
            ep.tr.position += ep.velocity * Time.deltaTime;
            float s = Mathf.Lerp(0, 1, t);
            ep.tr.localScale = Vector3.one * s * 0.05f;
            if (ep.go != null)
            {
                Renderer r = ep.go.GetComponent<Renderer>();
                if (r != null)
                {
                    Color c = ep.color;
                    c.a = t;
                    r.material.color = c;
                }
            }
        }
    }

    // 工具
    static Material GameBootstrapColored(Color c)
    {
        Material m = new Material(Shader.Find("Standard"));
        m.color = c;
        return m;
    }
}

/// <summary>
/// 冲击波放大动画
/// </summary>
public class ImpactRingExpander : MonoBehaviour
{
    float life = 0.4f;
    float maxLife = 0.4f;

    void Update()
    {
        life -= Time.deltaTime;
        if (life <= 0) { Object.Destroy(gameObject); return; }

        float t = life / maxLife;
        float s = (1f - t) * 2f;
        transform.localScale = new Vector3(s, 0.005f, s);

        Renderer r = GetComponent<Renderer>();
        if (r != null)
        {
            Color c = r.material.color;
            c.a = t * 0.6f;
            r.material.color = c;
        }
    }
}
