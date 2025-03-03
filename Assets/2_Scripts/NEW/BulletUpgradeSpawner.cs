using System.Collections;
using UnityEngine;

public class BulletUpgradeSpawner : MonoBehaviour
{
    public static BulletUpgradeSpawner Instance { get; private set; }

    public GameObject[] bulletUpgradePrefab; // Reference to the BulletUpgrade prefab

    public float spawnTime = 30f; // Minimum time before spawning

    private int upgradeCount = 0; // Keeps track of how many upgrades have been spawned

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Start the spawning coroutine
        StartCoroutine(SpawnBulletUpgrades());
    }

    private IEnumerator SpawnBulletUpgrades()
    {
        while (upgradeCount < bulletUpgradePrefab.Length)
        {
            // Random delay between min and max spawn times
            float spawnDelay = Random.Range(spawnTime, spawnTime * 1.2f);
            yield return new WaitForSeconds(spawnDelay);

            // Get the spawn position above the top row of a random column
            Vector3 spawnPosition = GetRandomSpawnPosition();

            // Spawn the BulletUpgrade at the calculated position
            Instantiate(bulletUpgradePrefab[upgradeCount], spawnPosition, Quaternion.identity, transform);

            // Increment the counter for upgrades spawned
            upgradeCount++;

            // Optionally, log each spawn (can be removed)
            Debug.Log("Spawned BulletUpgrade #" + upgradeCount);
        }

        // After spawning all upgrades, stop the spawning process
        Debug.Log("Spawn limit reached. No more upgrades will be spawned.");
    }

    private Vector3 GetRandomSpawnPosition()
    {
        // Pick a random column (within the range of columns)
        int randomColumn = Random.Range(1, GridManager.Instance.Columns - 1);

        // Get the top row (the top row will be at the highest Y position)
        int topRow = GridManager.Instance.Rows;

        // Calculate the spawn position just above the top row (a little offset above)
        float spawnY = topRow + 1f; // Adjust this value to control the spawn height

        // Return the spawn position above the top row in the selected column
        return new Vector3(randomColumn, spawnY, 0);
    }

    public void ReSpawn()
    {
        // Reduce the upgrade count
        if (upgradeCount > 0) --upgradeCount;

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
