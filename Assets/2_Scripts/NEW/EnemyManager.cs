using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public GameObject monster_1_prefab;  // Prefab for Monster 1
    public GameObject monster_2_prefab;  // Prefab for Monster 2
    public GameObject GhostPrefab;       // Prefab for Ghost

    public GridManager gridManager;      // Reference to the grid manager to access tiles

    private List<GameObject> activeEnemies = new List<GameObject>();
    private int maxEnemies = 0;
    private bool isSpawning = false;
    private float spawnDelay = 2f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Calculate maxEnemies initially (or update dynamically if grid changes)
        maxEnemies = CalculateMaxEnemies();
        Debug.Log("Starting EnemyManager. maxEnemies: " + maxEnemies);

        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {

        // If already spawning, do not start another routine
        if (isSpawning)
            yield break;
        // Mark that a spawn routine is running
        isSpawning = true;
        Debug.Log("SpawnEnemies started. ActiveEnemies: " + activeEnemies.Count);

        yield return new WaitForSeconds(spawnDelay);

        // ReCalculate max if grid changes. /// once at start>inside start()/// (or you can update it dynamically if grid changes)> here
        maxEnemies = CalculateMaxEnemies();

        // <<-- NEW LINE: Clean up any destroyed enemy references before iterating -->>
        activeEnemies.RemoveAll(item => item == null);

        // Ensure gridManager and its grid tiles exist
        // Check that gridManager and its GridTiles are valid
        if (gridManager == null)
        {
            Debug.LogError("GridManager is null!");
            isSpawning = false;
            yield break;
        }
        if (gridManager.GridTiles == null)
        {
            Debug.LogError("GridTiles array is null!");
            isSpawning = false;
            yield break;
        }

        // Build a list of tiles marked for enemy spawn (state == Enemy)
        List<GridTile> enemyTiles = new List<GridTile>();
        for (int i = 0; i < gridManager.gridHeight; i++)
        {
            for (int j = 0; j < gridManager.gridWidth; j++)
            {
                if (gridManager.GridTiles[i, j].currentState == GridTile.TileState.Enemy)
                {
                    enemyTiles.Add(gridManager.GridTiles[i, j]);
                }
            }
        }
        Debug.Log("Total enemyTiles: " + enemyTiles.Count);

        // Filter out tiles that are already occupied by an enemy
        List<GridTile> availableTiles = new List<GridTile>();
        foreach (GridTile tile in enemyTiles)
        {
            bool occupied = false;
            foreach (GameObject enemy in activeEnemies)
            {
                if (enemy == null)
                    continue;
                // Using a threshold to compare positions
                if (Vector3.Distance(enemy.transform.position, tile.transform.position) < 0.1f)
                {
                    occupied = true;
                    break;
                }
            }
            if (!occupied)
                availableTiles.Add(tile);
        }
        Debug.Log("Available tiles after filtering: " + availableTiles.Count);

        // Determine how many enemies we can spawn now
        int spawnableCount = maxEnemies - activeEnemies.Count;
        Debug.Log("SpawnableCount: " + spawnableCount);

        // Shuffle the available tiles list for random spawn positions
        availableTiles = ShuffleList(availableTiles);

        // Spawn on as many available tiles as possible (up to spawnableCount)
        for (int k = 0; k < Mathf.Min(spawnableCount, availableTiles.Count); k++)
        {
            Vector3 spawnPosition = availableTiles[k].transform.position;
            spawnPosition.z = 0;

            // Randomly choose one of the three enemy types
            int prefabChoice = Random.Range(0, 3);  // 0, 1, or 2
            GameObject enemyPrefab = null;
            if (prefabChoice == 0)
                enemyPrefab = monster_1_prefab;
            else if (prefabChoice == 1)
                enemyPrefab = monster_2_prefab;
            else
                enemyPrefab = GhostPrefab;

            GameObject enemy = Instantiate(enemyPrefab, spawnPosition, Quaternion.identity, transform);
            activeEnemies.Add(enemy);
            Debug.Log("Spawned enemy at " + spawnPosition + ". ActiveEnemies now: " + activeEnemies.Count);

        }

        // Spawn routine is done
        isSpawning = false;
        // If we still have room for more enemies, start another spawn cycle
        // Continue to spawn more enemies if under the limit
        if (activeEnemies.Count < maxEnemies)
        {
            Debug.Log("ActiveEnemies (" + activeEnemies.Count + ") less than maxEnemies (" + maxEnemies + "). Restarting spawn cycle.");
            StartCoroutine(SpawnEnemies());
        }
    }

    public void SetGridManager(GridManager newGridManager)
    {
        gridManager = newGridManager;
        // Optionally restart the spawning routine:
        if (activeEnemies.Count < maxEnemies && !isSpawning)
        {
            StartCoroutine(SpawnEnemies());
        }
    }

    // Fisher-Yates shuffle algorithm for the enemy tiles list
    private List<GridTile> ShuffleList(List<GridTile> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            GridTile temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
        return list;
    }

    private int CalculateMaxEnemies()
    {
        // For example, maximum enemy count is defined as half the square root of total grid cells.
        if (gridManager.gridWidth >= 3)
        {
            int totalAllowedEnemies = (int)Mathf.Sqrt(gridManager.gridWidth * gridManager.gridHeight) / 2;
            return Mathf.Max(totalAllowedEnemies, 0);
        }

        return 0;
    }

    public void RemoveAllEnemies()    // Call this to remove all enemies from the scene

    {
        Debug.Log("RemoveAllEnemies called.");
        foreach (GameObject enemy in activeEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        activeEnemies.Clear();
        Debug.Log("All enemies removed. ActiveEnemies count: " + activeEnemies.Count);

        // Ensure spawn routine runs if needed
        if (activeEnemies.Count < maxEnemies && !isSpawning)
        {
            Debug.Log("ActiveEnemies less than maxEnemies after removal. Restarting spawn cycle.");
            StartCoroutine(SpawnEnemies());
        }
    }

    public void RemoveEnemy(GameObject enemy)     // Call this when a single enemy is killed

    {
        Debug.Log("RemoveEnemy called.");
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            Debug.Log("Enemy removed. ActiveEnemies count: " + activeEnemies.Count);
        }
        Destroy(enemy);

        // Check and start spawning if enemy count is below maximum
        if (activeEnemies.Count < maxEnemies && !isSpawning)
        {
            Debug.Log("ActiveEnemies less than maxEnemies after removal. Starting spawn cycle.");
            StartCoroutine(SpawnEnemies());
        }
    }

    public void NotifyEnemyDestroyed(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
            Debug.Log("Enemy removed via OnDestroy. ActiveEnemies count: " + activeEnemies.Count);
            if (activeEnemies.Count < maxEnemies && !isSpawning)
            {
                StartCoroutine(SpawnEnemies());
            }
        }
    }

}
