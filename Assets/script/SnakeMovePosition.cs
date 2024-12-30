using UnityEngine;

public class SnakeMovePosition
{
    private SnakeMovePosition previousSnakeMovePosition;
    private Vector2Int gridPosition;
    private Snake.Direction direction;

    public SnakeMovePosition(SnakeMovePosition previousSnakeMovePosition, Vector2Int gridPosition, Snake.Direction direction)
    {
        this.previousSnakeMovePosition = previousSnakeMovePosition;
        this.gridPosition = gridPosition;
        this.direction = direction;
    }

    public Vector2Int GetGridPosition() => gridPosition;

    public Snake.Direction GetDirection() => direction;

    public Snake.Direction GetPreviousDirection() => previousSnakeMovePosition?.direction ?? Snake.Direction.Right;
}
