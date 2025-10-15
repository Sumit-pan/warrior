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

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
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
        }
        if (Input.GetMouseButtonDown(1))
        {
            animator.SetTrigger("Launch");
            Launch();
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
    // Decide direction based on facing
    Vector2 launchDir = facingRight ? Vector2.right : Vector2.left;

    // Offset spawn position slightly in front of player
    Vector2 spawnOffset = new Vector2(facingRight ? 0.6f : -0.6f, 0.2f);

    // Spawn projectile
    GameObject projectileObject = Instantiate(projectilePrefab, rb.position + spawnOffset, Quaternion.identity);

    // Ignore collision between player and projectile
    Physics2D.IgnoreCollision(projectileObject.GetComponent<Collider2D>(), GetComponent<Collider2D>());

    // Launch projectile
    Projectile projectile = projectileObject.GetComponent<Projectile>();
    projectile.Launch(launchDir, 10f); // adjust force if too slow or too fast

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
}
