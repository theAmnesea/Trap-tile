using System.Collections;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    public int gridChangeTime = 60;

    private int rows = 9;
    private int columns = 6;
    private float tileSize = 1.0f;

    public GameObject tilePrefab;
    public GameObject playerTilePrefab;

    public EnemyManager EnemyManager;

    [HideInInspector]
    public int gridWidth, gridHeight;

    private GridTile[,] gridTiles;
    public GridTile[,] GridTiles { get => gridTiles; set => gridTiles = value; }

    public int Rows { get => rows; set => rows = value; }
    public int Columns { get => columns; set => columns = value; }
    public float TileSize { get => tileSize; set => tileSize = value; }

    private GameObject playerTile;

    private int gridChangeCount = 0;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    private void Start()
    {
        gridChangeCount = 0;

        CreateGame();
        StartCoroutine(GameLoop());
    }

    private void Update()
    {
        if (!PlayerController.Instance.IsGamePlaying())
        {
            return;
        }
    }

    private IEnumerator GameLoop()
    {
        while (HealthManager.Instance.GetHealth() > 0)
        {
            yield return new WaitForSeconds(gridChangeTime);
            CreateGame();
        }
    }

    private void AdjustGridSize()
    {
        Rows = Random.Range(5, 12);
        Columns = Random.Range(5, 12);
    }

    private void CreateGame()
    {
        AdjustGridSize();

        gridHeight = Rows;
        gridWidth = Columns;

        EnemyManager.RemoveAllEnemies();

        GenerateGrid();
        PositionCamera();
        PlacePlayerOnTile();

        BulletUpgradeSpawner.Instance.ReSpawn();

        // Ensure player is rendered above tiles
        if (TryGetComponent<SpriteRenderer>(out var playerRenderer))
        {
            playerRenderer.sortingOrder = 1;  // Higher value ensures it appears on top
        }
    }

    private void GenerateGrid()
    {
        // Increment grid change count
        gridChangeCount++;

        // Calculate UnSweptTiles
        int unsweptTiles = transform.childCount > 1 ? UnSweptTiles() : 0;

        // Destroy old tiles
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        GridTiles = new GridTile[gridHeight, gridWidth];

        for (int row = 0; row < Rows; row++)
        {
            for (int column = 0; column < Columns; column++)
            {
                Vector2 position = new(column * TileSize, row * TileSize);
                GameObject tileObj = Instantiate(tilePrefab, position, Quaternion.identity);
                tileObj.transform.SetParent(transform);
                tileObj.SetActive(true);
                tileObj.name = "R" + row + " C" + column;

                GridTile tile = tileObj.GetComponent<GridTile>();
                tile.transform.position = position;
                tile.SetState(row < Rows / 2 ? GridTile.TileState.Swept : GridTile.TileState.Enemy);

                // Randomly distribute the unswept tiles after the first row is placed
                if (row > 0 && unsweptTiles > 0 && tile.currentState == GridTile.TileState.Swept)
                {
                    if (Random.Range(0, (gridWidth + gridHeight) / 2) == 0)
                    {
                        tile.SetState(GridTile.TileState.Unswept);
                        unsweptTiles--;
                    }
                }

                GridTiles[row, column] = tile;

                // Set tile sorting order to be lower than player
                if (tileObj.TryGetComponent<SpriteRenderer>(out var tileRenderer))
                {
                    tileRenderer.sortingOrder = 0;  // Ensure tiles are rendered below the player
                }
            }
        }
    }

    private void PlacePlayerOnTile()
    {
        if (playerTile == null)
        {
            playerTile = Instantiate(playerTilePrefab);
            playerTile.SetActive(true);
            playerTile.GetComponent<PlayerController>().gridManager = this;
        }

        int randomRow = 0;
        int randomCol = Columns / 2;

        playerTile.GetComponent<PlayerController>().SetPosition(randomRow, randomCol);
    }

    private void PositionCamera()
    {
        Camera.main.transform.position = new Vector3(Columns / 2, Rows / 2, -10);
        Camera.main.orthographicSize = Rows / 2 + 2;
    }

    public GridTile GetTileAt(int row, int column)
    {
        if (row >= 0 && row < Rows && column >= 0 && column < Columns)
        {
            return GridTiles[row, column];
        }
        return null;
    }

    private int UnSweptTiles()
    {
        try
        {
            int count = 0;
            for (int i = 0; i < gridHeight; i++)
            {
                for (int j = 0; j < gridWidth; j++)
                {
                    if (GridTiles[i, j].currentState == GridTile.TileState.Unswept)
                    {
                        count++;
                    }
                }
            }
            return count;
        }
        catch (System.Exception)
        {
            Debug.Log("Error in UnSweptTiles");
            return 0;
        }
    }
}
