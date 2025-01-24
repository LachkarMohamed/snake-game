using System;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Snake : MonoBehaviour
{
    public enum Direction { Left, Right, Up, Down }
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
            SetDirectionUp();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && lastDirection != Direction.Up)
        {
            SetDirectionDown();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && lastDirection != Direction.Left)
        {
            SetDirectionRight();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && lastDirection != Direction.Right)
        {
            SetDirectionLeft();
        }
    }

    private void HandleGridMovement()
    {
        if (!ShouldMove()) return;

        lastDirection = gridMoveDirection;
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
        int scoreToAdd = gridMoveTimerMax switch
        {
            0.3f => 5,    // Slow speed
            0.15f => 10,  // Normal speed
            0.075f => 20, // Fast speed
            _ => 10       // Default score if speed is not matched
        };

        GameHandler.AddScore(scoreToAdd);

        await Task.Delay(1000);
        snakeBodySize++;
        CreateSnakeBody();
        UpdateSnakeBodyScaling(); // Update scaling for all body parts
    }

    private void UpdateTransform()
    {
        Vector2Int gridMoveDirectionVector = GetDirectionVector(gridMoveDirection);
        transform.position = new Vector3(gridPosition.x, gridPosition.y);
        transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector) - 90);
    }

    private void CheckSelfCollision()
    {
        foreach (SnakeBodyPart snakeBodyPart in snakeBodyPartList)
        {
            if (gridPosition == snakeBodyPart.GetGridPosition())
            {
                state = State.Dead;
                pauseMenu.ShowPauseMenu(true);
                return;
            }
        }

        if (levelGrid.IsObstacleAtPosition(gridPosition))
        {
            state = State.Dead;
            pauseMenu.ShowPauseMenu(true);
        }
    }

    private void CreateSnakeBody()
    {
        int totalBodyParts = snakeBodyPartList.Count + 1; // +1 for the new body part
        Vector2Int tailPosition = snakeMovePositionList[snakeMovePositionList.Count - 1].GetGridPosition();
        SnakeBodyPart newBodyPart = new SnakeBodyPart(snakeBodyPartList.Count, totalBodyParts, bodySprite);
        newBodyPart.SetSnakeMovePosition(new SnakeMovePosition(null, tailPosition, gridMoveDirection));
        snakeBodyPartList.Add(newBodyPart);

        // Update the sprites of the existing body parts
        if (snakeBodyPartList.Count > 1)
            snakeBodyPartList[snakeBodyPartList.Count - 2].SetSprite(bodySprite);
        snakeBodyPartList[snakeBodyPartList.Count - 1].SetSprite(tailSprite);

        // Update scaling for all body parts
        UpdateSnakeBodyScaling();
    }

    private float CalculateTaperedScale(int bodyIndex, int totalBodyParts)
    {
        // Special case: If there are exactly 2 body parts
        if (totalBodyParts == 2)
        {
            return bodyIndex == 0 ? 0.92f : 0.8f; // First body part: 92%, Second body part: 80%
        }

        // Default case: Use quadratic tapering for 3 or more body parts
        if (totalBodyParts < 2)
            return 0.9f; // Default scale for a single body part

        // Quadratic function for smooth tapering
        float t = (float)bodyIndex / (totalBodyParts - 1); // Normalized position (0 to 1)
        float scale = 1f - 0.2f * t * t; // Quadratic tapering (starts at 1, ends at 0.8)

        return scale;
    }

    private void UpdateSnakeBodyScaling()
    {
        int totalBodyParts = snakeBodyPartList.Count;

        // Ensure there are at least 2 body parts (head + 1 body part)
        if (totalBodyParts < 1)
        {
            // No scaling needed if there's only the head or no body parts
            return;
        }

        for (int i = 0; i < snakeBodyPartList.Count; i++)
        {
            // Calculate the scale using the tapering function
            float scale = CalculateTaperedScale(i, totalBodyParts);

            // Assign the scale to the body part
            snakeBodyPartList[i].transform.localScale = new Vector3(scale, scale, 1f);
        }
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
}