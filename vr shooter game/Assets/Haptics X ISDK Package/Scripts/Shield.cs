using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Shield : MonoBehaviour
{
    public GameObject shieldObject; // The GameObject representing the shield
    public Animator animator; // Animator for controlling shield animations
    public AnimationClip animationClip; // The animation clip for determining duration
    public BoxCollider boxCollider;
    private float animationDuration; // Duration of the animation
    private bool isShieldActive = false; // Is the shield currently active?
    public float shieldActiveDuration = 10;

    public UnityEvent onShieldOpen;
    public UnityEvent onShieldClose;
    public UnityEvent onShieldHit;

    public float AnimationDutation { get => animationDuration; }

    private void Start()
    {
        animationDuration = animationClip.length; // Get the length of the animation clip

        // Ensure the shield is initially disabled
        if (shieldObject != null)
        {
            shieldObject.SetActive(false);
            boxCollider.isTrigger = true;
        }
    }

    // Method to open the shield
    public void OpenShield()
    {
        if (!isShieldActive)
        {
            isShieldActive = true; // Set shield as active
            shieldObject.SetActive(true); // Enable the shield object
            boxCollider.isTrigger = false;

            onShieldOpen?.Invoke();
            // Set the animator parameter to open the shield
            animator.SetBool("OpenShield", true);

            // Start the shield opening coroutine
            StartCoroutine(ScaleShield(0f, 1f, animationDuration));

            // Automatically close the shield after the specified time
            Invoke("CloseShield", shieldActiveDuration); // You can adjust this timing if needed
        }
    }

    // Method to close the shield
    public void CloseShield()
    {
        if (isShieldActive)
        {
            onShieldClose?.Invoke();
            // Set the animator parameter to close the shield
            animator.SetBool("OpenShield", false);
            StartCoroutine(ScaleShield(1f, 0f, animationDuration)); // Close shield over the same duration
        }
    }

    // Coroutine to scale the shield from startScale to endScale over the duration
    private IEnumerator ScaleShield(float startScale, float endScale, float duration)
    {
        float elapsedTime = 0f;
        Vector3 originalScale = shieldObject.transform.localScale;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration; // Calculate interpolation factor (0 to 1)
            float currentScale = Mathf.Lerp(startScale, endScale, t); // Lerp between start and end scale
            shieldObject.transform.localScale = new Vector3(currentScale, currentScale, currentScale); // Scale uniformly

            elapsedTime += Time.deltaTime; // Increment elapsed time
            yield return null; // Wait for the next frame
        }

        // Ensure the final scale is set
        shieldObject.transform.localScale = new Vector3(endScale, endScale, endScale);

        // Disable the shield object when closing
        if (endScale == 0f && isShieldActive)
        {
            isShieldActive = false;
            shieldObject.SetActive(false);
            boxCollider.isTrigger = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("<<< colioded with " + collision.gameObject.name);
        if (collision.gameObject.GetComponent<Projectile>() != null)
        {
            onShieldHit?.Invoke();
        }
    }
}
