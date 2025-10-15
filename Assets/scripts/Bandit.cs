using UnityEngine;
using System.Collections;
using Random = UnityEngine.Random;

public class Bandit : MonoBehaviour
{
   
    public float moveSpeed = 2f;
    public Transform groundCheck;     
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    private bool movingLeft = true;
    private Rigidbody2D rb;
    private Animator animator;


    public HealthBar healthBarPrefab; // Assign prefab in Inspector
    private Health health;
    private HealthBar healthBarInstance;

    // Flip cooldown to prevent jitter
    private float lastFlipTime;
    public float flipCooldown = 0.2f;

     public Transform[] respawnPoints;   // Assign spawn point in Inspector
    public float respawnDelay = 3f;
    
  public GameObject projectilePrefab;  // Assign in Inspector
public float projectileForce = 10f;
public float attackCooldown = 2f;
private float lastAttackTime;
private bool facingLeft = true;       // Bandit AI facing
public Transform firePoint;           // Where projectile spawns
public float attackRange = 5f;        // Distance to player to attack

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();

        // Setup HealthBar
        if (health != null && healthBarPrefab != null)
        {
            healthBarInstance = Instantiate(
                healthBarPrefab,
                transform.position + new Vector3(0, 1.5f, 0),
                Quaternion.identity,
                transform
            );
            healthBarInstance.Initialize(transform, new Vector3(0, 1.5f, 0));
            health.SetHealthBar(healthBarInstance);
        }
    }

    void Update()
    {
        Patrol();
        ShootPlayer();
    }

    void ShootPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);
        if (distance <= attackRange && Time.time - lastAttackTime >= attackCooldown)
        {
            // Face the player
            if (player.transform.position.x < transform.position.x)
                facingLeft = true;
            else
                facingLeft = false;

            // Flip sprite
            transform.localScale = new Vector3(facingLeft ? 1 : -1, 1, 1);

            // Shoot
            Launch(player.transform.position);

            lastAttackTime = Time.time;
            if (animator != null)
                animator.SetTrigger("attack"); // optional attack animation
        }
    }

 void Launch(Vector2 targetPosition)
{
    if (projectilePrefab == null || firePoint == null) return;

    // Direction toward the target
    Vector2 direction = ((Vector2)targetPosition - (Vector2)firePoint.position).normalized;

    // Spawn projectile at firePoint
    GameObject projectileObject = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

    // Ignore collision with Bandit
    Physics2D.IgnoreCollision(projectileObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());

    // Launch projectile
    Projectile projectile = projectileObject.GetComponent<Projectile>();
    if (projectile != null)
        projectile.Launch(direction, projectileForce);


    Debug.Log("Bandit shot projectile at player: " + direction);
}



    void Patrol()
    {
        Vector2 moveDir = movingLeft ? Vector2.left : Vector2.right;
        rb.velocity = new Vector2(moveDir.x * moveSpeed, rb.velocity.y);

        // Flip sprite visually
        transform.localScale = new Vector3(movingLeft ? 1 : -1, 1, 1);

        // Ground check
        Vector2 checkPos = groundCheck.position;
        RaycastHit2D groundInfo = Physics2D.Raycast(checkPos, Vector2.down, groundCheckDistance, groundLayer);
        Debug.DrawRay(checkPos, Vector2.down * groundCheckDistance, Color.green);

        // Wall check
        RaycastHit2D wallInfo = Physics2D.Raycast(checkPos, moveDir, 0.2f, wallLayer);
        Debug.DrawRay(checkPos, moveDir * 0.2f, Color.red);

        // Flip only if ground ends or wall detected, and respecting cooldown
        if ((groundInfo.collider == null || wallInfo.collider != null) &&
            Time.time - lastFlipTime >= flipCooldown)
        {
            Flip();
        }
    }

    void Flip()
    {
        movingLeft = !movingLeft;
        lastFlipTime = Time.time;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10);
            }
        }
    }

     public void Die()
    {
        if (animator != null)
            animator.SetTrigger("Bandit_die");

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        rb.velocity = Vector2.zero;
        this.enabled = false; // Disable patrol temporarily

        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        // Move to respawn point
           // Choose a random respawn point
    if (respawnPoints.Length > 0)
    {
        int index = UnityEngine.Random.Range(0, respawnPoints.Length);
        transform.position = respawnPoints[index].position;
    }

        // Reset health
        if (health != null)
            health.ResetHealth();

        // Re-enable components
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = true;
        this.enabled = true;
    }

}
