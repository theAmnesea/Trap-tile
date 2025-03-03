using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombHandler : MonoBehaviour
{
    bool isUsed = false;

    public void DestroyBomb()
    {
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (other.gameObject.TryGetComponent<Monster_AKA>(out var mons))
            {
                // Apply damage to the skeleton
                mons.Damage(10);
            }
            if (other.gameObject.TryGetComponent<Skeleton_AKA>(out var skeleton))
            {
                // Apply damage to the skeleton
                skeleton.Damage(10);
            }
        }
        if (other.gameObject.CompareTag("Player") && !isUsed)
        {
            isUsed = true;
            // Decrease Player Health
            PlayerController_AKA.Instance.Damage();
        }
    }
}
