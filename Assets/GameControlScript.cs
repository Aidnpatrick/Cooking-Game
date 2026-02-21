using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.InputSystem;



public class GameControlScript : MonoBehaviour
{
    public GameObject foodOrderContainer;
    public GameObject image;
    public GameObject tilePrefab, emptyTilePrefab, foodOrderPrefab;
    public GameObject textTemplate;
    public GameObject[] foodList;
    public GameObject player;
    
    public int points = 0, money = 0;
    public GameObject[] tiles;
    private int[] map =
    {
        1,8,6,5,7,1,
        1,0,0,0,0,1,
        1,0,0,0,0,1,
        1,0,0,0,0,1,
        2,0,0,0,0,3,
        2,0,0,0,0,3,
        1,0,0,0,0,1,
        1,0,0,0,0,1,
        1,0,0,0,0,1,
        1,1,20,4,1,1,
    };

    private string[] foodDictionary = { "Beef", "Cheese", "Bun", "Lettuce"};

    public Dictionary<int, string> foodNum = new Dictionary<int, string>()
    {
        {4, "Plate"},
        {5, "Beef"},
        {6, "Cheese"},
        {7, "Bun"},
        {8, "Lettuce"},
        {9, " "}
    };

    public string[] cannotBeCookedList = {"Lettuce"};

    public float foodOrderCooldown = 0; 
    public bool toggleFoodOrder = false;

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
        new List<string> {"Bun", "BeefCookedChopped", "Bun"}
    };

    private List<List<string>> orders = new List<List<string>>()
    {
        
    };

    void Start()
    {

        toggleFoodOrder = false;

        int index = 0;
        for(int i = 0; i < 10; i ++)
        {
            for(int j = 0; j < 6; j++)
            {
                Vector3 position = new Vector3(i, j, 1);
                if(map[index] == 0) Instantiate(emptyTilePrefab, position, Quaternion.identity);
                else
                {
                    GameObject tileClone = Instantiate(tilePrefab, position, Quaternion.identity);
                    TileScript tileScript = tileClone.GetComponent<TileScript>();
                    tileScript.typeOfTile = map[index];
                    tileScript.isChosen = false;

                    if(tileScript.typeOfTile >= 4 && tileScript.typeOfTile <= 19)
                    {
                        tileScript.typeOfFood = foodNum[map[index]];
                    }
                }
                index++;
            }
        }
        for(int i = 0; i < recipes.Count; i++)
            recipes[i] = sortIngredients(recipes[i]);

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
            if(foodList[i].transform.parent.name.Contains("Player"))
                adjusted = foodList[i].name.Substring(0, foodList[i].name.Length - 1);
            else if(!foodList[i].transform.parent.name.Contains("Player"))
                adjusted = foodList[i].name.Replace("(Clone)", "");
            else 
                adjusted = "";
            spriteRenderer.sprite = Resources.Load<Sprite>("Images/" + adjusted);
        }
        foodOrderCooldown -= Time.deltaTime;
        if(foodOrderCooldown <= 0 && orders.Count < 5)
        {
            MakeNewOrder();
            foodOrderCooldown = Random.Range(10,26);
        }

        RefreshOrders();

        if(keyboard.tabKey.wasPressedThisFrame)
        {
            toggleFoodOrder = !toggleFoodOrder;
            foodOrderContainer.SetActive(toggleFoodOrder);
        }

        if(keyboard.pKey.wasPressedThisFrame)
            orders.Clear();
        
    }

    public GameObject makeTileChosen(GameObject tile)
    {
        GameObject target = null;
        for(int i = 0; i < tiles.Length; i++)
        {
            if(tiles[i] == tile) {
                tiles[i].GetComponent<SpriteRenderer>().color = UnityEngine.Color.lightGray;
                target = tiles[i];
            }
            else tiles[i].GetComponent<SpriteRenderer>().color = UnityEngine.Color.white;
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
        foreach(Transform i in foodOrderContainer.transform)        Destroy(i.gameObject);

        GameObject textTemplateClone = Instantiate(textTemplate, foodOrderContainer.transform);
        textTemplateClone.GetComponent<TMP_Text>().text = "$" + money + "\nPoints: " + points;        

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
                orders.Remove(orders[i]);
                foodOrderCooldown += 10;
                return;           
            }
            else
            {
                Debug.Log(servedStorage);
            }
        }
        points--;
    }
    public GameObject ClosestTile()
    {
        GameObject closest = null;
        float shortestDistance = Mathf.Infinity;

        Vector3 playerPos = player.transform.position;

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
}
