using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow Cam = null;

    private GameObject target;

    public float FollowSpeed = 2f;

    // Transform of the camera to shake. Grabs the gameObject's transform if null.
    private Transform camTransform;

    // How long the object should shake for.
    public float shakeDuration = 0f;

    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.1f;
    public float decreaseFactor = 1.0f;

    [SerializeField, Space(5f)] private Vector3 offset;
    private Vector3 originalPos;

    private void Awake()
    {
        if (Cam == null)
        {
            Cam = this;
        }

        if (camTransform == null)
        {
            camTransform = GetComponent<Transform>();
        }
    }

    private void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnEnable()
    {
        originalPos = camTransform.localPosition;
    }

    private void FixedUpdate()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
            return;
        }

        Vector3 newPosition = target.transform.position;
        newPosition.z = -10;
        newPosition += offset;

        transform.position = Vector3.Slerp(transform.position, newPosition, FollowSpeed * Time.deltaTime);

        if (shakeDuration > 0)
        {
            camTransform.localPosition = originalPos + Random.insideUnitSphere * shakeAmount;

            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
    }

    public void ShakeCamera()
    {
        originalPos = camTransform.localPosition;
        shakeDuration = 0.5f;
    }
}
