using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
     [SerializeField] private float currentHealth;

    private Animator animator;

    public float CurrentHealth => currentHealth;

    public HealthBar healthBar;

    public void SetHealthBar(HealthBar bar)
    {
        healthBar = bar;
        if (healthBar != null)
            healthBar.UpdateHealth(currentHealth / maxHealth);
    }
  

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log(gameObject.name + " took " + amount + " damage. Remaining health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

 void Die()
{
    Debug.Log(gameObject.name + " died!");

    // Trigger animation
    if (animator != null)
        animator.SetTrigger("die");

    // If this GameObject has a Bandit component, let it handle death
    Bandit bandit = GetComponent<Bandit>();
    if (bandit != null)
    {
        bandit.Die(); // Call Bandit's Die() to handle respawn
    }
    else
    {
        // Otherwise destroy normally
        Destroy(gameObject, 1.0f);
    }
}
    
    public void Initialize(float max)
{
    maxHealth = max;
    currentHealth = maxHealth;
}

public void ResetHealth()
{
    currentHealth = maxHealth;
    if (healthBar != null)
        healthBar.UpdateHealth(currentHealth / maxHealth);
}

}
