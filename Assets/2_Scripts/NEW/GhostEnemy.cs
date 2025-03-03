using System.Collections;
using UnityEngine;

public class GhostEnemy : MonoBehaviour
{
    private GridManager gridManager;
    private float moveInterval = 1.0f; // Seconds between moves
    private int currentRow, currentCol;
    private float tileSize;

    private void Start()
    {
        gridManager = GridManager.Instance;
        tileSize = gridManager.TileSize;
        // Determine initial grid coordinates from world position
        currentCol = Mathf.RoundToInt(transform.position.x / tileSize);
        currentRow = Mathf.RoundToInt(transform.position.y / tileSize);
        StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveInterval);

            // Ghost moves in the same column toward the Swept region.
            // Assuming enemy tiles are in the upper region and Swept tiles in the lower region,
            // the ghost should move downward (decreasing row index).
            int nextRow = currentRow - 1;
            GridTile nextTile = gridManager.GetTileAt(nextRow, currentCol);

            if (nextTile != null)
            {
                // If the next tile is Swept, change it to Unswept (enemy unswept) and destroy the ghost.
                if (nextTile.currentState == GridTile.TileState.Swept)
                {
                    nextTile.SetState(GridTile.TileState.Unswept);
                    Destroy(gameObject);
                    yield break;
                }
                // Otherwise, if the tile is Enemy or Unswept, move into it.
                else if (nextTile.currentState == GridTile.TileState.Enemy || nextTile.currentState == GridTile.TileState.Unswept)
                {
                    Vector3 targetPos = nextTile.transform.position;
                    targetPos.z = 0;
                    transform.position = targetPos;
                    currentRow = nextRow;
                }
                else
                {
                    // In any other unexpected case, destroy the ghost.
                    Destroy(gameObject);
                    yield break;
                }
            }
            else
            {
                // If no tile exists in the next row, destroy the ghost.
                Destroy(gameObject);
                yield break;
            }
        }
    }
}
