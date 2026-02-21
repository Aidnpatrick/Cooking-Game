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
    void Start()
    {
        transform.position = new Vector3(4.5f, 3.3f, -5);
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

        GameObject target = gameControlScript.ClosestTile(player);
        

        if(target == null) return;
        if(!target.name.Contains("Tile")) return;

        
        if(Vector3.Distance(target.transform.position, player.transform.position) >= 3)
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

                    Debug.Log(itemPlate);
                    Debug.Log("Added " + childItemName + "To Pluh");
                    Destroy(target.transform.GetChild(0).gameObject);
                    return;
                }
                
                else if(invScript.inventory.Count == 0)
                {
                    invScript.AddItem(childItemName);
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

                Debug.Log(indexFood);
                Debug.Log(item.name);
                GameObject prefab = Resources.Load<GameObject>(indexFood);
                if(prefab == null)
                {
                    Debug.LogError("This isnt working: " + indexFood + " " + item.name);
                }

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
                Debug.Log("Placed down " + clone.name);
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
                    Debug.Log("Added " + childItemName + "To Food");
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
                Debug.Log("Tried calling");
                if(tileScript.typeOfFood != "")
                {
                    Debug.Log("Called food creation");
                    invScript.AddItem(tileScript.typeOfFood);
                }
                return;
            }

            if(keyboard.eKey.wasPressedThisFrame && target.transform.childCount > 0)
            {
                string childItemName = target.transform.GetChild(0).name;

                if(tileScript.typeOfTile == 2 && !childItemName.Contains("Chopped") && !childItemName.Contains("Plate"))
                {

                    tileScript.StartCooking(1);
                    //target.transform.GetChild(0).name = childItemName.Replace("(Clone)", "") + "Chopped";
                }

                if(tileScript.typeOfTile == 3 && !childItemName.Contains("Cooked") && !childItemName.Contains("Plate"))
                {
                    Debug.Log("Coming from camerascript: " + childItemName);
                    if(gameControlScript.canBeCooked(childItemName))
                        tileScript.StartCooking(5);
                }
                /*
                if(tileScript.typeOfTile == 20 && childItemName.Contains("Plate"))
                {
                    Debug.Log("This food is being checked");
                    gameControlScript.CheckFood(target.GetComponentInChildren<ItemScript>().storage);
                    Destroy(target.transform.GetChild(0).gameObject);
                }
                */
                return;
            }
        }
        

    }

}
