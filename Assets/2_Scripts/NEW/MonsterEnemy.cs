using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEnemy : MonoBehaviour
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

            // Define potential moves: up, down, right, left
            List<Vector2Int> possibleMoves = new List<Vector2Int>
            {
                new Vector2Int(currentRow + 1, currentCol), // Up
                new Vector2Int(currentRow - 1, currentCol), // Down
                new Vector2Int(currentRow, currentCol + 1), // Right
                new Vector2Int(currentRow, currentCol - 1)  // Left
            };

            // Filter valid moves: tile exists and state is either Enemy or Unswept (not Swept)
            List<Vector2Int> validMoves = new List<Vector2Int>();
            foreach (Vector2Int move in possibleMoves)
            {
                GridTile tile = gridManager.GetTileAt(move.x, move.y);
                if (tile != null && (tile.currentState == GridTile.TileState.Enemy || tile.currentState == GridTile.TileState.Unswept))
                {
                    validMoves.Add(move);
                }
            }

            // If there is at least one valid move, choose one randomly and move there.
            if (validMoves.Count > 0)
            {
                Vector2Int chosenMove = validMoves[Random.Range(0, validMoves.Count)];
                GridTile targetTile = gridManager.GetTileAt(chosenMove.x, chosenMove.y);
                if (targetTile != null)
                {
                    Vector3 targetPos = targetTile.transform.position;
                    targetPos.z = 0;
                    transform.position = targetPos;
                    currentRow = chosenMove.x;
                    currentCol = chosenMove.y;
                }
            }
            // If no valid moves exist, the enemy will remain in place until the next cycle.
        }
    }
}
