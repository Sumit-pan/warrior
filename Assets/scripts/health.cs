using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    private Animator animator;
    private HealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        healthBar.UpdateHealth(1f);
    }

    public void SetHealthBar(HealthBar bar)
    {
        healthBar = bar;
        if (healthBar != null)
            healthBar.UpdateHealth(currentHealth / maxHealth);
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log($"{gameObject.name} took {amount} damage, HP {currentHealth}/{maxHealth}");

        if (healthBar != null)
            healthBar.UpdateHealth(currentHealth / maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
            healthBar.UpdateHealth(1f);
    }

private void Die()
{
    Debug.Log($"{gameObject.name} died.");

    Bandit bandit = GetComponent<Bandit>();
    Player player = GetComponent<Player>();

    if (bandit != null)
        bandit.Die();
    else if (player != null)
        player.Die();
    else
        Destroy(gameObject);
}

}
