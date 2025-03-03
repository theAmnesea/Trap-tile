using UnityEngine;

public class GridTile : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    public enum TileState { Unswept, Swept, Enemy }
    public TileState currentState = TileState.Swept;

    public Sprite playerTileSprite;
    public Sprite enemyTileSprite;

    public Color UnsweptColor = Color.clear;
    public Color SweptColor = Color.white;
    public Color EnemyColor = Color.black;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetState(TileState newState)
    {
        currentState = newState;
        UpdateTileColor();
    }

    private void UpdateTileColor()
    {
        if (_spriteRenderer != null)
        {
            switch (currentState)
            {
                case TileState.Unswept:
                    _spriteRenderer.sprite = playerTileSprite;
                    _spriteRenderer.color = UnsweptColor;
                    break;
                case TileState.Swept:
                    _spriteRenderer.sprite = playerTileSprite;
                    _spriteRenderer.color = SweptColor;
                    break;
                case TileState.Enemy:
                    _spriteRenderer.sprite = enemyTileSprite;
                    _spriteRenderer.color = EnemyColor;
                    break;
            }
        }
        else
        {
            Debug.LogError("SpriteRenderer not found on GridTile object!");
        }
    }
}
