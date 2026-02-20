using UnityEngine;
using System.Collections.Generic;

using System.Linq;
using TMPro;
using UnityEngine.InputSystem;



public class GameControlScript : MonoBehaviour
{
    public GameObject foodOrderContainer;
    public GameObject tilePrefab, emptyTilePrefab, foodOrderPrefab;
    public GameObject[] foodList;
    
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
        if(foodOrderCooldown <= 0)
        {
            MakeNewOrder();
            foodOrderCooldown = Random.Range(0,30);
        }

        RefreshOrders();

        if(keyboard.tabKey.wasPressedThisFrame)
        {
            toggleFoodOrder = !toggleFoodOrder;
            foodOrderContainer.SetActive(toggleFoodOrder);
        }

        
    }


    public void makeTileChosen(GameObject tile)
    {
        for(int i = 0; i < tiles.Length; i++)
        {
            if(tiles[i] == tile) tiles[i].GetComponent<SpriteRenderer>().color = UnityEngine.Color.lightGray;
            else tiles[i].GetComponent<SpriteRenderer>().color = UnityEngine.Color.white;
        }
    }

    public void MakeNewOrder()
    {
        int randomDish = Random.Range(0, recipes.Count);
        orders.Add(recipes[randomDish]);
    }

    public void RefreshOrders()
    {
        foreach(Transform i in foodOrderContainer.transform)        Destroy(i.gameObject);
        foreach(List<string> listIndex in orders)
        {
            GameObject foodOrderClone = Instantiate(foodOrderPrefab, foodOrderContainer.transform);
            TMP_Text text = foodOrderClone.transform.GetChild(0).GetComponent<TMP_Text>();
            int randomDish = Random.Range(0, recipes.Count);
            text.text = "";
            foreach(string stringIndex in listIndex)
                text.text += stringIndex + "\n"; 
        }

    }

    public void CheckFood(List<string> servedStorage)
    {
        foreach(List<string> row in orders)
        {
            if(servedStorage.SequenceEqual(row))
            {
                points++;
                return;           
            }
            else
            {
                Debug.Log(servedStorage);
                Debug.Log(row);
            }
        }
        points--;
    }
}
