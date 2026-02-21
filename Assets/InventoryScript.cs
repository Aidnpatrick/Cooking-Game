using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;
using TMPro.EditorUtilities;

public class InventoryScript : MonoBehaviour
{
    public PlayerScript playerScript;
    public CameraScript cameraScript;
    public GameControlScript gameControlScript;
    public GameObject player;

    public List<string> inventory = new List<string>();
    public int equippedItem = 1;
    public int ammo = 10;
    public int materials = 1000;
    public int coins = 1000;
    private int maxInventorySize = 5;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var keyboard = Keyboard.current;

        if (keyboard.digit1Key.wasPressedThisFrame && inventory.Count > 0) equippedItem = 1;
        else if (keyboard.digit2Key.wasPressedThisFrame && inventory.Count > 1) equippedItem = 2;
        else if (keyboard.digit3Key.wasPressedThisFrame && inventory.Count > 2) equippedItem = 3;
        else if (keyboard.digit4Key.wasPressedThisFrame && inventory.Count > 3) equippedItem = 4;
        else if (keyboard.digit5Key.wasPressedThisFrame && inventory.Count > 4) equippedItem = 5;

        for (int i = 0; i < inventory.Count; i++)
        {
            Transform t = player.transform.Find(inventory[i] + (i + 1));
            if (t == null) continue;

            bool equipped = i == equippedItem - 1;
            t.gameObject.SetActive(equipped);

            if (equipped)
            {
                t.localPosition = Vector3.zero + new Vector3(0.5f,0,0);
                t.localRotation = Quaternion.identity;
                if(inventory.Count > 0)
                {
                    
                }
                
            }

        }
        
        if (keyboard.qKey.wasPressedThisFrame)
            DropEquippedItem();
    }
    public GameObject AddItem(string itemName, ItemScript ps = null)
    {

        itemName = itemName.Replace("(Clone)", "");
        Debug.Log(itemName);
        if (itemName.Contains("Ammo"))
        {
            ammo += 10;
            return null;        
        }
        if (inventory.Count >= maxInventorySize) return null;
        string foodIndexMain = "";
        foodIndexMain = findFood(itemName);
        
        if(itemName.Contains("Plate"))
        {
            foodIndexMain = itemName;
        }
        if(Resources.Load<GameObject>(foodIndexMain) == null)
        {
            Debug.LogError("Couldn't find: " + foodIndexMain + ", " + itemName);
            return null;
        }
        /*
        if (Resources.Load<GameObject>(foodIndexMain) == null) {
            string swapped = itemName.Replace("Chopped", "").Replace("Cooked", "");
            if(Resources.Load<GameObject>(swapped + "Cooked" + "Chopped") != null)
            {
                itemName = swapped + "Cooked" + "Chopped";
            }
            else return null;
        }
        */

        
        inventory.Add(itemName);

        equippedItem = Mathf.Clamp(equippedItem, 1, inventory.Count);
        
        UpdatePlayerChildren(ps);
        return null;

    }

    public void UpdatePlayerChildren(ItemScript itemScript = null)
    {
        foreach (Transform child in player.transform)
            child.gameObject.SetActive(false);

        for (int i = 0; i < inventory.Count; i++)
        {
            string itemName = inventory[i];

            Transform existing = player.transform.Find(itemName + (i + 1));

            if (existing != null)
            {
                existing.gameObject.SetActive(true);
                continue;
            }
            string foodIndexMain = "";
            foodIndexMain = findFood(itemName);
            if(itemName.Contains("Plate"))
            {
                foodIndexMain = itemName;
            }
            GameObject prefab = Resources.Load<GameObject>(foodIndexMain);
            //GameObject prefab = Resources.Load<GameObject>(itemName);

            if (prefab == null)
            {
                Debug.LogError("There isn't a " + foodIndexMain + ", " + itemName);
                continue;
            }

            GameObject item = Instantiate(prefab, player.transform);
            if(item.name.Contains("Plate") && itemScript != null)
            {
                ItemScript ps = item.GetComponent<ItemScript>();
                ps.storage = itemScript.storage;
            }
            item.name = orderWords(item.name);
            item.name = itemName + (i + 1);
            item.transform.localPosition = new Vector3(0.6f, 0, 0);
            item.transform.localRotation = Quaternion.identity;
            item.SetActive(true);
        }
    }

    public void DropEquippedItem(bool isBlock = false)
    {
        Debug.Log("Used q");
        if (inventory.Count == 0) return;

        int index = equippedItem - 1;
        if (index < 0 || index >= inventory.Count) return;
        Debug.Log("This works.");
        string itemName = inventory[index];
        string foodIndexMain = findFood(itemName);
        if(itemName.Contains("Plate"))
            foodIndexMain = itemName;
            
        Debug.Log(foodIndexMain);
        GameObject lootPrefab = Resources.Load<GameObject>(foodIndexMain);

        if (lootPrefab != null && !isBlock)
        {
            GameObject dropped = Instantiate(lootPrefab, player.transform.position, Quaternion.identity);
            dropped.tag = "Loot";
            dropped.name = orderWords(dropped.name);
            dropped.name = itemName + "Loot";
            dropped.GetComponent<BoxCollider2D>().enabled = true;
            dropped.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/" + itemName.Replace("Loot", ""));
        }

        Debug.Log(index);

        Transform targetTransform = findPlayerTargetChild(inventory[index]);
        inventory.RemoveAt(index);

        Destroy(targetTransform.gameObject);
        equippedItem = inventory.Count == 0 ? 1 : Mathf.Clamp(equippedItem, 1, inventory.Count);


        UpdatePlayerChildren();
        Debug.Log("This works too");
    }

    public Transform findPlayerTargetChild(string targetString)
    {
        int index = equippedItem - 1;

        if (index < 0 || index >= player.transform.childCount)
            return null;

        Transform child = player.transform.GetChild(index);

        if (child.name.Contains(targetString))
            return child;

        return null;
    }
    public string findFood(string itemName)
    {
        string foodIndexMain = "";
        foreach(string foodIndex in gameControlScript.getfoodDictionary())
        {
            if(itemName.Contains(foodIndex) && Resources.Load<GameObject>(foodIndex) != null)
            {
                    
                foodIndexMain = foodIndex;
                return foodIndexMain;
            }
            
        }
        return foodIndexMain;
        
    }
    public string orderWords(string word)
    {
        string swapped = word;  
        if(word.Contains("Chopped") && word.Contains("Cooked"))
        {
            swapped = word.Replace("Chopped", "").Replace("Cooked", "");
            word = swapped + "Cooked" + "Chopped";
        }
        return word;
    }
}

