using UnityEngine;


public class EnemyScript : MonoBehaviour
{
    public InventoryScript inventoryScript;
    public GameControlScript gameControlScript;

    public GameObject bodyPrefab;
    private GameObject player;
    public int job;
    public float cooldown = 10;
    public int health = 100;
    private float speed = 1.2f;
    public float knifeCooldown = 0.5f;
    float randomSkinColor;
    void Start()
    {
        player = GameObject.Find("Player");
        inventoryScript = GameObject.Find("Inventory").GetComponent<InventoryScript>();
        gameControlScript = GameObject.Find("GameControl").GetComponent<GameControlScript>();
        /*
        if(Random.Range(0f,4f) < 0.5f) {
            Debug.Log("Inspector");
            job = 1;
            gameControlScript.amountOfInspectors++;
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/Inspector");
        }
        else job = 0;
        */
        if(job == 1)
        {
            gameControlScript.amountOfInspectors++;
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/Inspector");
        }

        health = 100;
        knifeCooldown = 0.5f;

        randomSkinColor = Random.Range(0.85f,1f);
        GetComponent<SpriteRenderer>().color = new Color(randomSkinColor, randomSkinColor, randomSkinColor,1f);
        cooldown = 1.5f;
        GetComponent<Rigidbody2D>().linearVelocity = transform.up * Random.Range(6.5f, 10.5f);
    }

    void Update()
    {
        if(inventoryScript == null)
            inventoryScript = GameObject.Find("Inventory").GetComponent<InventoryScript>();

        if(gameControlScript == null)
        gameControlScript = GameObject.Find("GameControl").GetComponent<GameControlScript>();
        if(!gameControlScript.ISPAUSED)
        {
            cooldown -= Time.deltaTime;
            knifeCooldown -= Time.deltaTime;            
        }
        if(health <= 0) {
            GameObject body = gameControlScript.CreateParticle(bodyPrefab, transform.position, 2.5f ,2, false, 0);
            body.GetComponent<SpriteRenderer>().sprite = GetComponent<SpriteRenderer>().sprite;
            body.GetComponent<SpriteRenderer>().color = new Color(randomSkinColor, randomSkinColor, randomSkinColor, 1f);
            gameControlScript.Blood(1, gameObject);
            if(job == 1)
                gameControlScript.amountOfInspectors--;
            Destroy(gameObject);
        }
    }

    void FixedUpdate()
    {
        if(cooldown <= 0 && !gameControlScript.ISPAUSED)
            MoveTowards(player.transform.position);
    }
    public void MoveTowards(Vector3 targetPosition)
    {
        //Vector2 direction = (player.transform.position - transform.position);
        Vector2 direction = (targetPosition - transform.position);
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        
        direction.y = direction.y;
        direction.x = direction.x;

        if (direction.magnitude < 0.1f)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        direction.Normalize();

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        rb.MoveRotation(angle);

        rb.MovePosition(rb.position + direction * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Bullet"))
        {
            gameControlScript.Blood(1, gameObject);
            Destroy(collision.gameObject);
            health -= 20;
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && inventoryScript.currentItem().Contains("Knife"))
        {
            if(knifeCooldown <= 0)
            {
                gameControlScript.Blood(2, gameObject);
                health -= 50;
                knifeCooldown = 0.5f;
            }
        }
        if(collision.CompareTag("Player"))
        {
        }

    }
}
