using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    public ParticleSystem firstHitEffect;  // Particle effect for the first hit
    public ParticleSystem secondHitEffect; // Particle effect for the second hit
    public float destroyDelay = 3f;        // Time delay before destroying the enemy on second hit

    private int hitCount = 0; // Track the number of hits the enemy takes

    public UnityEvent OnKilled;

    public void ApplyDamage()
    {
        hitCount++; // Increment hit count

        //// Destroy the arrow object
        //Destroy(other.gameObject);

        // Handle hits
        if (hitCount == 1)
        {
            // Play the first hit particle effect
            if (firstHitEffect != null && !firstHitEffect.isPlaying)
            {
                firstHitEffect.Play();
            }
        }
        else if (hitCount == 2)
        {
            // Play the second hit particle effect
            if (secondHitEffect != null && !secondHitEffect.isPlaying)
            {
                secondHitEffect.Play();
            }

            // Destroy the enemy after a delay
            Destroy(gameObject, destroyDelay);
        }
    }

    private void OnDestroy()
    {
        OnKilled.Invoke();
    }
}
