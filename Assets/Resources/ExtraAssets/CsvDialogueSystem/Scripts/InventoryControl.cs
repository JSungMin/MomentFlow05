using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Inventory prepack for demonstration CSV dialog system features
/// </summary>
public class InventoryControl : MonoBehaviour
{
    public static InventoryControl Instance;                                        // Singleton

    public Text goldAmount;                                                         // Amount of gold in inventory
    public Text foodAmount;                                                         // Amount of food in inventory
    public Image itemCell;                                                          // Image in item slot

    private string itemFolder = "Prefabs/";                                         // Folder for item load

    void Awake()
    {
        Instance = this;                                                            // Make singleton Instance
    }

    void Start()
    {
        if (goldAmount == null || foodAmount == null || itemCell == null)
        {
            Debug.LogError("Wrong default settings");
            return;
        }
        ResetInventory();                                                           // Apply default inventory conditions
    }

    /// <summary>
    /// Apply default inventory conditions
    /// </summary>
    public void ResetInventory()
    {
        goldAmount.text = "2";
        foodAmount.text = "0";
        itemCell.sprite = null;
        itemCell.color = Color.clear;                                               // Make transparent
    }

    /// <summary>
    /// Get current amount of gold
    /// </summary>
    /// <returns> Current amount of gold </returns>
    public int GetGold()
    {
        int res;
        int.TryParse(goldAmount.text, out res);                                     // Parse amount from text value
        return res;
    }

    /// <summary>
    /// Change amount of gold to inventory
    /// </summary>
    /// <param name="value"> Gold change amount </param>
    public void AddGold(int value)
    {
        int currentNum;
        if (int.TryParse(goldAmount.text, out currentNum) == true)                  // Parse amount from text value
        {
            currentNum += value;                                                    // Apply change
            goldAmount.text = currentNum.ToString();                                // Display new amount
        }
    }

    /// <summary>
    /// Add food to inventory
    /// </summary>
    /// <param name="value"> Food change amount </param>
    public void AddFood(int value)
    {
        int currentNum;
        if (int.TryParse(foodAmount.text, out currentNum) == true)                  // Parse amount from text value
        {
            currentNum += value;                                                    // Apply change
            foodAmount.text = currentNum.ToString();                                // Display new amount
        }
    }

    /// <summary>
    /// Check if there is free slot in inventory
    /// </summary>
    /// <returns> true - slot is free, false - no free slots </returns>
    public bool IsItemCellFree()
    {
        if (itemCell.sprite == null)                                                // If no sprite - cell is free
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Add item to inventory slot
    /// </summary>
    /// <param name="itemName"> Item name for loading from resources </param>
    public void AddItem(string itemName)
    {
        if (itemName == null)
        {
            Debug.Log("Wrong input data");
            return;
        }
        GameObject newItem = Resources.Load<GameObject>(itemFolder + itemName);     // Try to load item by its name
        if (newItem == null)
        {
            Debug.LogError("Can not find such item");
            return;
        }
        Sprite sprite = newItem.GetComponent<Image>().sprite;                       // Get sprite of item
        if (sprite != null)
        {
            itemCell.sprite = sprite;                                               // Place on inventory cell
            itemCell.color = Color.white;                                           // Set opacity
        }
    }
}
