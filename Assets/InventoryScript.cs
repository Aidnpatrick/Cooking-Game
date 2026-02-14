using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;

public class InventoryScript : MonoBehaviour
{
    public PlayerScript playerScript;
    public CameraScript cameraScript;
    public GameObject gameCanvas;
    public GameObject healthContainer;
    public GameObject tabContainer;
    public GameObject tabUI;
    public GameObject tabChild;
    public GameObject currentSlotImg;
    public TMP_Text materialText;
    public GameObject image;
    public GameObject heartPrefab;
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
                    if(inventory[equippedItem - 1].Contains("TakeDown"))
                        cameraScript.canEdit = false;
                    else
                        cameraScript.canEdit = true;
                }
                else
                    cameraScript.canEdit = true;
            }
            else
                cameraScript.canEdit = true;
        }
        
        if (keyboard.qKey.wasPressedThisFrame)
            DropEquippedItem();
    }
    public void AddItem(string itemName)
    {
        Debug.Log(itemName);
        if (itemName.Contains("Ammo"))
        {
            ammo += 10;
            return;        
        }
        if (inventory.Count >= maxInventorySize) return;
        if (Resources.Load<GameObject>(itemName) == null) return;
        inventory.Add(itemName);
        equippedItem = Mathf.Clamp(equippedItem, 1, inventory.Count);
        UpdatePlayerChildren();
    }

    public void UpdatePlayerChildren()
    {
        foreach (Transform child in player.transform)
            Destroy(child.gameObject);

        for (int i = 0; i < inventory.Count; i++)
        {
            GameObject prefab = Resources.Load<GameObject>(inventory[i]);
            if (prefab == null) continue;

            GameObject item = Instantiate(prefab, player.transform);
            item.name = inventory[i] + (i + 1);
            item.transform.localPosition = Vector3.zero + new Vector3(0.5f,0,0);
            item.transform.localRotation = Quaternion.identity;
            item.SetActive(false);
        }
    }



    public void DropEquippedItem(bool isBlock = false)
    {
        if (inventory.Count == 0) return;

        int index = equippedItem - 1;
        if (index < 0 || index >= inventory.Count) return;

        string itemName = inventory[index];

        GameObject lootPrefab = Resources.Load<GameObject>(itemName + "Loot");
        if (lootPrefab != null && !isBlock)
        {
            GameObject dropped = Instantiate(lootPrefab, player.transform.position, Quaternion.identity);
            dropped.tag = "Loot";
            dropped.name = itemName + "Loot";
        }

        inventory.RemoveAt(index);
        equippedItem = inventory.Count == 0 ? 1 : Mathf.Clamp(equippedItem, 1, inventory.Count);

        UpdatePlayerChildren();
        cameraScript.canEdit = true;
    }

    public GameObject findPlayerTargetChild(string targetString)
    {
        int index = 0;
        foreach(GameObject i in gameObject.transform)
        {
            if(i.name.Contains(targetString) && index == equippedItem - 1)
            {
                return i;
            }
            index++;
        }
        return null;
    }
}
