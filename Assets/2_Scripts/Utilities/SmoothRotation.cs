using System.Collections;
using UnityEngine;

public class SmoothRotation : MonoBehaviour
{
    public enum RotationAxis
    {
        X,
        Y,
        Z
    }

    public RotationAxis rotationAxis = RotationAxis.Y; // Dropdown for selecting rotation axis
    public bool randomizeDirection = true; // Randomize rotation direction

    public float rotationSpeed = 10f; // Speed of rotation
    public float momentumSmoothing = 2f; // Smoothing factor for momentum

    private float targetRotationSpeed; // Target rotation speed
    private float currentRotationSpeed; // Current rotation speed
    private float changeDirectionTimer;

    private void Start()
    {
        // Set the initial rotation speed
        currentRotationSpeed = rotationSpeed;
        targetRotationSpeed = rotationSpeed;

        if (randomizeDirection)
        {
            // Start the coroutine to change rotation direction
            StartCoroutine(ChangeDirectionRoutine());
        }
    }

    private void Update()
    {
        // Smoothly interpolate the current speed towards the target speed
        currentRotationSpeed = Mathf.Lerp(currentRotationSpeed, targetRotationSpeed, Time.deltaTime * momentumSmoothing);

        // Apply rotation based on the selected axis
        switch (rotationAxis)
        {
            case RotationAxis.X:
                transform.localRotation *= Quaternion.Euler(currentRotationSpeed * Time.deltaTime, 0, 0);
                break;
            case RotationAxis.Y:
                transform.localRotation *= Quaternion.Euler(0, currentRotationSpeed * Time.deltaTime, 0);
                break;
            case RotationAxis.Z:
                transform.localRotation *= Quaternion.Euler(0, 0, currentRotationSpeed * Time.deltaTime);
                break;
        }
    }

    private IEnumerator ChangeDirectionRoutine()
    {
        while (true)
        {
            // Random time between 3 to 7 seconds
            changeDirectionTimer = Random.Range(3f, 7f);

            // Wait for the timer
            yield return new WaitForSeconds(changeDirectionTimer);

            // Change the direction by inverting the target rotation speed
            targetRotationSpeed = -targetRotationSpeed;
        }
    }
}
