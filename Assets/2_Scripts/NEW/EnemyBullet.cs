using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed = 10f; // Speed at which the bullet will move

    private bool isHit = false;

    private void Start()
    {
        Destroy(gameObject, 4f);
        isHit = false;
    }

    private void Update()
    {
        if (isHit)
        {
            return;
        }

        // Move the bullet downwards
        transform.position += speed * Time.deltaTime * Vector3.down;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isHit && collision.gameObject.CompareTag("Player"))
        {
            // Set the bullet as hit
            isHit = true;

            // Damage the player
            PlayerController.Instance.Damage();

            Destroy(gameObject);
        }
    }
}
