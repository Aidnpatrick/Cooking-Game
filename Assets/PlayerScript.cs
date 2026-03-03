using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerScript : MonoBehaviour
{
    public GameControlScript gameControlScript;
    public InventoryScript inventoryScript;
    public PlayerScript playerScript;
    public GameObject playerParticle;
    public float partCooldown = 0;

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
    public int playerNumber = 1;
    public Vector3 startinglocation = new Vector3(20, 30, 1);

    void Start()
    {
        transform.position = new Vector3(4,4,0);
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = baseMoveSpeed;
        canMove = false;
    }

    public void StartGame()
    {
        canMove = true;
        StartCoroutine(healthRegen());
    }
    IEnumerator healthRegen()
    {
        while(true)
        {            
            if(health < 10 && healthCoolDown <= 0)
            {
                health = Mathf.Clamp(health + 1, 0, 10);
                healthCoolDown = 5;
            }
            yield return new WaitForSeconds(3f);
        }
    }
    void Update()
    {
        partCooldown -= Time.deltaTime;

        moveInput = Vector2.zero;
        if (canMove && !gameControlScript.ISPAUSED)
        {
            if(playerNumber == 2)
            {
                if (Keyboard.current.iKey.isPressed) moveInput.y += 1;
                if (Keyboard.current.jKey.isPressed) moveInput.y -= 1;
                if (Keyboard.current.kKey.isPressed) moveInput.x -= 1;
                if (Keyboard.current.lKey.isPressed) moveInput.x += 1;
            }
            else
            {
                if (Keyboard.current.wKey.isPressed) moveInput.y += 1;
                if (Keyboard.current.sKey.isPressed) moveInput.y -= 1;
                if (Keyboard.current.aKey.isPressed) moveInput.x -= 1;
                if (Keyboard.current.dKey.isPressed) moveInput.x += 1;
            }
        }

        moveInput = moveInput.normalized;

        if(moveInput.x > 0 || moveInput.y > 0)
        {            

        }

        if (canGrab && Keyboard.current.eKey.wasPressedThisFrame && canMove && inventoryScript.inventory.Count == 0 && !gameControlScript.ISPAUSED)
        {
            inventoryScript.AddItem(currentLoot);
            Destroy(currentLootObject);
            canGrab = false;
        }


        healthCoolDown -= Time.deltaTime;

        if(Keyboard.current.shiftKey.isPressed)
        {
            moveInput = moveInput.normalized * 1.6f;
        }
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
        if(other.name.Contains("Blood") && inventoryScript.currentItem().Contains("Bounty"))
        {
            Destroy(other.gameObject);
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
