using UnityEngine;

public class CameraFloat : MonoBehaviour
{
    public float floatStrength = 0.1f;  // How much the camera moves
    public float floatSpeed = 0.5f;    // How fast the camera moves
    private Vector3 originalPosition;

    private void Start()
    {
        // Store the original position of the camera
        originalPosition = transform.position;
    }

    private void Update()
    {
        // Apply the floating effect to the camera's position
        float offsetX = Mathf.Sin(Time.time * floatSpeed) * floatStrength;
        float offsetY = Mathf.Cos(Time.time * floatSpeed) * floatStrength;

        // Update the camera's position with a floating effect
        transform.position = originalPosition + new Vector3(offsetX, offsetY, 0);
    }
}
