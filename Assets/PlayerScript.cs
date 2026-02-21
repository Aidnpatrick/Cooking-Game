using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    public InventoryScript inventoryScript;

    private Vector2 moveInput;
    private Rigidbody2D rb;

    public bool canMove = true;
    public bool canGrab = false;

    public float baseMoveSpeed = 5f;
    private float moveSpeed;

    private string currentLoot = "";
    private GameObject currentLootObject = null;

    public float health = 10;
    public float healthCoolDown = 5;
    public Vector3 startinglocation = new Vector3(20, 30, 1);

    void Start()
    {
        transform.position = new Vector3(4,4,0);
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = baseMoveSpeed;
    }

    public void StartGame()
    {
        gameObject.SetActive(true);
    }

    void Update()
    {
        moveInput = Vector2.zero;

        if (canMove)
        {
            if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
            if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
            if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
            if (Keyboard.current.dKey.isPressed) moveInput.x += 1;
        }

        moveInput = moveInput.normalized;

        if (canGrab && Keyboard.current.eKey.wasPressedThisFrame && canMove && inventoryScript.inventory.Count == 0)
        {
            inventoryScript.AddItem(currentLoot);
            Destroy(currentLootObject);
            canGrab = false;
        }
        
        healthCoolDown -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Loot"))
        {
            currentLoot = other.gameObject.name.Replace("Loot", "");
            currentLootObject = other.gameObject;
            canGrab = true;
        }

        if (other.CompareTag("Tile"))
        {
            
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        
        if (other.CompareTag("Loot"))
        {
            canGrab = false;
        }
        
            
        if (other.CompareTag("Tile"))
        {
            moveSpeed = baseMoveSpeed;
        }
    }
}
