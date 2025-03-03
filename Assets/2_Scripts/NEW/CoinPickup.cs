using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    private bool isUsed = false;

    private readonly float speed = 2f;

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

            // Increase Player Health
            ScoreManager.Instance.AddScore(5);
            PlayerController.Instance.PlayCoinPickup();
            Destroy(gameObject);
        }
    }
}
