using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections;



public class GameControlScript : MonoBehaviour
{
    public bool ISPAUSED = false;
    public AudioSource audioSource;
    public AudioClip pop, hey;
    public CameraScript cameraScript;
    public CameraScript2 cameraScript2;
    public PlayerScript playerScript, player2Script;
    public GameObject startButton;
    public GameObject gameCanvas, menuCanvas, settingsCanvas, levelMainContainer, gameOverCanvas;
    public GameObject foodOrderContainer, levelContainer, verticalContainer, gameOverStatsText;
    public GameObject levelsUI;
    public GameObject instructions;
    public GameObject image;
    public GameObject tilePrefab, emptyTilePrefab, foodOrderPrefab;
    public GameObject textTemplate;
    public GameObject[] foodList;
    public GameObject player, player2;
    public GameObject enemyPrefab, bloodPrefab, gunPrefab, bountyPrefab, knifePrefab;
    public Coroutine enemyDeductionMain;
    public int amountOfInspectors = 0;
    public int points = 0, money = 0;
    public float review = 5;
    public int currentLevel = 0;
    public GameObject[] tiles;

    private List<int[]> levels = new List<int[]>
    {
        //lvl 1
        new int[] 
        {        
            1,8,6,5,7,1,
            1,0,0,0,0,1,
            1,0,0,0,0,1,
            1,0,0,0,0,1,
            2,0,0,0,0,3,
            2,0,0,0,0,3,
            1,0,0,0,0,1,
            1,0,0,0,0,21,
            0,0,0,0,0,1,
            1,1,20,4,1,1
        },
        //lvl 2
        new int[]
        {
            1,1,5,6,1,1,
            21,0,0,0,0,1,
            1,0,0,0,0,1,
            1,0,1,3,3,1,
            1,0,0,0,0,1,
            4,0,0,0,0,1,
            1,2,2,1,0,1,
            20,0,0,0,0,1,
            0,0,0,0,0,1,
            1,1,8,7,1,1,
        },
        //lvl 3
        new int[]
        {
            1,1,1,1,1,1,
            1,0,0,0,0,1,
            1,0,1,3,3,1,
            7,0,1,1,1,1,
            8,0,0,0,0,1,
            1,0,0,2,0,5,
            20,0,0,2,0,6,
            1,0,0,1,0,1,
            0,0,0,0,0,1,
            1,21,1,4,1,1
        },
        //lvl 4
        new int[]
        {
            1,1,1,4,1,1,
            1,0,0,0,0,1,
            5,0,0,0,0,6,
            1,0,2,2,0,1,
            1,0,0,0,0,1,
            20,0,0,0,0,1,
            1,0,3,3,0,1,
            8,0,0,0,0,7,
            0,0,0,0,0,1,
            1,1,1,1,21,1
        }
        ,
        //lvl 5
        new int[] 
        {        
            1,1,1,1,21,1,
            1,0,1,0,0,1,
            3,0,1,0,0,7,
            3,0,1,0,0,8,
            1,0,0,0,0,1,
            1,0,0,0,0,1,
            6,0,0,1,0,2,
            5,0,0,1,0,2,
            0,0,0,1,0,1,
            1,20,4,1,1,1
        }
        ,
        //lvl 6
        new int[] 
        {        
            1,1,1,3,3,1,
            1,0,1,0,0,1,
            7,0,1,1,0,7,
            8,0,0,1,0,8,
            1,0,0,0,0,1,
            1,0,0,0,0,1,
            20,0,4,0,0,6,
            21,0,1,1,0,5,
            0,0,0,1,0,1,
            1,2,2,1,1,1
        }
    };
    
    private string[] foodDictionary = { "Beef", "Cheese", "Bun", "Lettuce", "Gun", "Bounty", "Knife"};

    public List<Dictionary<int, string>> itemsLevelList = new List<Dictionary<int, string>> {
        //level 1
        new Dictionary<int, string>
        {
            {37,"Gun"},
            {54, "Bounty"},
            {19, "Knife"}
        }
        ,
        //level 2
        new Dictionary<int, string>
        {
            {2,"Gun"},
            {40, "Knife"},
            {13, "Bounty"}
        }
        ,
        //level 3
        new Dictionary<int, string>
        {
            {4,"Gun"},
            {5, "Knife"},
            {54, "Bounty"}
        }
        ,
        //level 4
        new Dictionary<int, string>
        {
            {2,"Gun"},
            {5, "Knife"},
            {54, "Bounty"}
        }
        ,
        //level 5
        new Dictionary<int, string>
        {
            {25,"Gun"},
            {4, "Knife"},
            {54, "Bounty"}
        }
        ,
        //level 6
        new Dictionary<int, string>
        {
            {30,"Gun"},
            {7, "Knife"},
            {54, "Bounty"}
            
        }
    };

    public Dictionary<int, string> foodNum = new Dictionary<int, string>()
    {
        {4, "Plate"},
        {5, "Beef"},
        {6, "Cheese"},
        {7, "Bun"},
        {8, "Lettuce"},
    };

    private string[] cannotBeCookedList = {"Lettuce", "Bun"};

    public float foodOrderCooldown = 0, enemySpawnCooldown = 0, peaceTime = 0, policeCooldown = 0;
    public bool canServe = false;
    public bool toggleFoodOrder = false;
    public bool canPlayer2Play = false;
    private string[] bloodSprites = {"Blood1", "Blood2", "Blood3"};

    public bool canBeCooked(string food)
    {
        bool boolean = true;
        foreach(string index in cannotBeCookedList)
            if(food == index) boolean = false;

        return boolean;
    }

    public string[] getfoodDictionary()
    {
        return foodDictionary;
    }

    private List<List<string>> recipes = new List<List<string>>()
    {
        //burger
        new List<string> {"Bun", "Cheese", "BeefCooked", "Bun"},
        new List<string> {"Bun", "LettuceChopped", "BeefCooked", "Bun"},
        new List<string> {"LettuceChopped", "BeefCookedChopped"},
        new List<string> {"Bun", "BeefCooked", "Bun"},
        new List<string> {"BeefCooked"},
        new List<string> {"Bun", "BeefCookedChopped", "Bun"},
        new List<string> {"BeefCookedChopped"},
        new List<string> {"Bun", "BeefCooked", "BeefCooked", "Bun"}
    };

    private List<List<string>> orders = new List<List<string>>()
    {
        
    };

    public void Start()
    {
        ISPAUSED = false;
        gameCanvas.SetActive(false);
        menuCanvas.SetActive(true);
        gameOverCanvas.SetActive(false);
        instructions.SetActive(false);
        levelMainContainer.SetActive(false);
        verticalContainer.SetActive(true);
        player.transform.position = new Vector3(4,4,1);
        player2.transform.position = new Vector3(100,100,1);

        Restart();
        LoadUpMenu();
        GenerateLevel(Random.Range(0,levels.Count));

        playerScript.canMove = false;
        playerScript.canGrab = false;
        player2Script.canMove = false;
        player2Script.canGrab = false;
        canServe = false;
        peaceTime = 25;
    }

    public void SetPlayerCount(int num)
    {
        if(num == 1) canPlayer2Play = false;
        else canPlayer2Play = true;
    }
    public void LoadUpMenu()
    {
        if(levelContainer.transform.childCount >= levels.Count)
            return;
        for(int i = 0; i < levels.Count; i++)
        {
            GameObject levelClone = Instantiate(levelsUI, levelContainer.transform);
            levelClone.transform.GetChild(0).GetComponent<TMP_Text>().text = "Level " + (i + 1).ToString();
            int temp = i;
            
            levelClone.transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>("Images/LevelScreenShots/Level" + (i + 1));
            if(levelClone.transform.GetChild(1).GetComponent<Image>().sprite == null)
                Debug.Log("Images/LevelScreenShots/Level" + (i + 1));
            levelClone.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => Level(temp));
        }
    }
    public void Level(int level)
    {
        StartGame(level);
    }

    public void GenerateLevel(int level = 0)
    {
        int index = 0;
        for(int i = 0; i < 10; i ++)
        {
            for(int j = 0; j < 6; j++)
            {
                Vector3 position = new Vector3(i, j, 1);
                if(levels[level][index] == 0) Instantiate(emptyTilePrefab, position, Quaternion.identity);
                else
                {
                    GameObject tileClone = Instantiate(tilePrefab, position, Quaternion.identity);
                    TileScript tileScript = tileClone.GetComponent<TileScript>();
                    tileScript.typeOfTile = levels[level][index];
                    tileScript.id = index + 1;
                    tileScript.isChosen = false;

                    if(tileScript.typeOfTile >= 4 && tileScript.typeOfTile <= 19)
                    {
                        tileScript.typeOfFood = foodNum[levels[level][index]];
                    }
                    /*
                    if(tileScript.id == 37)
                        cameraScript.Build(tileClone, gunPrefab, new Vector3(0,0,0));
                    if(tileScript.id == 54)
                        cameraScript.Build(tileClone, bountyPrefab, new Vector3(0,0,0));
                    if(tileScript.id == 19)
                        cameraScript.Build(tileClone, knifePrefab, new Vector3(0,0,0));
                    */
                    foreach(KeyValuePair<int, string> dicIndex in itemsLevelList[level])
                    {
                        if(tileScript.id == dicIndex.Key)
                        {
                            cameraScript.Build(
                                tileClone,
                                Resources.Load<GameObject>(dicIndex.Value),
                                new Vector3(0,0,0)
                            );
                            break;

                        }
                    }
                }
                index++;
            }
        }
        
    }

    public void StartGame(int level)
    {
        StopAllCoroutines();

        menuCanvas.SetActive(false);
        gameCanvas.SetActive(true);
        gameOverCanvas.SetActive(false);

        playerScript.canMove = true;
        cameraScript.canEdit = true;
        playerScript.health = 100;
        peaceTime = 25;
        ISPAUSED = false;

        player.transform.position = new Vector3(4,4,0);
        playerScript.StartGame();

        if(canPlayer2Play)
        {
            player2.transform.position = new Vector3(2,2,1);
            player2Script.canMove = true;
            cameraScript2.canEdit = true;
            player2Script.canGrab = true;
            player2Script.health = 100;
            player2Script.StartGame();
        }
        currentLevel = level;

        toggleFoodOrder = false;
        canServe = true;

        Restart();


        GenerateLevel(level);

        for(int i = 0; i < recipes.Count; i++)
            recipes[i] = sortIngredients(recipes[i]);

        enemyDeductionMain = StartCoroutine(enemyDeduction());

    }
    public void GameOver()
    {
        gameOverStatsText.GetComponent<TMP_Text>().text = "Orders Completed:" + points + "\nMoney Made: " + money + " \nRating: " + review;
        gameOverCanvas.SetActive(true);
        player.transform.position = new Vector3(100,100,1);
        player2.transform.position = new Vector3(100,100,1);
        GameObject[] inventorys = GameObject.FindGameObjectsWithTag("Inventory");
        foreach(GameObject index in inventorys)
            index.GetComponent<InventoryScript>().inventory.Clear();
        foreach(GameObject index in player.transform)
            Destroy(index);
        foreach(GameObject index in player2.transform)
            Destroy(index);
        
    }
    public void Restart()
    {
        review = 5;
        points = 0;
        money = 0;
        orders.Clear();

        foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
            Destroy(enemy);
        foreach(GameObject loot in GameObject.FindGameObjectsWithTag("Loot"))
            Destroy(loot);
        foreach(GameObject tile in GameObject.FindGameObjectsWithTag("Tile"))
            Destroy(tile);
        foreach(GameObject tile in GameObject.FindGameObjectsWithTag("Blood"))
            Destroy(tile);
        foreach(GameObject emptyTile in GameObject.FindGameObjectsWithTag("EmptyTile"))
            Destroy(emptyTile);
            
        GameObject[] inventorys = GameObject.FindGameObjectsWithTag("Inventory");
        foreach(GameObject index in inventorys)
            index.GetComponent<InventoryScript>().inventory.Clear();
        foreach(Transform index in player.transform)
            Destroy(index.gameObject);
        foreach(Transform index in player2.transform)
            Destroy(index.gameObject);
        
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;

        foodList = GameObject.FindGameObjectsWithTag("Food");
        tiles = GameObject.FindGameObjectsWithTag("Tile");

        for(int i = 0; i < foodList.Length; i++)
        {
            SpriteRenderer spriteRenderer = foodList[i].GetComponent<SpriteRenderer>();
            string adjusted;
            if(foodList[i].transform.parent == null)
            {
                Debug.Log("Null " +  foodList[i]);
            }
            if(foodList[i].transform.parent.name.Contains("Player"))
                adjusted = foodList[i].name.Substring(0, foodList[i].name.Length - 1);
            else if(!foodList[i].transform.parent.name.Contains("Player"))
                adjusted = foodList[i].name.Replace("(Clone)", "");
            else 
                adjusted = "";
            spriteRenderer.sprite = Resources.Load<Sprite>("Images/" + adjusted);
        }

        if(!ISPAUSED)
        {
            if(canServe)
                peaceTime -= Time.deltaTime;
            
            enemySpawnCooldown -= Time.deltaTime;
            foodOrderCooldown -= Time.deltaTime;
            policeCooldown -= Time.deltaTime;        
        }
        settingsCanvas.SetActive(ISPAUSED);
        

        if(foodOrderCooldown <= 0 && orders.Count < 5 && canServe)
        {
            MakeNewOrder();
            foodOrderCooldown = Random.Range(10,15);
        }

        RefreshOrders();

        if(keyboard.tabKey.wasPressedThisFrame)
        {
            toggleFoodOrder = !toggleFoodOrder;
            foodOrderContainer.SetActive(toggleFoodOrder);
        }

        if(keyboard.pKey.wasPressedThisFrame)
            orders.Clear();
        

        if(keyboard.escapeKey.wasPressedThisFrame && canServe)
            ISPAUSED = !ISPAUSED;

        if(policeCooldown <= 0 && review <= 0)
        {
            Debug.Log("PLLOOOIIICEEE");
            //SpawnEnemy(2);
            StartCoroutine(Police());
            policeCooldown = 100;
        }


    }
    public IEnumerator enemyDeduction()
    {
        while(true)
        {
            if(!ISPAUSED)
            {
                for(int i = 0; i < GameObject.FindGameObjectsWithTag("Enemy").Length; i++)
                    review -= 0.05f;
                if(Random.Range(0f,2f) < 0.1f && peaceTime <= 0)
                    SpawnEnemy(0);
                if(Random.Range(0f,2.5f) < 0.05f && peaceTime <= 0)
                    SpawnEnemy(1);
                
                review -= amountOfInspectors * (GameObject.FindGameObjectsWithTag("Blood").Length / 25.0f);
            }
            yield return new WaitForSeconds(3f);
        }
    }
    IEnumerator Police()
    {
        for(int i = 0; i < 4; i++)
        {
            SpawnEnemy(2);
            yield return new WaitForSeconds(0.3f);
        }
    }

    public GameObject makeTileChosen(GameObject tile)
    {
        GameObject target = null;
        for(int i = 0; i < tiles.Length; i++)
        {
            if(tiles[i] == tile) {
                tiles[i].GetComponent<SpriteRenderer>().color = Color.lightGray;
                target = tiles[i];
            }
            else tiles[i].GetComponent<SpriteRenderer>().color = Color.white;
        }
        return target;
    }

    public void MakeNewOrder()
    {
        int randomDish = Random.Range(0, recipes.Count);
        orders.Add(recipes[randomDish]);
    }

    public void RefreshOrders()
    {
        foreach(Transform i in foodOrderContainer.transform) Destroy(i.gameObject);

        GameObject textTemplateClone = Instantiate(textTemplate, foodOrderContainer.transform);
        textTemplateClone.GetComponent<TMP_Text>().text = "$" + money + "\nPoints: " + points + "\nRating: " + review;    

        foreach(List<string> listIndex in orders)
        {
            GameObject foodOrderClone = Instantiate(foodOrderPrefab, foodOrderContainer.transform);
            foreach(string stringIndex in listIndex)
            {
                GameObject imageClone = Instantiate(image, foodOrderClone.transform);
                var imageCloneRenderer = imageClone.GetComponent<Image>();
                imageCloneRenderer.sprite = Resources.Load<Sprite>("Images/" + stringIndex);
            }
        }

    }
    public List<string> sortIngredients(List<string> storage)
    {
        storage.Sort();
        List<string> buns = storage.Where(s => s.Contains("Bun")).ToList();
        List<string> others = storage.Where(s => !s.Contains("Bun")).ToList();

        storage.Clear();
        if (buns.Count > 0)
        {
            storage.Add(buns[0]);
            others.ForEach(item => storage.Add(item));
            for (int i = 1; i < buns.Count; i++)
            {
                storage.Add(buns[i]);
            }
        }
        else
        {
            storage.AddRange(others);
        }
        return storage;
    }

    public void CheckFood(List<string> servedStorage)
    {
        for(int i = 0; i < orders.Count; i++)
        {
            if(servedStorage.SequenceEqual(orders[i]))
            {
                points++;
                money += servedStorage.Count * 10;
                review = Mathf.Clamp(review + 0.1f, 0f, 5f);

                orders.Remove(orders[i]);
                foodOrderCooldown += 3.5f;
                audioSource.PlayOneShot(pop);
                return;
            }

        }
        Debug.Log("enemy is being spawned");
        SpawnEnemy(0);
        points--;
    }

    public void SpawnEnemy(int role)
    {
        audioSource.PlayOneShot(hey);

        GameObject enemyClone = Instantiate(enemyPrefab, new Vector3(8,0,1), Quaternion.identity);
        enemyClone.GetComponent<EnemyScript>().job = role;
    }
    
    public GameObject ClosestTile(GameObject currentPlayer)
    {
        GameObject closest = null;
        float shortestDistance = Mathf.Infinity;
        Vector3 playerPos = currentPlayer.transform.position;
        foreach (GameObject tile in tiles)
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
    public void Blood(int amount, GameObject target)
    {
        for(int i = 0; i < amount; i++)
        {
            Vector3 randomPosition = new Vector3(
                Random.Range(-0.8f,0.8f),
                Random.Range(-0.8f,0.8f),
                
                1
            );
            GameObject bloodClone = Instantiate(bloodPrefab, target.transform.position + randomPosition, Quaternion.identity);
            bloodClone.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/" + bloodSprites[Random.Range(0, bloodSprites.Length)]);
        }
    }
    public GameObject CreateParticle(GameObject prefab, Vector3 position, float force, float deletion, bool isTrans, float angle)
    {
        GameObject smokeClone = Instantiate(prefab, position, Quaternion.identity);
        if(isTrans)
            smokeClone.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, Random.Range(0.2f,0.75f));
        smokeClone.transform.Rotate(0,0,Random.Range(-angle,angle + 1));
        Rigidbody2D smokeBody = smokeClone.GetComponent<Rigidbody2D>();
        smokeBody.linearVelocity = smokeClone.transform.up * force;
        Destroy(smokeClone, deletion);
        return smokeClone;
    }
}
