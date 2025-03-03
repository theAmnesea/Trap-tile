using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 1;
    public GameObject bulletPrefab; // Bullet prefab to spawn
    private EnemyManager enemyManager;
    private bool hasNotifiedManager = false;

    public float minFireInterval = 3f; // Minimum interval between shots
    public float maxFireInterval = 6f; // Maximum interval between shots

    private Animator animator;
    private Coroutine firingCoroutine;

    public enum EnemyType
    {
        Ghost,
        Fly,
        Bee
    }

    public EnemyType enemyType = EnemyType.Ghost;

    private void Start()
    {
        animator = GetComponent<Animator>();
        enemyManager = EnemyManager.Instance;

        // Start firing bullets at random intervals
        firingCoroutine = StartCoroutine(FireBulletsAtRandomIntervals());
    }

    private IEnumerator FireBulletsAtRandomIntervals()
    {
        while (true)
        {
            // Wait for a random amount of time between minFireInterval and maxFireInterval
            float fireInterval = Random.Range(minFireInterval, maxFireInterval);
            yield return new WaitForSeconds(fireInterval);

            // Spawn a bullet
            FireBullet();
        }
    }

    private void FireBullet()
    {
        // Instantiate a bullet and set its position
        if (bulletPrefab != null)
        {
            Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        ScoreManager.Instance.AddScore(damage);

        if (health <= 0)
        {
            Die();
        }
        else
        {
            animator.SetTrigger("IsHit");
        }
    }

    private void Die()
    {
        // Stop firing bullets when the enemy dies
        if (firingCoroutine != null)
        {
            StopCoroutine(firingCoroutine);
        }

        if (!hasNotifiedManager && enemyManager != null)
        {
            enemyManager.NotifyEnemyDestroyed(gameObject);
            hasNotifiedManager = true;
        }

        GetComponent<Collider2D>().enabled = false;
        animator.SetTrigger("IsDead");
        Destroy(gameObject, 1f);

        // Random chance to spawn a health pickup if player is low on health
        if (HealthManager.Instance.SpawnHealthPickup())
        {
            Instantiate(HealthManager.Instance.healthPickupPrefab, transform.position, Quaternion.identity);
        }
        else if (Random.value < 0.25f)
        {
            Instantiate(PlayerController.Instance.coinPickup, transform.position, Quaternion.identity);
        }
        // else if (Random.value < 0.1f)
        // {
        //     Instantiate(PlayerController.Instance.bulletSpeedUpgrade, transform.position, Quaternion.identity);
        // }

        switch (enemyType)
        {
            case EnemyType.Ghost:
                WhalepassManager.Instance.DefeatGhostEnemy();
                break;
            case EnemyType.Fly:
                WhalepassManager.Instance.DefeatFlyEnemy();
                break;
            case EnemyType.Bee:
                WhalepassManager.Instance.DefeatBeeEnemy();
                break;
        }
    }
}
