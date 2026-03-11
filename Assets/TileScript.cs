using System.Collections;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    private GameControlScript gameControlScript;
    private PlayerScript playerScript;
    private InventoryScript inventoryScript;
    private GameObject player, player2;
    public GameObject smokePrefab, particlePrefab, firePrefab;
    public int id = 0;
    public bool isFull = false;
    public int typeOfTile = 1;
    public string typeOfFood = "";
    public bool isCooking = true, isDone = false;
    public string childItemName = "";
    public float progressTime = 0, smokeCooldown = 0, choppedCooldown = 0, timeAfterDone = 0;
    public bool isFire = false;
    public bool isChosen = false;
    void Start()
    {
        gameControlScript = GameObject.Find("GameControl").GetComponent<GameControlScript>();
        inventoryScript = GameObject.Find("Inventory").GetComponent<InventoryScript>();
        playerScript = GameObject.Find("Player").GetComponent<PlayerScript>();

        player = GameObject.Find("Player");
        player2 = GameObject.Find("Player2");

        isFull = false;
        
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if(typeOfTile == 1) spriteRenderer.sprite = Resources.Load<Sprite>("Images/TableTile");
        if(typeOfTile == 2) spriteRenderer.sprite = Resources.Load<Sprite>("Images/CuttingBoardTableTile");
        if(typeOfTile == 3) spriteRenderer.sprite = Resources.Load<Sprite>("Images/CookingTableTile");
        if(typeOfTile >= 4 && typeOfTile <= 19)
        {
            spriteRenderer.sprite = Resources.Load<Sprite>("Images/" + gameControlScript.foodNum[typeOfTile] + "Storage");
        }
        if(typeOfTile == 20) spriteRenderer.sprite = Resources.Load<Sprite>("Images/ServingTableTile");
        if(typeOfTile == 21) spriteRenderer.sprite = Resources.Load<Sprite>("Images/Trashcan");

    }

    public float cookDuration = 5f;
    
    void Update()
    {
        if(transform.childCount > 0)
        {
            isFull = true;
            Transform child = transform.GetChild(0);
            child.localPosition = new Vector3(0,0,1);
            if(typeOfTile == 20 && child.name.Contains("Plate"))
            {
                gameControlScript.CheckFood(transform.GetComponentInChildren<ItemScript>().storage);
                Destroy(transform.GetChild(0).gameObject);
                return;
            }
            if(typeOfTile == 21)
            {
                if(transform.GetChild(0).name.Contains("Plate"))
                    child.GetComponent<ItemScript>().storage.Clear();

                else
                    Destroy(child.gameObject);

                return;
            }
            if(typeOfTile == 3 && smokeCooldown <= 0)
            {
                /*
                particleScript.CreateParticle(smokePrefab, transform.position, 2, 2, true, 0);
                smokeCooldown = 1f;
                */
            }
        }
        
        if(Vector3.Distance(player.transform.position, transform.position) >= 1.75 && Vector3.Distance(player2.transform.position, transform.position) >= 1.75)
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
            if(!gameControlScript.ISPAUSED)
            {
                progressTime += Time.deltaTime;
                smokeCooldown -= Time.deltaTime;
                choppedCooldown -= Time.deltaTime;
            }


            if(typeOfTile == 2) {}

            if (progressTime >= cookDuration)
                FinishCooking(food);
            if(typeOfTile == 3 && smokeCooldown <= 0)
            {
                gameControlScript.CreateParticle(smokePrefab, transform.position, 2, 2, true, 0);
                smokeCooldown = 1f;

            }
            if(typeOfTile == 2 && choppedCooldown <= 0)
            {
                gameControlScript.CreateParticle(particlePrefab, transform.position, 3, 1.2f, false, 20);
                choppedCooldown = 0.2f;
            }
        }
        else
        {
        }
        if(isDone && transform.childCount > 0)
        {
            timeAfterDone += Time.deltaTime;
            if(timeAfterDone >= 5)
            {
                StartCoroutine(fire());
            }
        }
    }
    private IEnumerator fire()
    {
        while(isFire)
        {
            gameControlScript.CreateParticle(firePrefab, transform.position, 3, 1.3f, false, 20);
            yield return new WaitForSeconds(0.1f);
        }
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Bullet"))
            Destroy(collision.gameObject);
    }
}
