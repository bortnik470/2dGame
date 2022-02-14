using UnityEngine;

public class DamageScript : MonoBehaviour
{
    [SerializeField] private int health = 100;
    [SerializeField] private HealthBar healthBar = null;

    private int currentHealth;

    public bool isDeath = false;

    private void Start()
    {
        currentHealth = health;
        if(healthBar)
        {
            healthBar.SetHealth(health);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (healthBar) { healthBar.ChangeHealth(currentHealth); }

        if (currentHealth <= 0) { isDeath = true; }
    }
}