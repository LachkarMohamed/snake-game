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
    private List<GameObject> obstacles;
    private int width;
    private int height;

    public LevelGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
        obstacles = new List<GameObject>();
    }

    public void Setup(Snake snake)
    {
        this.snake = snake;
        SpawnFood();
    }

    public void AddObstacle(Vector2Int position, GameObject obstaclePrefab)
    {
        GameObject obstacle = GameObject.Instantiate(obstaclePrefab, new Vector3(position.x, position.y), Quaternion.identity);
        obstacles.Add(obstacle);
    }

    private void SpawnFood()
    {
        do
        {
            foodGridPosition = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
        } while (snake.GetFullSnakeGridPositionList().IndexOf(foodGridPosition) != -1);

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
}
