using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Snake : MonoBehaviour
{
    private enum Direction { Left, Right, Up, Down }
    private enum State { Alive, Dead }

    private State state;
    private Direction gridMoveDirection;
    private Direction lastDirection;
    private Vector2Int gridPosition;
    private float gridMoveTimer;
    private float gridMoveTimerMax;
    private LevelGrid levelGrid;
    private int snakeBodySize;
    private List<SnakeMovePosition> snakeMovePositionList;
    private List<SnakeBodyPart> snakeBodyPartList;

    private bool isPaused;

    private PauseMenu pauseMenu;

    private Sprite headSprite;
    private Sprite bodySprite;
    private Sprite tailSprite;
    private Sprite foodSprite;

    public void Setup(LevelGrid levelGrid)
    {
        this.levelGrid = levelGrid;
    }

    private void Awake()
    {
        InitializeSnake();
        pauseMenu = FindObjectOfType<PauseMenu>();
    }

    private void Start()
    {
        ApplySkins();
    }

    private void Update()
    {
        if (levelGrid == null || state == State.Dead) return;

        HandleInput();
        if (!isPaused)
        {
            HandleGridMovement();
        }
    }

    private void InitializeSnake()
    {
        gridPosition = new Vector2Int(10, 10);
        gridMoveTimerMax = .15f;
        gridMoveTimer = 0f;
        gridMoveDirection = Direction.Right;
        lastDirection = gridMoveDirection;
        isPaused = true;

        snakeMovePositionList = new List<SnakeMovePosition> { new SnakeMovePosition(null, gridPosition, gridMoveDirection) };
        snakeBodySize = 0;
        snakeBodyPartList = new List<SnakeBodyPart>();
        state = State.Alive;
    }

    public void SetDirectionUp()
    {
        if (lastDirection != Direction.Down)
        {
            gridMoveDirection = Direction.Up;
            isPaused = false;
        }
    }

    public void SetDirectionDown()
    {
        if (lastDirection != Direction.Up)
        {
            gridMoveDirection = Direction.Down;
            isPaused = false;
        }
    }

    public void SetDirectionLeft()
    {
        if (lastDirection != Direction.Right)
        {
            gridMoveDirection = Direction.Left;
            isPaused = false;
        }
    }

    public void SetDirectionRight()
    {
        if (lastDirection != Direction.Left)
        {
            gridMoveDirection = Direction.Right;
            isPaused = false;
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && lastDirection != Direction.Down)
        {
            gridMoveDirection = Direction.Up;
            isPaused = false;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && lastDirection != Direction.Up)
        {
            gridMoveDirection = Direction.Down;
            isPaused = false;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && lastDirection != Direction.Left)
        {
            gridMoveDirection = Direction.Right;
            isPaused = false;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && lastDirection != Direction.Right)
        {
            gridMoveDirection = Direction.Left;
            isPaused = false;
        }
    }

    private void HandleGridMovement()
    {
        if (!ShouldMove()) return;

        lastDirection = gridMoveDirection; // Update lastDirection only when the snake moves
        UpdateSnakePosition();
        CheckFoodConsumption();
        UpdateTransform();
        UpdateSnakeBodyParts();
        CheckSelfCollision();
    }

    private bool ShouldMove()
    {
        gridMoveTimer += Time.deltaTime;
        if (gridMoveTimer < gridMoveTimerMax) return false;

        gridMoveTimer -= gridMoveTimerMax;
        return true;
    }

    private void UpdateSnakePosition()
    {
        if (snakeMovePositionList.Count > snakeBodySize)
            snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);

        SnakeMovePosition previousSnakeMovePosition = snakeMovePositionList.Count > 0 ? snakeMovePositionList[0] : null;
        SnakeMovePosition snakeMovePosition = new SnakeMovePosition(previousSnakeMovePosition, gridPosition, gridMoveDirection);
        snakeMovePositionList.Insert(0, snakeMovePosition);

        Vector2Int gridMoveDirectionVector = GetDirectionVector(gridMoveDirection);
        gridPosition += gridMoveDirectionVector;
        gridPosition = levelGrid.ValidateGridPosition(gridPosition);
    }

    private Vector2Int GetDirectionVector(Direction direction)
    {
        return direction switch
        {
            Direction.Up => new Vector2Int(0, 1),
            Direction.Down => new Vector2Int(0, -1),
            Direction.Left => new Vector2Int(-1, 0),
            Direction.Right => new Vector2Int(1, 0),
            _ => new Vector2Int(0, 0)
        };
    }

    private async void CheckFoodConsumption()
    {
        if (levelGrid.TrySnakeEatFood(gridPosition))
        {
            await HandleFoodConsumption();
        }
    }

    private async Task HandleFoodConsumption()
    {

        await Task.Delay(1000);

        snakeBodySize++;
        CreateSnakeBody();
        GameHandler.AddScore(100);
    }

    private void UpdateTransform()
    {
        Vector2Int gridMoveDirectionVector = GetDirectionVector(gridMoveDirection);
        transform.position = new Vector3(gridPosition.x, gridPosition.y);
        transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector) - 90);
    }


    private void CheckSelfCollision()
    {
        // Check collision with snake body
        foreach (SnakeBodyPart snakeBodyPart in snakeBodyPartList)
        {
            if (gridPosition == snakeBodyPart.GetGridPosition())
            {
                state = State.Dead;
                pauseMenu.ShowPauseMenu(true);
                return;
            }
        }

        // Check collision with obstacles
        if (levelGrid.IsObstacleAtPosition(gridPosition))
        {
            state = State.Dead;
            pauseMenu.ShowPauseMenu(true);
        }
    }

    private bool CheckObstacleCollision()
    {
        // Get the list of obstacles from the level grid
        List<Vector2Int> obstaclePositions = levelGrid.GetObstaclePositions();

        // Check if the snake's current position matches any obstacle position
        foreach (Vector2Int obstaclePosition in obstaclePositions)
        {
            // Check if the snake's position is within the bounds of the obstacle
            if (IsPositionWithinObstacleBounds(gridPosition, obstaclePosition))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsPositionWithinObstacleBounds(Vector2Int snakePosition, Vector2Int obstaclePosition)
    {
        // Assuming each obstacle occupies a single grid cell
        // Adjust this method if obstacles can occupy multiple grid cells
        return snakePosition == obstaclePosition;
    }

    private void CreateSnakeBody()
    {
        Vector2Int tailPosition = snakeMovePositionList[snakeMovePositionList.Count - 1].GetGridPosition();
        SnakeBodyPart newBodyPart = new SnakeBodyPart(snakeBodyPartList.Count, bodySprite);
        newBodyPart.SetSnakeMovePosition(new SnakeMovePosition(null, tailPosition, gridMoveDirection));
        snakeBodyPartList.Add(newBodyPart);

        if (snakeBodyPartList.Count > 1)
            snakeBodyPartList[snakeBodyPartList.Count - 2].SetSprite(bodySprite);
        snakeBodyPartList[snakeBodyPartList.Count - 1].SetSprite(tailSprite);
    }

    private void UpdateSnakeBodyParts()
    {
        for (int i = 0; i < snakeBodyPartList.Count; i++)
        {
            if (i < snakeMovePositionList.Count)
                snakeBodyPartList[i].SetSnakeMovePosition(snakeMovePositionList[i]);
        }

        if (snakeBodyPartList.Count > 0)
        {
            for (int i = 0; i < snakeBodyPartList.Count - 1; i++)
                snakeBodyPartList[i].SetSprite(bodySprite);
            snakeBodyPartList[snakeBodyPartList.Count - 1].SetSprite(tailSprite);
        }
    }

    private float GetAngleFromVector(Vector2Int dir)
    {
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return n < 0 ? n + 360 : n;
    }

    public Vector2Int GetGridPosition() => gridPosition;

    public List<Vector2Int> GetFullSnakeGridPositionList()
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int> { gridPosition };
        gridPositionList.AddRange(snakeMovePositionList.ConvertAll(snakeMovePosition => snakeMovePosition.GetGridPosition()));
        return gridPositionList;
    }

    private void ApplySkins()
    {
        if (GameAssets.i.headSprites.TryGetValue(GameHandler.selectedHead, out Sprite headSprite))
            GetComponent<SpriteRenderer>().sprite = headSprite;

        foreach (var bodyPart in snakeBodyPartList)
            bodyPart.SetSprite(bodySprite);
    }

    public void SetHeadSkin(string headSkin)
    {
        if (GameAssets.i.headSprites.TryGetValue(headSkin, out Sprite sprite))
            headSprite = sprite;
    }

    public void SetBodySkin(string bodySkin)
    {
        if (GameAssets.i.bodySprites.TryGetValue(bodySkin, out Sprite sprite))
            bodySprite = sprite;
    }

    public void SetTailSkin(string tailSkin)
    {
        if (GameAssets.i.tailSprites.TryGetValue(tailSkin, out Sprite sprite))
            tailSprite = sprite;
    }

    public void SetFoodSkin(string foodSkin)
    {
        if (GameAssets.i.foodSprites.TryGetValue(foodSkin, out Sprite sprite))
            foodSprite = sprite;
    }

    public void SetSpeed(string speed)
    {
        gridMoveTimerMax = speed switch
        {
            "Slow" => 0.3f,
            "Normal" => 0.15f,
            "Fast" => 0.075f,
            _ => gridMoveTimerMax
        };
    }

    
    public void SetPause(bool pause)
    {
        isPaused = pause;
    }
    private class SnakeBodyPart
    {
        private SnakeMovePosition snakeMovePosition;
        private Transform transform;
        private SpriteRenderer spriteRenderer;

        public SnakeBodyPart(int bodyIndex, Sprite bodySprite)
        {
            GameObject snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));
            spriteRenderer = snakeBodyGameObject.GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = bodySprite;
            spriteRenderer.sortingOrder = -1 - bodyIndex;
            transform = snakeBodyGameObject.transform;
        }

        public void SetSnakeMovePosition(SnakeMovePosition snakeMovePosition)
        {
            this.snakeMovePosition = snakeMovePosition;
            transform.position = new Vector3(snakeMovePosition.GetGridPosition().x, snakeMovePosition.GetGridPosition().y);

            float angle = GetBodyPartAngle(snakeMovePosition);
            transform.eulerAngles = new Vector3(0, 0, angle);
        }

        private float GetBodyPartAngle(SnakeMovePosition snakeMovePosition)
        {
            return snakeMovePosition.GetDirection() switch
            {
                Direction.Right => snakeMovePosition.GetPreviousDirection() switch
                {
                    Direction.Down => 45,
                    Direction.Up => 135,
                    _ => 90
                },
                Direction.Left => snakeMovePosition.GetPreviousDirection() switch
                {
                    Direction.Down => -45,
                    Direction.Up => 225,
                    _ => -90
                },
                Direction.Up => snakeMovePosition.GetPreviousDirection() switch
                {
                    Direction.Left => 225,
                    Direction.Right => 135,
                    _ => 180
                },
                Direction.Down => snakeMovePosition.GetPreviousDirection() switch
                {
                    Direction.Left => -45,
                    Direction.Right => 45,
                    _ => 0
                },
                _ => 0
            };
        }

        public Vector2Int GetGridPosition() => snakeMovePosition.GetGridPosition();

        public void SetSprite(Sprite sprite) => spriteRenderer.sprite = sprite;
    }

    private class SnakeMovePosition
    {
        private SnakeMovePosition previousSnakeMovePosition;
        private Vector2Int gridPosition;
        private Direction direction;

        public SnakeMovePosition(SnakeMovePosition previousSnakeMovePosition, Vector2Int gridPosition, Direction direction)
        {
            this.previousSnakeMovePosition = previousSnakeMovePosition;
            this.gridPosition = gridPosition;
            this.direction = direction;
        }

        public Vector2Int GetGridPosition() => gridPosition;

        public Direction GetDirection() => direction;

        public Direction GetPreviousDirection() => previousSnakeMovePosition?.direction ?? Direction.Right;
    }
}
