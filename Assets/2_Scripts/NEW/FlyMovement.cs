using UnityEngine;

public class FlyMovement : MonoBehaviour
{
    public float speed = 3f; // Speed of movement
    public float jitterIntensity = 0.5f; // Small random fluctuations in movement
    public float changeDirectionTime = 1.5f; // Time before changing direction
    public float boundaryRadius = 3f; // Movement boundary within parent

    private Vector2 targetPosition;
    private Transform parentTransform;
    private float timer;

    void Start()
    {
        parentTransform = transform.parent;
        PickNewTargetPosition();
    }

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= changeDirectionTime)
        {
            PickNewTargetPosition();
            timer = 0f;
        }

        // Apply natural jitter effect to simulate erratic fly movement
        Vector2 jitter = new Vector2(Random.Range(-jitterIntensity, jitterIntensity), Random.Range(-jitterIntensity, jitterIntensity));

        transform.position = Vector2.Lerp(transform.position, targetPosition + jitter, speed * Time.deltaTime);
    }

    void PickNewTargetPosition()
    {
        Vector2 randomOffset = Random.insideUnitCircle * boundaryRadius;
        targetPosition = (Vector2)parentTransform.position + randomOffset;
    }
}