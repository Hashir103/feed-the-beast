using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int currentHealth;
    public int maxHealth;

    [SerializeField] private HealthHeartBar heartBar;

    private void Awake()
    {
        if (heartBar == null)
            heartBar = FindObjectOfType<HealthHeartBar>();
    }

    void Start()
    {
        currentHealth = maxHealth;

        if (heartBar != null)
        {
            heartBar.playerHealth = this;
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

    private void Die()
    {
        // handle death
    }
}
