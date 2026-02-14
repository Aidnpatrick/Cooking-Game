using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
public class CameraScript : MonoBehaviour
{
    public InventoryScript invScript;
    public PlayerScript playerScript;

    private Vector3 playerCameraPos = new Vector3(0, 0, -5);
    public float interactRange = 3f;

    public GameObject player;
    public GameObject cursorText;

    public bool canEdit = true;
    public bool isOnTile = false;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Keyboard keyboard = Keyboard.current;

        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if(!hit) return;

        if(hit.collider.name.Contains("Tile"))
        {
            Debug.Log("Touched Tile");
            TileScript tileScript = hit.collider.GetComponent<TileScript>();
            string currentItem = invScript.inventory[invScript.equippedItem - 1];

            // if PLAYER is TAKING
            if(Input.GetMouseButtonDown(0) && tileScript.isFull)
            {

                //YES PLATE
                if(currentItem.Contains("Plate"))
                {
                    GameObject itemPlate = invScript.findPlayerTargetChild(currentItem);
                    ItemScript plateScript = itemPlate.GetComponent<ItemScript>();
                    plateScript.storage.Add(hit.collider.transform.GetChild(0).name);
                }
                //NO PLATE
                if(!currentItem.Contains("Plate") && invScript.inventory.Count < 4)
                {
                    invScript.AddItem(hit.collider.transform.GetChild(0).name);
                }
            }
            // if PLAYER is GIVING
            if(Input.GetMouseButtonDown(1) && !tileScript.isFull)
            {
                Debug.Log("Right Clicked Tile.");
                //YES PLATE 
                if(currentItem.Contains("Plate"))
                {
                    ItemScript plateScript = hit.collider.GetComponentInChildren<ItemScript>();
                    plateScript.storage.Add(currentItem.Remove(currentItem.Length - 1));
                }

            }

        }
        
    }
}
