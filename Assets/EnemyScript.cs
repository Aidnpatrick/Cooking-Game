using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private GameControlScript gameControlScript;
    public GameObject bodyPrefab;
    private GameObject player;
    private int health = 100;
    private float speed = 1.2f;
    void Start()
    {
        player = GameObject.Find("Player");
        gameControlScript = GameObject.Find("GameControl").GetComponent<GameControlScript>();

        float randomSkinColor = Random.Range(0.85f,1f);
        GetComponent<SpriteRenderer>().color = new Color(randomSkinColor, randomSkinColor, randomSkinColor,1f);
    }
    void FixedUpdate()
    {
        Vector2 direction = (player.transform.position - transform.position);
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
            if(health <= 0) {
                Debug.Log("Dead");

                gameControlScript.CreateParticle(bodyPrefab, transform.position, 2.5f ,2, false, 0);
                gameControlScript.Blood(1, gameObject);
                Destroy(gameObject);
            }

            
        }
    }
}
