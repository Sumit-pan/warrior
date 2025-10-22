using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Animator animator;
    private Rigidbody2D rb;
    public float jumpHeight = 20f;
    private float movement;
    public float speed = 5f;
    private bool facingRight = true;
    public GameObject projectilePrefab;
    private Vector2 moveDirection;
    public int jumpCount = 0;
    public int maxJump = 2;
    [SerializeField] private bool isGrounded = true;

    [Header("Health System")]
    public HealthBar healthBarPrefab;   // assign in Inspector
    private HealthBar healthBarInstance;
    private Health health;

    [Header("Respawn Settings")]
    public Transform[] respawnPoints;     // assign spawn points in Inspector
    public float respawnDelay = 3f;       // seconds before respawning

    public int maxLives = 3;
    private int currentLives;
    private bool isDead = false;

    private Collider2D col;



    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
        col = GetComponent<Collider2D>();
        currentLives = maxLives;

        if (health != null && healthBarPrefab != null)
        {
            Vector3 offset = new Vector3(0, 2.5f, 0);
            healthBarInstance = Instantiate(healthBarPrefab, transform.position + offset, Quaternion.identity);
            healthBarInstance.Initialize(transform, offset);
            health.SetHealthBar(healthBarInstance);
        }
        if (UIManager.Instance != null)
    UIManager.Instance.UpdateLives(currentLives);


    }

    void Update()
    {
        movement = Input.GetAxis("Horizontal");
        moveDirection = new Vector2(movement, 0f);

        // Flip sprite
        if (movement < 0f && facingRight)
        {
            transform.eulerAngles = new Vector3(0f, -180f, 0f);
            facingRight = false;
        }
        else if (movement > 0f && !facingRight)
        {
            transform.eulerAngles = new Vector3(0f, 0f, 0f);
            facingRight = true;
        }

        // Jump input
        if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.W)) && jumpCount < maxJump)
        {
            Jump();
        }

        if (Mathf.Abs(movement) > 0.1f)
        {
            animator.SetFloat("Run", 1f);
        }
        else
        {
            animator.SetFloat("Run", 0f);
        }

        animator.SetBool("jump", !isGrounded);

        if (Input.GetMouseButtonDown(0))
        {
            animator.SetTrigger("attack");
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.playerAttackClip);
        }
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("Launch");
            Launch();
            AudioManager.Instance?.PlaySFX(AudioManager.Instance.playerAttackClip);
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(movement * speed, rb.velocity.y);
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(new Vector2(0f, jumpHeight), ForceMode2D.Impulse);
        jumpCount++;
    }

    void Launch()
    {
        Vector2 launchDir = facingRight ? Vector2.right : Vector2.left;
        Vector2 spawnOffset = new Vector2(facingRight ? 0.6f : -0.6f, 0.2f);
        GameObject projectileObject = Instantiate(projectilePrefab, rb.position + spawnOffset, Quaternion.identity);
        Physics2D.IgnoreCollision(projectileObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.Launch(launchDir, 10f);
        animator.SetTrigger("Launch");
        Debug.Log("Projectile launched in direction: " + launchDir);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = true;
            jumpCount = 0;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = false;
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log("Player died!");
        // Disable controls
        // Play death animation
        if (animator != null)
            animator.SetTrigger("Death");

        // Freeze physics and disable controls
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0;
        }

        if (col != null)
            col.enabled = false;

        this.enabled = false;

        currentLives--;
UIManager.Instance.UpdateLives(currentLives);

        Debug.Log("Lives left: " + currentLives);

        if (currentLives > 0)
            StartCoroutine(RespawnCoroutine());
        else
            StartCoroutine(GameOverCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        // Choose respawn point
        if (respawnPoints != null && respawnPoints.Length > 0)
        {
            int index = Random.Range(0, respawnPoints.Length);
            transform.position = respawnPoints[index].position;
        }
        else
        {
            transform.position = Vector3.zero;
        }

        // Reset health
        if (health != null)
            health.ResetHealth();

        // Restore physics and controls
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1;
        }

        if (col != null)
            col.enabled = true;

        this.enabled = true;
        isDead = false;

        // Reset death trigger if needed
        if (animator != null)
            animator.ResetTrigger("die");
    }

    private IEnumerator GameOverCoroutine()
    {
        yield return new WaitForSeconds(2f); // short delay after final death

        Debug.Log("GAME OVER!");

        // Freeze everything
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Play Game Over animation or show UI here
        if (animator != null)
            animator.SetTrigger("GameOver");
    }

}
