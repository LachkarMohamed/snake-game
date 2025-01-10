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

    // Returns the world position of the current grid position
    public Vector3 GetCurrentWorldPosition()
    {
        return new Vector3(gridPosition.x, gridPosition.y);
    }

    // Returns the world position of the previous move position, or defaults to the current position
    public Vector3 GetPreviousWorldPosition()
    {
        if (previousSnakeMovePosition != null)
        {
            Vector2Int prevGridPosition = previousSnakeMovePosition.GetGridPosition();
            return new Vector3(prevGridPosition.x, prevGridPosition.y);
        }
        return GetCurrentWorldPosition();
    }
}