using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey;
using CodeMonkey.Utils;

public class LevelGrid
{
    private Snake snake;
    private Vector2Int foodGridPosition;
    private GameObject foodGameObject;
    private List<Vector2Int> obstaclePositions;
    private int width;
    private int height;

    public LevelGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
        obstaclePositions = new List<Vector2Int>();
    }

    public void Setup(Snake snake)
    {
        this.snake = snake;
        SpawnFood();
    }

    public void AddObstacle(Vector2Int position, GameObject obstaclePrefab)
    {
        obstaclePositions.Add(position);
        Vector3 worldPosition = new Vector3(position.x, position.y);
        GameObject.Instantiate(obstaclePrefab, worldPosition, Quaternion.identity);
    }

    public List<Vector2Int> GetObstaclePositions()
    {
        return obstaclePositions;
    }

    private void SpawnFood()
    {
        do
        {
            foodGridPosition = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
        } while (snake.GetFullSnakeGridPositionList().IndexOf(foodGridPosition) != -1 || obstaclePositions.Contains(foodGridPosition));

        foodGameObject = new GameObject("Food", typeof(SpriteRenderer));

        if (GameAssets.i.foodSprites.TryGetValue(GameHandler.selectedFood, out Sprite foodSprite))
        {
            foodGameObject.GetComponent<SpriteRenderer>().sprite = foodSprite;
        }
        else
        {
            Debug.LogError($"Food skin '{GameHandler.selectedFood}' not found. Using default food sprite.");
            foodGameObject.GetComponent<SpriteRenderer>().sprite = GameAssets.i.defaultFoodSprite;
        }

        foodGameObject.transform.position = new Vector3(foodGridPosition.x, foodGridPosition.y);
    }

    public bool TrySnakeEatFood(Vector2Int snakeGridPosition)
    {
        if (snakeGridPosition == foodGridPosition)
        {
            Object.Destroy(foodGameObject);
            SpawnFood();
            return true;
        }
        return false;
    }

    public Vector2Int ValidateGridPosition(Vector2Int gridPosition)
    {
        if (gridPosition.x < 0)
        {
            gridPosition.x = width - 1;
        }
        if (gridPosition.x > width - 1)
        {
            gridPosition.x = 0;
        }

        if (gridPosition.y < 0)
        {
            gridPosition.y = height - 1;
        }
        if (gridPosition.y > height - 1)
        {
            gridPosition.y = 0;
        }
        return gridPosition;
    }

    public bool IsObstacleAtPosition(Vector2Int position)
    {
        return obstaclePositions.Contains(position);
    }
}
