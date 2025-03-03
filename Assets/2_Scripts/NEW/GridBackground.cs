using UnityEngine;

public class GridBackground : MonoBehaviour
{
    public GameObject tilePrefab;
    public int minRows = 5, maxRows = 12;
    public int minCols = 5, maxCols = 12;
    public float tileSize = 1.0f;

    private void Start()
    {
        GenerateRandomGrid();
    }

    private void GenerateRandomGrid()
    {
        int rows = Random.Range(minRows, maxRows + 1);
        int cols = Random.Range(minCols, maxCols + 1);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Vector2 position = new Vector2(col * tileSize, row * tileSize);
                GameObject tileObj = Instantiate(tilePrefab, position, Quaternion.identity, transform);
                tileObj.name = $"Tile_{row}_{col}";

                tileObj.SetActive(true);

                GridTile.TileState state = (Random.value > 0.5f) ? GridTile.TileState.Swept : GridTile.TileState.Enemy;
                tileObj.GetComponent<GridTile>()?.SetState(state);
            }
        }

        CenterCamera(rows, cols);
    }

    private void CenterCamera(int rows, int cols)
    {
        Camera.main.transform.position = new Vector3((cols - 1) * tileSize / 2, (rows - 1) * tileSize / 2, -10);
        // Camera.main.orthographicSize = Mathf.Max(rows, cols) * 0.6f;
    }
}