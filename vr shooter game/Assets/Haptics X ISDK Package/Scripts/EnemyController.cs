using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    private Transform player; // Reference to the player's transform
    public float minDistanceFromPlayer = 3f; // Minimum distance from the player at which the drone can rotate
    public float maxDistanceFromPlayer = 6f; // Maximum distance from the player at which the drone can rotate

    public float moveSpeed = 2f; // Base move speed
    public float speedVariance = 5f; // Range to randomize speed (0 to -5)
    public float rotationSpeed = 20f; // Base rotation speed around the player
    public float heightVariance = 2f; // Random height variation while rotating
    public float transitionSpeed = 1f; // Speed for smooth height transitions
    public GameObject projectilePrefab; // The projectile to be shot at the player
    public float shootInterval = 4f; // Time interval between each shot
    public float projectileSpeed = 10f; // Speed of the projectile

    private bool isRotating = false; // Is the drone currently rotating around the player?
    private bool isShooting = false; // Is the drone currently shooting projectiles?
    private float randomHeightOffset;
    private float randomRadius;
    private float randomMoveSpeed;
    private float randomRotationSpeed;
    private Vector3 targetPosition; // Target position for smooth movement
    private float stoppingDistanceTolerance = 0.2f; // Tolerance to prevent overshooting

    private void Awake()
    {
        // Get the main camera's transform and set it as the player
        player = Camera.main.transform;
    }

    private void Start()
    {
        // Randomize height, radius, move speed, and rotation speed at the start
        randomHeightOffset = Random.Range(0, heightVariance);
        randomRadius = Random.Range(minDistanceFromPlayer, maxDistanceFromPlayer); // Random radius between min and max
        randomMoveSpeed = Mathf.Clamp(moveSpeed + Random.Range(0, speedVariance), moveSpeed, 10f); // Clamp move speed to prevent too slow/fast movement
        randomRotationSpeed = rotationSpeed + Random.Range(0, speedVariance);
    }

    private void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        // Ensure the drone stays within the max radius range
        if (distanceToPlayer > maxDistanceFromPlayer)
        {
            isRotating = false;
            MoveTowards(player.position); // If beyond max radius, move toward the player
            return;
        }

        // If drone is not rotating, move towards the player until within the random radius
        if (!isRotating)
        {
            if (distanceToPlayer > randomRadius + stoppingDistanceTolerance)
            {
                MoveTowards(player.position);
            }
            else
            {
                StartRotating();
            }
        }
        else
        {
            RotateAroundPlayer();

            // If drone moves beyond max radius while rotating, stop rotating and move back towards player
            if (distanceToPlayer > maxDistanceFromPlayer + stoppingDistanceTolerance)
            {
                isRotating = false;
                StopCoroutine(ShootAtIntervals()); // Stop shooting when drone stops rotating
                isShooting = false;
            }
        }
    }

    private void StartRotating()
    {
        isRotating = true;
        SetTargetHeight(player.position.y + randomHeightOffset); // Set target height for smooth transition

        if (!isShooting) // Start shooting if not already shooting
        {
            StartCoroutine(ShootAtIntervals());
            isShooting = true;
        }
    }

    private void RotateAroundPlayer()
    {
        // Rotate the drone around the player at the random radius
        transform.RotateAround(player.position, Vector3.up, randomRotationSpeed * Time.deltaTime);

        // Smoothly move to the target height based on the random height offset
        float targetY = player.position.y + randomHeightOffset;
        SetTargetHeight(targetY);

        transform.position = Vector3.Lerp(transform.position, targetPosition, transitionSpeed * Time.deltaTime);

        // Ensure the drone is always looking at the player
        transform.LookAt(player);
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        // Move the drone towards the player if it is beyond the random radius
        Vector3 direction = (targetPosition - transform.position).normalized;
        transform.position += direction * randomMoveSpeed * Time.deltaTime;
        transform.LookAt(player);

        // Smoothly transition the height while moving towards the player
        float targetY = player.position.y + randomHeightOffset;
        SetTargetHeight(targetY);
        transform.position = Vector3.Lerp(transform.position, this.targetPosition, transitionSpeed * Time.deltaTime);
    }

    // Helper function to set the target position with smooth height transition
    private void SetTargetHeight(float targetY)
    {
        targetPosition = new Vector3(transform.position.x, targetY, transform.position.z);
    }

    // Coroutine to shoot projectiles at regular intervals
    private IEnumerator ShootAtIntervals()
    {
        while (isRotating)
        {
            ShootProjectile(); // Shoot the projectile
            yield return new WaitForSeconds(shootInterval); // Wait for the interval before shooting again
        }
    }

    private void ShootProjectile()
    {
        if (projectilePrefab != null)
        {
            // Instantiate the projectile at the drone's position
            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);

            // Calculate the direction to the player
            Vector3 directionToPlayer = (player.position - transform.position).normalized;
            float distanceToPlayer = Vector3.Distance(transform.position, player.position);

            // Apply the adjusted speed to the projectile
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
            if (projectileRb != null)
            {
                projectileRb.velocity = directionToPlayer * projectileSpeed;
            }
        }
    }
}
