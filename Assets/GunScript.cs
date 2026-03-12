using UnityEngine;
using UnityEngine.InputSystem;

public class GunScript : MonoBehaviour
{
    public GameObject player;
    public AudioSource audioSource;
    public AudioClip shoot, cocking, knife;
    public GameObject bulletPrefab;
    public InventoryScript inventoryScript;
    public GameObject closestMain;
    public float cooldown = 0;
    void Start()
    {
        cooldown = 0;
        player = GameObject.Find("Player");
        inventoryScript = GameObject.Find("Inventory").GetComponent<InventoryScript>();
        if(transform.parent != null && !transform.parent.name.Contains("Player"))
            transform.localPosition = new Vector3(0,0,1);
        if(name.Contains("Gun") && transform.parent.name.Contains("Player"))
        audioSource.PlayOneShot(cocking);
        if(name.Contains("Knife") && transform.parent.name.Contains("Player"))
        audioSource.PlayOneShot(knife);
    }

    // Update is called once per frame
    void Update()
    {
        cooldown -= Time.deltaTime;
        if(transform.parent != null && !transform.parent.name.Contains("Player"))
            transform.localPosition = new Vector3(0,0,1);


        if((Keyboard.current.rKey.wasPressedThisFrame || Keyboard.current.hKey.wasPressedThisFrame) && inventoryScript.ammo > 0 && !gameObject.tag.Contains("Loot") && transform.parent != null && transform.parent.name.Contains("Player") && name.Contains("Gun") && cooldown <= 0)
        {
            audioSource.PlayOneShot(shoot);
            GameObject bulletClone = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            GameObject target = GetClosestEnemy(gameObject);
            Vector3 direction;
            
            if(target == null) 
                direction = player.transform.position + new Vector3(10,0,0);
            else
                direction = target.transform.position - transform.position;
            
            bulletClone.transform.position += new Vector3(0.65f,0f,0);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            bulletClone.transform.rotation = Quaternion.Euler(0f, 0f, angle);

            bulletClone.transform.Rotate(0,0,Random.Range(-5f,6f));
            Rigidbody2D bulletbp = bulletClone.GetComponent<Rigidbody2D>();
            bulletbp.linearVelocity = bulletClone.transform.right * 20;
            bulletClone.name += "Player";
            cooldown = 0.25f;
            Destroy(bulletClone, 2f);
        }
        
        if(Keyboard.current.spaceKey.wasPressedThisFrame && transform.parent != null && transform.parent.name.Contains("Player") && name.Contains("Knife"))
        {
            
        }
    }

    public  GameObject GetClosestEnemy(GameObject target)
    {
        GameObject closest = null;
        float shortestDistance = Mathf.Infinity;
        foreach(GameObject index in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if(Vector3.Distance(target.transform.position, index.transform.position) < shortestDistance)
            {
                float sqrDist = (index.transform.position - target.transform.position).sqrMagnitude;

                if (sqrDist < shortestDistance)
                {
                    shortestDistance = sqrDist;
                    closest = index;
                }
            }
        }
        closestMain = closest;
        return closest;
    }
}
