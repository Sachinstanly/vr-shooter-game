using UnityEngine;
using UnityEngine.Events; // Include UnityEvents for health change events

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100; // Maximum health of the player
    private int currentHealth; // Current health of the player

    // UnityEvent that can be used to trigger events when health changes
    public UnityEvent onHealthChanged; // Called when health changes

    public UnityEvent onDeath; // Called when the player dies

    private void Start()
    {
        currentHealth = maxHealth; // Set current health to max health at the start
        onHealthChanged.Invoke(); // Invoke health change event initially
    }

    // Method to apply damage to the player
    public void TakeDamage(int damage)
    {
        currentHealth -= damage; // Reduce health by damage amount
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Clamp health to ensure it doesn't go below 0

        onHealthChanged.Invoke(); // Invoke health change event

        if (currentHealth <= 0)
        {
            Die(); // Call die method if health reaches zero
        }
    }

    // Method to heal the player (optional)
    public void Heal(int amount)
    {
        currentHealth += amount; // Increase health by the heal amount
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Clamp health to ensure it doesn't exceed max health

        onHealthChanged.Invoke(); // Invoke health change event
    }

    // Method to handle player death
    private void Die()
    {
        // Handle death (e.g., disable player, trigger animations, etc.)
        Debug.Log("Player has died.");
        onDeath.Invoke(); // Invoke death event
        // You can add more logic here, like restarting the level or displaying a game over screen
    }

    // Optional: Method to get the current health
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    // Optional: Method to get the max health
    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
