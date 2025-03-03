using UnityEngine;

public class Bullet : MonoBehaviour
{
    public bool rotateConstantly = false;

    private float speed = 10f;

    private bool isHit = false;

    private Animator animator;

    private AudioSource audioSource;

    private void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        Destroy(gameObject, 4f);
        isHit = false;
    }

    private void Update()
    {
        if (isHit)
        {
            return;
        }

        // Continuously move the bullet upwards
        transform.position += speed * Time.deltaTime * Vector3.up;

        if (rotateConstantly)
        {
            // Rotate the GameObject around its Z-axis
            transform.Rotate(0, 0, speed * 25 * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isHit && collision.gameObject.CompareTag("Enemy"))
        {
            // Set the bullet as hit
            isHit = true;

            // Stop the bullet from moving
            transform.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

            // Play the hit animation
            animator.SetTrigger("IsHit");
            Destroy(gameObject, 1f);

            // Play the hit audio
            audioSource.PlayOneShot(AudioClips.Instance.enemyHitClip);

            // Damage the enemy
            collision.gameObject.GetComponent<Enemy>().TakeDamage(1);
        }
        else if (!isHit && collision.gameObject.CompareTag("EnemyBullet"))
        {
            // Set the bullet as hit
            isHit = true;

            // Stop the bullet from moving
            transform.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;

            // Play the hit animation
            animator.SetTrigger("IsHit");
            Destroy(gameObject, 1f);

            // Play the hit audio
            audioSource.PlayOneShot(AudioClips.Instance.bulletHitClip);

            // Destroy the enemy bullet
            Destroy(collision.gameObject);
        }
    }
}
