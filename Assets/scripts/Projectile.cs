using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float lifetime = 3f;
    public float damage = 25f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction, float force)
    {
        if (rb == null)
        {
            Debug.LogError("Rigidbody2D missing on projectile!");
            return;
        }
        rb.velocity = direction * force;
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Health target = collision.gameObject.GetComponent<Health>();
        if (target != null)
            target.TakeDamage(damage);

        Destroy(gameObject);
    }
}
