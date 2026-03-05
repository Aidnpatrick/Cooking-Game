using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Unity.VisualScripting;
public class CameraScript : MonoBehaviour
{
    public InventoryScript invScript;
    public PlayerScript playerScript;
    public GameControlScript gameControlScript;

    private Vector3 playerCameraPos = new Vector3(0, 0, -5);
    public float interactRange = 3f;

    public GameObject player;
    public GameObject cursorText;

    public bool canEdit = true;
    public bool isOnTile = false;
    public int playerNumber = 1;
    
    void Start()
    {
        transform.position = new Vector3(4.6f, 3.3f, -5);
        canEdit = false;
    }
    public void StartGame()
    {
        canEdit = true;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position = player.transform.position + playerCameraPos;
        
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        Keyboard keyboard = Keyboard.current;

        if(!canEdit) return;

        GameObject target = ClosestTile(player);

        if(target == null) return;
        if(!target.name.Contains("Tile")) return;

        
        if(Vector3.Distance(target.transform.position, player.transform.position) >= 1.75f)
            return;
        else
            gameControlScript.makeTileChosen(target.gameObject);

        //TileScript tileScript = target.GetComponent<TileScript>();

        TileScript tileScript = target.GetComponent<TileScript>();

        if (tileScript == null) return;

        string currentItem;
        if(invScript.inventory.Count > 0)
            currentItem  = invScript.inventory[invScript.equippedItem - 1];
        else currentItem = "";
        //Taking
        
        if(invScript.inventory.Count > 0)
        {
            if(keyboard.eKey.wasPressedThisFrame && target.transform.childCount == 1)
            {
                string childItemName = target.transform.GetChild(0).name;
                childItemName = childItemName.Replace("(Clone)", "");
                Transform itemPlate = invScript.findPlayerTargetChild(currentItem);
                if(currentItem.Contains("Plate") && childItemName.Contains("Chopped"))
                {
                    ItemScript ps = itemPlate.GetComponent<ItemScript>();
                    if(ps.storage.Count == 4)
                        return;
                    if(ps.foodCount("Bun") > 2) return;
                    
                    ps.storage.Add(childItemName);

                    Destroy(target.transform.GetChild(0).gameObject);
                    return;
                }
                
                else if(invScript.inventory.Count == 0)
                {
                    invScript.AddItem(childItemName, null);
                    Transform newPlate = invScript.findPlayerTargetChild(currentItem);
                    ItemScript ps = newPlate.GetComponent<ItemScript>();
                    ps = itemPlate.GetComponent<ItemScript>();
                    Destroy(target.transform.GetChild(0).gameObject);
                    return;
                }

            }
            //giving plate
            if(keyboard.eKey.wasPressedThisFrame && target.transform.childCount == 0)
            {
                Transform item = invScript.findPlayerTargetChild(currentItem);
                item.name = item.name.Substring(0, item.name.Length - 1);

                string indexFood = invScript.findFood(item.name);
                
                if(item.name.Contains("Plate"))
                    indexFood = item.name;

                GameObject prefab = Resources.Load<GameObject>(indexFood);


                GameObject clone = Instantiate(prefab, target.transform);
                clone.name = item.name;
                clone.name = invScript.orderWords(clone.name);
                clone.GetComponent<BoxCollider2D>().enabled = false;
                
                if(clone.name.Contains("Plate"))
                    clone.GetComponent<ItemScript>().storage = item.GetComponent<ItemScript>().storage;
                
                invScript.inventory.RemoveAt(invScript.equippedItem - 1);
                Destroy(item.gameObject);
                invScript.equippedItem = invScript.inventory.Count == 0 ? 1 : Mathf.Clamp(invScript.equippedItem, 1, invScript.inventory.Count);
                invScript.UpdatePlayerChildren();
                return;
            }



            //giving food to plate
            if(keyboard.eKey.wasPressedThisFrame && target.transform.childCount == 1)
            {
                string childItemName = target.transform.GetChild(0).name;
                Transform item = invScript.findPlayerTargetChild(currentItem);
                if(childItemName.Contains("Plate") && !currentItem.Contains("Plate"))
                {
                    ItemScript ps = target.GetComponentInChildren<ItemScript>();
                    if(ps.storage.Count == 4)
                        return;
                        
                    if(ps.foodCount("Bun") > 2) return;
                    ps.storage.Add(item.name.Replace("(Clone)", ""));
                    invScript.inventory.RemoveAt(invScript.equippedItem - 1);
                    Destroy(item.gameObject);
                }
                return;
            }
        }
        if(invScript.inventory.Count == 0)
        {
            if(keyboard.eKey.wasPressedThisFrame && target.transform.childCount > 0)
            {
                string childItemName = target.transform.GetChild(0).name;
                childItemName = childItemName.Replace("(Clone)", "");
                Transform itemPlate = invScript.findPlayerTargetChild(currentItem);
                if(currentItem.Contains("Plate"))
                {
                    ItemScript ps = itemPlate.GetComponent<ItemScript>();
                    if(ps.storage.Count == 4)
                        return;
                    if(ps.foodCount("Bun") > 2) return;

                    ps.storage.Add(childItemName);
                }
                /*
                if(itemPlate.name.Contains("Plate"))
                    invScript.AddItem(childItemName, target.transform.GetChild(0).GetComponent<ItemScript>());
                else 
                    invScript.AddItem(childItemName);*/
                
                invScript.AddItem(childItemName, target.transform.GetChild(0).GetComponent<ItemScript>());
                Destroy(target.transform.GetChild(0).gameObject);
                return;
            }
            if(keyboard.eKey.wasPressedThisFrame && target.transform.childCount == 0 && invScript.inventory.Count == 0)
            {
                if(tileScript.typeOfFood != "")
                {
                    invScript.AddItem(tileScript.typeOfFood, null);
                }
                return;
            }
        }
        
    }
    public GameObject ClosestTile(GameObject currentPlayer)
    {
        GameObject closest = null;
        float shortestDistance = Mathf.Infinity;
        Vector3 playerPos = currentPlayer.transform.position;
        foreach (GameObject tile in GameObject.FindGameObjectsWithTag("Tile"))
        {
            float sqrDist = (tile.transform.position - playerPos).sqrMagnitude;

            if (sqrDist < shortestDistance)
            {
                shortestDistance = sqrDist;
                closest = tile;
            }
        }
        return closest;
    }
        public GameObject Build(GameObject parent, GameObject prefab, Vector3 position)
        {
            if (parent.name.Contains("Tile"))
                parent.GetComponent<TileScript>().isFull = true;

            GameObject obj = Instantiate(prefab, position, Quaternion.identity);
            obj.transform.SetParent(parent.transform, false);
            obj.transform.localPosition = position;
            obj.name = prefab.name;
            return obj;
        }
}
