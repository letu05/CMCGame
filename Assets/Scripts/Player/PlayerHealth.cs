using UnityEngine;

public class PlayerHealth : MonoBehaviour, IcanTakeDamage
{
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        Debug.Log($"[PlayerHealth] Player bị {damageAmount} sát thương. HP còn: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log("[PlayerHealth] Player đã chết.");
        // TODO: game over, animation, v.v.
    }
}
