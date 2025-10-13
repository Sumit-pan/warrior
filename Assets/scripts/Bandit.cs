using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bandit : MonoBehaviour
{
    public float moveSpeed = 2f;
    public Transform groundCheck;     // point at the Bandit's feet
    public float groundCheckDistance = 1f;
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    private bool movingLeft = true;
    private Rigidbody2D rb;
    private Animator animator;
    private Health health; // Reference to health system

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health = GetComponent<Health>();
    }

    void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        // Move left or right
        Vector2 moveDir = movingLeft ? Vector2.left : Vector2.right;
        rb.velocity = new Vector2(moveDir.x * moveSpeed, rb.velocity.y);

        // Flip sprite
        transform.localScale = new Vector3(movingLeft ? 1 : -1, 1, 1);

        // Check for ground ahead
        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, groundCheckDistance, groundLayer);
        if (groundInfo.collider == null)
        {
            Flip();
        }

        // Check for wall in front
        RaycastHit2D wallInfo = Physics2D.Raycast(groundCheck.position, moveDir, 0.2f, wallLayer);
        if (wallInfo.collider != null)
        {
            Flip();
        }
    }

    void Flip()
    {
        movingLeft = !movingLeft;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // If the bandit collides with the player, deal damage
        if (collision.gameObject.CompareTag("Player"))
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(10);
                animator.SetTrigger("attack");
            }
        }
    }
}
