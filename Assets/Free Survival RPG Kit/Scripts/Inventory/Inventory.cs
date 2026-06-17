using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour {

	#region Singleton

	public static Inventory instance;

	void Awake ()
	{
		instance = this;
	}

	#endregion

	public delegate void OnItemChanged();
	public OnItemChanged onItemChangedCallback;

	public int space = 10;	// Amount of item spaces

	// Our current list of items in the inventory
	public List<Item> items = new List<Item>();

    public int gold;

    /// <summary>
    /// Check if we have a specific item in inventory
    /// </summary>
    public bool HasItem(Item item)
    {
        return items.Contains(item);
    }

    /// <summary>
    /// Remove a specific amount of an item (for crafting/building)
    /// </summary>
    public bool RemoveItem(Item item, int amount = 1)
    {
        int removed = 0;
        for (int i = items.Count - 1; i >= 0 && removed < amount; i--)
        {
            if (items[i] == item)
            {
                items.RemoveAt(i);
                removed++;
            }
        }
        if (removed > 0 && onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
        return removed >= amount;
    }

    /// <summary>
    /// Count how many of a specific item we have
    /// </summary>
    public int CountItem(Item item)
    {
        int count = 0;
        foreach (var i in items)
        {
            if (i == item) count++;
        }
        return count;
    }

    public void AddGold(int amount)
    {
        gold += amount;
    }

    /// <summary>
    /// Add multiple copies of an item
    /// </summary>
    public void AddItem(Item item, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Add(item);
        }
    }

    private void Update()
    {
        // Update Gold UI
        GameObject inventoryUI = GameObject.Find("Canvas").transform.Find("Inventory").gameObject;
        if(inventoryUI.activeSelf)
            inventoryUI.transform.Find("Gold").Find("Value").GetComponent<Text>().text = gold.ToString();

    }

    // Add a new item if enough room
    public void Add (Item item)
	{
		if (item.showInInventory) {
			if (items.Count >= space) {
				Debug.Log ("Not enough room.");
				return;
			}

			items.Add (item);

			if (onItemChangedCallback != null)
				onItemChangedCallback.Invoke ();
		}
	}

	// Remove an item
	public void Remove (Item item)
	{
		items.Remove(item);

		if (onItemChangedCallback != null)
			onItemChangedCallback.Invoke();
	}

}
