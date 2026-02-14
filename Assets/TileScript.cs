using UnityEngine;

public class TileScript : MonoBehaviour
{
    public bool isFull = false;
    void Start()
    {
        isFull = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.childCount > 0) 
            isFull = true; 
    }
}
