using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ItemScript : MonoBehaviour
{
    private GameControlScript gameControlScript;
    private InventoryScript inventoryScript;
    public List<string> storage = new List<string>();
    
    void Start()
    {
        gameControlScript = GameObject.Find("GameControl").GetComponent<GameControlScript>();
        inventoryScript = GameObject.Find("Inventory").GetComponent<InventoryScript>();
    }
    void Update()
    {
        for(int i = 0; i < storage.Count; i++)
            storage[i] = storage[i].Replace("1", "");

        gameControlScript.sortIngredients(storage);
        UpdateChildren();
    }
    public int foodCount(string target)
    {
        int counter = 0;
        foreach(string index in storage)
            if(index.Contains(target)) counter++;
        Debug.Log(counter);
        return counter;
    }
    void UpdateChildren()
    {
        
        foreach(Transform child in gameObject.transform)
            Destroy(child.gameObject);

        int index = 0;
        foreach(string stringIndex in storage)
        {
            string foodIndexMain = inventoryScript.findFood(stringIndex);
            GameObject clone = Instantiate(
                Resources.Load<GameObject>(foodIndexMain),
                gameObject.transform
            );
            clone.name = stringIndex;
            clone.name = inventoryScript.orderWords(clone.name);
            clone.transform.position += new Vector3(0,0.07f * index, 0);
            clone.GetComponent<BoxCollider2D>().enabled = false;
            SpriteRenderer cloneRenderer = clone.GetComponent<SpriteRenderer>();
            cloneRenderer.sprite = Resources.Load<Sprite>("Images/" + clone.name.Replace("(Clone)", ""));
            cloneRenderer.sortingOrder = 60 + index;
            index++;
            //Debug.Log("Clone name child: " + clone.name.Replace("(Clone)", ""));
            //clone.transform.position = new Vector3(0,0,1);
        }
        
    }
}
