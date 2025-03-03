using UnityEngine;

public class BulletDamage : MonoBehaviour
{
    public int bulletDamage = 1;

    // Detect collisions with other GameObjects
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Event triggered when bullet hits an enemy
            Debug.Log("Bullet hit enemy: " + other.name);

            if (other.gameObject.TryGetComponent<Monster_AKA>(out var mons))
            {
                // Apply damage to the skeleton
                mons.Damage(bulletDamage);
            }
            if (other.gameObject.TryGetComponent<Skeleton_AKA>(out var skeleton))
            {
                // Apply damage to the skeleton
                skeleton.Damage(bulletDamage);
            }

            bulletDamage -= 1;

            if (bulletDamage <= 0)
            {
                Destroy(gameObject);       // Destroy the bullet
            }
        }
    }
}
