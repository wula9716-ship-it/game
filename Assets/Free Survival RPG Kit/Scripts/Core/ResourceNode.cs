using UnityEngine;

/// <summary>
/// 可采集资源节点 - Adapted for FreeSurvivalRPGKit
/// </summary>
public class ResourceNode : MonoBehaviour
{
    public string resourceName = "Wood";
    public float maxHealth = 30f;
    public float respawnTime = 30f;

    [Header("Drop Settings")]
    public Item dropItem;               // ScriptableObject item to drop
    public int dropAmount = 3;

    private float currentHealth;
    public float CurrentHealth => currentHealth;
    private Vector3 originalScale;
    private bool harvested;
    private float respawnTimer;
    private bool showHealthBar;
    private float healthBarTimer;

    void Start()
    {
        currentHealth = maxHealth;
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (harvested)
        {
            respawnTimer -= Time.deltaTime;
            if (respawnTimer <= 0)
            {
                harvested = false;
                currentHealth = maxHealth;
                transform.localScale = originalScale;
                gameObject.SetActive(true);
            }
        }
        if (showHealthBar)
        {
            healthBarTimer -= Time.deltaTime;
            if (healthBarTimer <= 0) showHealthBar = false;
        }
    }

    public void Harvest(float damage)
    {
        if (harvested) return;

        currentHealth -= damage;
        showHealthBar = true;
        healthBarTimer = 2f;

        float pct = currentHealth / maxHealth;
        transform.localScale = originalScale * (0.5f + 0.5f * pct);

        if (currentHealth <= 0)
        {
            harvested = true;
            gameObject.SetActive(false);
            respawnTimer = respawnTime;

            Debug.Log("[Resource] " + resourceName + " harvested!");

            // Drop items to inventory
            if (dropItem != null && Inventory.instance != null)
            {
                for (int i = 0; i < dropAmount; i++)
                {
                    Inventory.instance.Add(dropItem);
                }
                Debug.Log("[Resource] Added " + dropAmount + "x " + dropItem.name + " to inventory");
            }
        }
    }

    void OnGUI()
    {
        if (!showHealthBar || harvested) return;
        Camera cam = Camera.main;
        if (cam == null) return;

        Vector3 sp = cam.WorldToScreenPoint(transform.position + Vector3.up * 2.5f);
        if (sp.z < 0) return;

        float bw = 60, bh = 8;
        float x = sp.x - bw / 2, y = Screen.height - sp.y;
        float pct = currentHealth / maxHealth;

        GUI.Box(new Rect(x - 1, y - 1, bw + 2, bh + 2), "");
        Color bc = pct > 0.5f ? Color.green : (pct > 0.25f ? Color.yellow : Color.red);
        Texture2D t = new Texture2D(1, 1);
        t.SetPixel(0, 0, bc);
        t.Apply();
        GUI.DrawTexture(new Rect(x, y, bw * pct, bh), t);
        GUI.Label(new Rect(x, y - 16, bw, 14), resourceName);
    }
}
