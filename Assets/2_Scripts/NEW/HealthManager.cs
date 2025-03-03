using UnityEngine;

public class HealthManager : MonoBehaviour
{
    public static HealthManager Instance { get; private set; }

    public int playerHealth = 10;
    public GameObject healthPickupPrefab;

    private int maxHealth;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        maxHealth = playerHealth;
    }

    private void Update()
    {
        HandleHealth();
    }

    private void HandleHealth()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i < playerHealth)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void AddHealth()
    {
        playerHealth++;
    }

    public void RemoveHealth()
    {
        playerHealth--;
    }

    public int GetHealth()
    {
        return playerHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }

    public bool SpawnHealthPickup()
    {
        // lower the health, higher the chance
        return Random.value < Mathf.Clamp((maxHealth - playerHealth) / (float)maxHealth, 0f, 1f); ;
    }

    public bool IsLowOnHealth()
    {
        return playerHealth < (int)(maxHealth * 0.8f);
    }
}