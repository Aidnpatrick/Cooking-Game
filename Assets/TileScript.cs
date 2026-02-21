using UnityEditor;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    private GameControlScript gameControlScript;
    private PlayerScript playerScript;
    private InventoryScript inventoryScript;
    private GameObject player;
    public GameObject smokePrefab, particlePrefab;
    public bool isFull = false;
    public int typeOfTile = 1;
    public string typeOfFood = "";
    public bool isCooking = true, isDone = false;
    public string childItemName = "";
    public float progressTime = 0, smokeCooldown = 0, choppedCooldown = 0, timeAfterDone = 0;
    public bool isChosen = false;
    void Start()
    {
        gameControlScript = GameObject.Find("GameControl").GetComponent<GameControlScript>();
        inventoryScript = GameObject.Find("Inventory").GetComponent<InventoryScript>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerScript>();

        player = GameObject.Find("Player");

        isFull = false;
        
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if(typeOfTile == 1) spriteRenderer.sprite = Resources.Load<Sprite>("Images/TableTile");
        if(typeOfTile == 2) spriteRenderer.sprite = Resources.Load<Sprite>("Images/CuttingBoardTableTile");
        if(typeOfTile == 3) spriteRenderer.sprite = Resources.Load<Sprite>("Images/CookingTableTile");
        
        if(typeOfTile >= 4 && typeOfTile <= 19)
        {
            spriteRenderer.sprite = Resources.Load<Sprite>("Images/" + gameControlScript.foodNum[typeOfTile] + "Storage");
            Debug.Log("Images/ " + gameControlScript.foodNum[typeOfTile] + "Storage");
        }
        if(typeOfTile == 20) spriteRenderer.sprite = Resources.Load<Sprite>("Images/ServingTableTile");
    }
    public float cookDuration = 5f;
    
    void Update()
    {
        if(transform.childCount > 0)
        {
            isFull = true;
            if(typeOfTile == 20 && transform.GetChild(0).name.Contains("Plate"))
            {
                Debug.Log("This food is being checked");
                gameControlScript.CheckFood(transform.GetComponentInChildren<ItemScript>().storage);
                Destroy(transform.GetChild(0).gameObject);
                return;
            }
        }
        
        if(Vector3.Distance(player.transform.position, transform.position) >= 3)
            GetComponent<SpriteRenderer>().color = Color.white;    

        if (transform.childCount == 0)
        {
            isFull = false;
            isCooking = false;
            isDone = false;
            progressTime = 0;
            return;
        }
        if(transform.childCount > 0 && !isCooking && !isDone)
        {
            if(!transform.GetChild(0).tag.Contains("Food")) return;

            if(typeOfTile == 2)
                StartCooking(1);
            if(typeOfTile == 3 && gameControlScript.canBeCooked(transform.GetChild(0).name))
                StartCooking(5);
        }

        isFull = true;

        GameObject food = transform.GetChild(0).gameObject;

        if (food.CompareTag("Food") && (typeOfTile == 3 || typeOfTile == 2)&& isCooking && !isDone)
        {
            progressTime += Time.deltaTime;
            smokeCooldown -= Time.deltaTime;
            choppedCooldown -= Time.deltaTime;

            if(typeOfTile == 2) playerScript.canMove = false;

            if (progressTime >= cookDuration)
                FinishCooking(food);
            if(typeOfTile == 3 && smokeCooldown <= 0)
            {
                GameObject smokeClone = Instantiate(smokePrefab, transform.position, Quaternion.identity);
                smokeClone.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Random.Range(0f,0.75f));
                Rigidbody2D smokeBody = smokeClone.GetComponent<Rigidbody2D>();
                smokeBody.linearVelocity = smokeClone.transform.up * 2;
                smokeCooldown = 1f;
                Destroy(smokeClone, 2);
            }
            if(typeOfTile == 2 && choppedCooldown <= 0)
            {
                GameObject partClone = Instantiate(particlePrefab, transform.position, Quaternion.identity);
                Rigidbody2D smokeBody = partClone.GetComponent<Rigidbody2D>();
                partClone.transform.Rotate(0,0,Random.Range(-20f,21f));
                smokeBody.linearVelocity = smokeBody.transform.up * 3;
                choppedCooldown = 0.2f;
                Destroy(partClone, 1.2f);
            }
        }
        else
            playerScript.canMove = true;
    }

    private void FinishCooking(GameObject food)
    {
        if (!food.name.Contains("Cooked") && cookDuration == 5)
        {
            string cleanName = food.name.Replace("(Clone)", "");
            food.name = cleanName + "Cooked";
            food.name = inventoryScript.orderWords(food.name);
        }
        else if(!food.name.Contains("Chopped") && cookDuration == 1)
        {
            string cleanName = food.name.Replace("(Clone)", "");
            food.name = cleanName + "Chopped";
            food.name = inventoryScript.orderWords(food.name);
        }

        isDone = true;
        isCooking = false;
    }

    public void StartCooking(int cookingTime, bool isCooking = true)
    {
        progressTime = 0f;
        isDone = false;
        this.isCooking = isCooking;
        cookDuration = cookingTime;
    }

}
