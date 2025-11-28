using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    [SerializeField] private HealthHeartBar heartBar;

    void Start()
    {
        currentHealth = maxHealth;

        if (heartBar != null)
        {
            heartBar.DrawHearts();
        }
    }

    public void TakeDamage(int amount)
    {
        int oldHealth = currentHealth;

        currentHealth = Mathf.Max(0, currentHealth - amount);

        if (heartBar != null && currentHealth != oldHealth)
        {
            heartBar.DrawHearts();
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        int oldHealth = currentHealth;

        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);

        if (heartBar != null && currentHealth != oldHealth)
        {
            heartBar.DrawHearts();
        }
    }

    private void Die()
    {
        // TODO: handle death (respawn, game over, etc.)
    }
}
