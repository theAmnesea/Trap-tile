using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    private bool isUsed = false;

    private readonly float speed = 2f;

    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Move the pickup downwards
        transform.position += speed * Time.deltaTime * Vector3.down;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !isUsed)
        {
            isUsed = true;

            // Play pickup audio
            audioSource.PlayOneShot(AudioClips.Instance.healthPickupClip);

            // Increase Player Health
            HealthManager.Instance.AddHealth();
            Destroy(gameObject);

            WhalepassManager.Instance.PickupHealth();
        }
    }
}
