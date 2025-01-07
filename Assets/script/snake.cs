using System;
using System.Collections;
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

    public void SetDirection(Direction direction)
    {
        if ((direction == Direction.Up && lastDirection != Direction.Down) ||
            (direction == Direction.Down && lastDirection != Direction.Up) ||
            (direction == Direction.Left && lastDirection != Direction.Right) ||
            (direction == Direction.Right && lastDirection != Direction.Left))
        {
            gridMoveDirection = direction;
            isPaused = false;
        }
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) SetDirection(Direction.Up);
        else if (Input.GetKeyDown(KeyCode.DownArrow)) SetDirection(Direction.Down);
        else if (Input.GetKeyDown(KeyCode.RightArrow)) SetDirection(Direction.Right);
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) SetDirection(Direction.Left);
    }

    private void HandleGridMovement()
    {
        if (!ShouldMove()) return;

        lastDirection = gridMoveDirection;
        UpdateSnakePosition();
        CheckFoodConsumption();
        StartCoroutine(SmoothMove());
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
        // Ensure only up-to-date positions remain
        if (snakeMovePositionList.Count > snakeBodySize)
            snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);

        // Create new move position based on current grid
        SnakeMovePosition previousMovePosition = snakeMovePositionList.Count > 0 ? snakeMovePositionList[0] : null;
        SnakeMovePosition newMovePosition = new SnakeMovePosition(previousMovePosition, gridPosition, gridMoveDirection);

        // Insert at the front of the list
        snakeMovePositionList.Insert(0, newMovePosition);

        // Update grid position
        Vector2Int directionVector = GetDirectionVector(gridMoveDirection);
        gridPosition += directionVector;
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

    private IEnumerator SmoothMove()
    {
        Vector3 startPosition = transform.position;
        Vector3 endPosition = new Vector3(gridPosition.x, gridPosition.y);
        float elapsedTime = 0f;

        while (elapsedTime < gridMoveTimerMax)
        {
            float lerpFactor = elapsedTime / gridMoveTimerMax;
            transform.position = Vector3.Lerp(startPosition, endPosition, lerpFactor);
            UpdateSnakeBodyPartsSmooth(lerpFactor);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
        UpdateSnakeBodyPartsSmooth(1f);
        Vector2Int gridMoveDirectionVector = GetDirectionVector(gridMoveDirection);
        transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector) - 90);
    }

    private void UpdateSnakeBodyPartsSmooth(float lerpFactor)
    {
        for (int i = 0; i < snakeBodyPartList.Count; i++)
        {
            // Get the current and next move positions for interpolation
            SnakeMovePosition currentMovePosition = snakeMovePositionList[Mathf.Clamp(i, 0, snakeMovePositionList.Count - 1)];
            Vector3 startPosition = currentMovePosition.GetPreviousWorldPosition();
            Vector3 endPosition = currentMovePosition.GetCurrentWorldPosition();

            // Smoothly interpolate position
            snakeBodyPartList[i].transform.position = Vector3.Lerp(startPosition, endPosition, lerpFactor);

            // Smoothly interpolate rotation between directions
            SnakeMovePosition nextMovePosition = i < snakeMovePositionList.Count - 1 ? snakeMovePositionList[i + 1] : currentMovePosition;
            Vector2Int currentDirectionVector = GetDirectionVector(currentMovePosition.GetDirection());
            Vector2Int nextDirectionVector = GetDirectionVector(nextMovePosition.GetDirection());

            float startAngle = GetAngleFromVector(currentDirectionVector) - 90;
            float endAngle = GetAngleFromVector(nextDirectionVector) - 90;

            float smoothAngle = Mathf.LerpAngle(startAngle, endAngle, lerpFactor);
            snakeBodyPartList[i].transform.eulerAngles = new Vector3(0, 0, smoothAngle);
        }
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
            0.3f => 5,
            0.15f => 10,
            0.075f => 20,
            _ => 10
        };

        GameHandler.AddScore(scoreToAdd);

        await Task.Delay(1000);
        snakeBodySize++;
        CreateSnakeBody();
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
}