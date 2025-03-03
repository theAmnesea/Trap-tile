using UnityEngine;

public class AudioClips : MonoBehaviour
{
    public static AudioClips Instance { get; private set; }

    public AudioClip shootClip, enemyHitClip, bulletHitClip, playerHitClip, coinPickupClip, healthPickupClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
}