using UnityEngine;

public class PortalRotation : MonoBehaviour
{
    public float rotationSpeed = 30f; // Speed of rotation in degrees per second

    private void Update()
    {
        // Rotate the portal around its local Z axis
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}
