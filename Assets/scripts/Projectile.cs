using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 2f;   // Time before the projectile disappears
    public int damage = 25;       // How much damage it does
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifetime);
    }

    public void Launch(Vector2 direction, float force)
    {
        rb.velocity = direction * force;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Ignore collisions with the player
        if (collision.gameObject.CompareTag("Player"))
            return;

        // Check if it hit an enemy (like the Bandit)
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Health target = collision.gameObject.GetComponent<Health>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
        }

        // Destroy projectile after any valid hit
        Destroy(gameObject);
    }
}
