using System.Collections.Generic;
using UnityEngine;

public class LevelGrid
{
    private int width;
    private int height;
    private HashSet<Vector2Int> occupiedPositions;
    private GameObject foodPrefab;
    private Vector2Int foodPosition;

    public LevelGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
        occupiedPositions = new HashSet<Vector2Int>();
    }

    public void Setup(Snake snake, GameObject foodPrefab)
    {
        this.foodPrefab = foodPrefab;
        snake.OnSnakeMove += HandleSnakeMove;
        snake.OnSnakeEat += HandleSnakeEat;
        SpawnFood();
    }

    public void AddObstacle(Vector2Int position)
    {
        occupiedPositions.Add(position);
    }

    public void RemoveOccupiedPosition(Vector2Int position)
    {
        occupiedPositions.Remove(position);
    }

    public bool IsPositionOccupied(Vector2Int position)
    {
        return occupiedPositions.Contains(position);
    }

    public void SpawnFood()
    {
        foodPosition = GetRandomUnoccupiedPosition();
        occupiedPositions.Add(foodPosition);
        Object.Instantiate(foodPrefab, new Vector3(foodPosition.x, foodPosition.y), Quaternion.identity);
    }

    private Vector2Int GetRandomUnoccupiedPosition()
    {
        List<Vector2Int> availablePositions = new List<Vector2Int>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2Int position = new Vector2Int(x, y);
                if (!IsPositionOccupied(position))
                {
                    availablePositions.Add(position);
                }
            }
        }

        if (availablePositions.Count == 0)
        {
            Debug.LogError("No available positions to spawn food!");
            return Vector2Int.zero;
        }

        return availablePositions[Random.Range(0, availablePositions.Count)];
    }

    private void HandleSnakeMove(Vector2Int position)
    {
        occupiedPositions.Add(position);
    }

    private void HandleSnakeEat(Vector2Int position)
    {
        occupiedPositions.Remove(foodPosition);
        SpawnFood();
    }

    public Vector2Int ValidateGridPosition(Vector2Int gridPosition)
    {
        if (gridPosition.x < 0) gridPosition.x = width - 1;
        if (gridPosition.x >= width) gridPosition.x = 0;
        if (gridPosition.y < 0) gridPosition.y = height - 1;
        if (gridPosition.y >= height) gridPosition.y = 0;
        return gridPosition;
    }

    public bool TrySnakeEatFood(Vector2Int snakePosition)
    {
        if (snakePosition == foodPosition)
        {
            SpawnFood();
            return true;
        }
        return false;
    }

    public bool IsObstacleAtPosition(Vector2Int position)
    {
        return occupiedPositions.Contains(position);
    }
}
