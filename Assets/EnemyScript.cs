using System.Collections.Generic;
using UnityEngine;


public class EnemyScript : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip hey, shooting;
    //public InventoryScript inventoryScript;
    public GameControlScript gameControlScript;

    public GameObject bodyPrefab, bulletPrefab;
    private List<GameObject> players = new List<GameObject>();
    public GameObject shootingTarget;
    public int job;
    public float cooldown = 10;
    public int health = 100;
    private float speed = 1.2f;
    public float knifeCooldown = 0.5f, attackCooldown = 0.5f;
    float randomSkinColor;
    private GameObject destinedTarget = null;
    void Start()
    {
        audioSource.PlayOneShot(hey);
        //inventoryScript = GameObject.Find("Inventory").GetComponent<InventoryScript>();
        gameControlScript = GameObject.Find("GameControl").GetComponent<GameControlScript>();

        if(job == 1)
        {
            gameControlScript.amountOfInspectors++;
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/Inspector");
        }
        if(job == 2)
        {
            health = 170;
            speed = 3;
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Images/Police");
        }

        foreach(GameObject index in GameObject.FindGameObjectsWithTag("Player"))
        {
            if(gameControlScript.canPlayer2Play)
                players.Add(index);
            else
            {
                if(!index.name.Contains("2"))
                    players.Add(index);
            }
        }

        destinedTarget = players[Random.Range(0,players.Count)];

        health = 100;
        knifeCooldown = 0.5f;

        randomSkinColor = Random.Range(0.85f,1f);
        GetComponent<SpriteRenderer>().color = new Color(randomSkinColor, randomSkinColor, randomSkinColor,1f);
        cooldown = 1.5f;
        GetComponent<Rigidbody2D>().linearVelocity = transform.up * Random.Range(6.5f, 10.5f);
    }

    void Update()
    {
    
        //if(inventoryScript == null)
        //    inventoryScript = GameObject.Find("Inventory").GetComponent<InventoryScript>();

        if(gameControlScript == null)
        gameControlScript = GameObject.Find("GameControl").GetComponent<GameControlScript>();
        if(!gameControlScript.ISPAUSED)
        {
            cooldown -= Time.deltaTime;
            knifeCooldown -= Time.deltaTime;        
            attackCooldown -= Time.deltaTime;    
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

        if(attackCooldown <= 0 && job == 2)
        {
            GameObject bulletClone = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            GameObject target = getClosestPlayer();
            Vector3 direction;
            
            if(target == null) 
                direction = transform.position + new Vector3(10,0,0);
            else
                direction = target.transform.position - transform.position;
            
            bulletClone.transform.position += new Vector3(0.5f,0f,0);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bulletClone.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            bulletClone.transform.Rotate(0,0,Random.Range(-5f,6f));
            Rigidbody2D bulletbp = bulletClone.GetComponent<Rigidbody2D>();
            bulletbp.linearVelocity = bulletClone.transform.right * 20;
            bulletClone.name += "Enemy";
            audioSource.PlayOneShot(shooting);
            attackCooldown = 0.15f;
            Destroy(bulletClone, 2f);
        }
    }

    void FixedUpdate()
    {
        if(cooldown <= 0 && !gameControlScript.ISPAUSED)
            MoveTowards(destinedTarget.transform.position);
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
        if(collision.CompareTag("Bullet") && collision.name.Contains("Player"))
        {
            gameControlScript.Blood(1, gameObject);
            Destroy(collision.gameObject);
            health -= 20;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            InventoryScript inventoryScript = GameObject.Find("Inventory").GetComponent<InventoryScript>();
            if(inventoryScript.currentItem().Contains("Knife") && knifeCooldown <= 0)
            {
                gameControlScript.Blood(2, gameObject);
                health -= 40;
                knifeCooldown = 0.5f;
            }
            else
            {
                if(attackCooldown <= 0 && job == 0)
                {
                    gameControlScript.Blood(2, collision.gameObject);
                    collision.GetComponent<PlayerScript>().health -= 15;
                    collision.GetComponent<PlayerScript>().checkHealth();
                    attackCooldown = 1f;
                }
            }
        }
    }

    public GameObject getClosestPlayer()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        GameObject closest = null;
        float closestDistance = Mathf.Infinity;

        foreach(GameObject player in players)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);

            if(distance < closestDistance)
            {
                closestDistance = distance;
                closest = player;
            }
        }

        return closest;
    }
}
