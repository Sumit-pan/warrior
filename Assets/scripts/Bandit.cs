using UnityEngine;
using System.Collections;

public class Bandit : MonoBehaviour
{
    [Header("Combat Settings")]
    public float moveSpeed = 2f;
    public float attackRange = 5f;
    public float chaseRange = 8f;
    public float projectileForce = 10f;
    public float attackCooldown = 2f;
    private float lastAttackTime;

    [Header("References")]
    public Transform groundCheck;
    public float groundCheckDistance = 0.2f;
    public LayerMask groundLayer;
    public LayerMask wallLayer;
    public Transform firePoint;
    public GameObject projectilePrefab;
    public HealthBar healthBarPrefab;
    private HealthBar healthBarInstance;
    private Health health;
    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D col;
    private GameObject player;

    [Header("Animation Settings")]
    public float deathAnimDuration = 1.5f;

    [HideInInspector] public WaveManager waveManager;

    private bool movingLeft = true;
    private bool isDead = false;
    private float lastFlipTime;
    public float flipCooldown = 0.2f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
    }

    void OnEnable()
    {
        // Reset everything important each time this Bandit is spawned
        isDead = false;

        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1;
            rb.velocity = Vector2.zero;
        }

        if (col != null)
        {
            col.enabled = true;
            col.isTrigger = false;
        }

        if (health != null)
            health.ResetHealth();

        if (animator != null)
        {
            animator.ResetTrigger("Bandit_die");
        }
    }

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        if (health != null && healthBarPrefab != null)
        {
            Vector3 offset = new Vector3(0, 2.5f, 0);
            healthBarInstance = Instantiate(healthBarPrefab, transform.position + offset, Quaternion.identity);
            healthBarInstance.Initialize(transform, offset);
            health.SetHealthBar(healthBarInstance);
        }
    }

    void Update()
    {
        if (isDead) return;
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.transform.position);

        if (distance <= attackRange)
        {
            rb.velocity = Vector2.zero;
            FacePlayer();
            ShootPlayer();
        }
        else if (distance <= chaseRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    void Patrol()
    {
        Vector2 moveDir = movingLeft ? Vector2.left : Vector2.right;
        rb.velocity = new Vector2(moveDir.x * moveSpeed, rb.velocity.y);
        transform.localScale = new Vector3(movingLeft ? 1 : -1, 1, 1);

        Vector2 checkPos = groundCheck.position;
        RaycastHit2D groundInfo = Physics2D.Raycast(checkPos, Vector2.down, groundCheckDistance, groundLayer);
        RaycastHit2D wallInfo = Physics2D.Raycast(checkPos, moveDir, 0.2f, wallLayer);

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

    void FacePlayer()
    {
        if (player == null) return;
        bool playerIsLeft = player.transform.position.x < transform.position.x;
        transform.localScale = new Vector3(playerIsLeft ? 1 : -1, 1, 1);
    }

    void ChasePlayer()
    {
        if (player == null) return;
        FacePlayer();
        float direction = player.transform.position.x < transform.position.x ? -1 : 1;
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
    }

    void ShootPlayer()
    {
        if (Time.time - lastAttackTime < attackCooldown) return;
        if (projectilePrefab == null || firePoint == null) return;

        FacePlayer();

        Vector2 direction = ((Vector2)player.transform.position - (Vector2)firePoint.position).normalized;
        GameObject projectileObject = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Physics2D.IgnoreCollision(projectileObject.GetComponent<Collider2D>(), col);

        Projectile projectile = projectileObject.GetComponent<Projectile>();
        if (projectile != null) projectile.Launch(direction, projectileForce);

        lastAttackTime = Time.time;
        if (animator != null) animator.SetTrigger("attack");
        AudioManager.Instance?.PlaySFX(AudioManager.Instance.banditAttackClip);
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        rb.velocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
        col.enabled = false;

        if (animator != null)
            animator.SetTrigger("Bandit_die");

        StartCoroutine(DestroyAfterDeath());
    }

    private IEnumerator DestroyAfterDeath()
    {
        yield return new WaitForSeconds(deathAnimDuration);

        if (waveManager != null)
            waveManager.RegisterBanditKill();

        if (healthBarInstance != null)
            Destroy(healthBarInstance.gameObject);

        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
                playerHealth.TakeDamage(10);
        }
    }
}
