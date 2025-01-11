
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.UI;

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
    private Sprite headSprite, bodySprite, tailSprite, foodSprite;

    private float currentRotationAngle = 0f;

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
        if (state == State.Dead || levelGrid == null) return;

        HandleInput();
        if (!isPaused) HandleGridMovement();
    }

    private void InitializeSnake()
    {
        gridPosition = new Vector2Int(10, 10);
        gridMoveTimerMax = 0.15f;
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
        if (Input.GetKeyDown(KeyCode.DownArrow)) SetDirection(Direction.Down);
        if (Input.GetKeyDown(KeyCode.RightArrow)) SetDirection(Direction.Right);
        if (Input.GetKeyDown(KeyCode.LeftArrow)) SetDirection(Direction.Left);
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
        if (snakeMovePositionList.Count > snakeBodySize)
            snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);

        var previousMovePosition = snakeMovePositionList.Count > 0 ? snakeMovePositionList[0] : null;
        var newMovePosition = new SnakeMovePosition(previousMovePosition, gridPosition, gridMoveDirection);

        snakeMovePositionList.Insert(0, newMovePosition);

        Vector2Int oldGridPosition = gridPosition;
        gridPosition += GetDirectionVector(gridMoveDirection);
        gridPosition = levelGrid.ValidateGridPosition(gridPosition);

        // Check for teleportation
        if (gridPosition != oldGridPosition && Vector3.Distance(new Vector3(gridPosition.x, gridPosition.y), new Vector3(oldGridPosition.x, oldGridPosition.y)) > Mathf.Max(levelGrid.width, levelGrid.height))
        {
            StartCoroutine(TeleportWithInvisibility());
        }
    }

    private IEnumerator TeleportWithInvisibility()
    {
        // Make the snake invisible
        SetSnakeVisibility(false);

        // Wait for 1 millisecond
        yield return new WaitForSeconds(0.001f);

        // Make the snake visible again
        SetSnakeVisibility(true);
    }

    private static Vector2Int GetDirectionVector(Direction direction)
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
        float targetAngle = GetAngleFromVector(GetDirectionVector(gridMoveDirection));
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle - 90);

        while (elapsedTime < gridMoveTimerMax)
        {
            float lerpFactor = elapsedTime / gridMoveTimerMax;
            transform.position = Vector3.Lerp(startPosition, endPosition, lerpFactor);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, lerpFactor);
            UpdateSnakeBodyPartsSmooth(lerpFactor);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = endPosition;
        transform.rotation = targetRotation;
        currentRotationAngle = targetAngle;
    }

    private void SetSnakeVisibility(bool isVisible)
    {
        // Set head visibility
        GetComponent<SpriteRenderer>().enabled = isVisible;

        // Set body parts visibility
        foreach (var bodyPart in snakeBodyPartList)
        {
            bodyPart.GetSpriteRenderer().enabled = isVisible;
        }
    }


    private void UpdateSnakeBodyPartsSmooth(float lerpFactor)
    {
        for (int i = 0; i < snakeBodyPartList.Count; i++)
        {
            var movePosition = snakeMovePositionList[Mathf.Clamp(i, 0, snakeMovePositionList.Count - 1)];
            Vector3 startPosition = movePosition.GetPreviousWorldPosition();
            Vector3 endPosition = movePosition.GetCurrentWorldPosition();

            snakeBodyPartList[i].transform.position = Vector3.Lerp(startPosition, endPosition, lerpFactor);
            float startAngle = GetAngleFromVector(GetDirectionVector(movePosition.GetPreviousDirection())) - 90;
            float endAngle = GetAngleFromVector(GetDirectionVector(movePosition.GetDirection())) - 90;
            float smoothAngle = Mathf.LerpAngle(startAngle, endAngle, lerpFactor);
            snakeBodyPartList[i].transform.eulerAngles = new Vector3(0, 0, smoothAngle);
        }
    }

    private void UpdateSnakeBodyParts()
    {
        for (int i = 0; i < snakeBodyPartList.Count; i++)
        {
            if (i < snakeMovePositionList.Count)
            {
                SnakeMovePosition movePosition = snakeMovePositionList[i];
                snakeBodyPartList[i].SetSnakeMovePosition(movePosition);
            }
        }

        if (snakeBodyPartList.Count > 0)
        {
            for (int i = 0; i < snakeBodyPartList.Count - 1; i++)
            {
                snakeBodyPartList[i].SetSprite(bodySprite);
                snakeBodyPartList[i].transform.position = snakeBodyPartList[i + 1].transform.position;
                snakeBodyPartList[i].transform.rotation = snakeBodyPartList[i + 1].transform.rotation;
            }
            snakeBodyPartList[snakeBodyPartList.Count - 1].SetSprite(tailSprite);
        }
    }


    public void SetPause(bool pause)
    {
        isPaused = pause;
    }

    public List<Vector2Int> GetFullSnakeGridPositionList()
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int> { gridPosition }; // Add head position
        foreach (var movePosition in snakeMovePositionList)
        {
            gridPositionList.Add(movePosition.GetGridPosition());
        }
        return gridPositionList;
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
        GameHandler.AddScore(gridMoveTimerMax switch
        {
            0.3f => 5,
            0.15f => 10,
            0.075f => 20,
            _ => 10
        });

        await Task.Delay(1000);
        snakeBodySize++;
        CreateSnakeBody();
    }

    private void CheckSelfCollision()
    {
        foreach (var bodyPart in snakeBodyPartList)
        {
            if (gridPosition == bodyPart.GetGridPosition())
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
        var tailPosition = snakeMovePositionList[snakeMovePositionList.Count - 1].GetGridPosition();
        var newBodyPart = new SnakeBodyPart(snakeBodyPartList.Count, bodySprite);
        newBodyPart.SetSnakeMovePosition(new SnakeMovePosition(null, tailPosition, gridMoveDirection));
        snakeBodyPartList.Add(newBodyPart);

        if (snakeBodyPartList.Count > 1)
            snakeBodyPartList[^2].SetSprite(bodySprite);
        snakeBodyPartList[^1].SetSprite(tailSprite);
    }

    private float GetAngleFromVector(Vector2Int dir) =>
        Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

    public void SetHeadSkin(string headSkin)
    {
        if (GameAssets.i.headSprites.TryGetValue(headSkin, out Sprite sprite))
        {
            headSprite = sprite;
            GetComponent<SpriteRenderer>().sprite = headSprite;
        }
    }

    public void SetBodySkin(string bodySkin)
    {
        if (GameAssets.i.bodySprites.TryGetValue(bodySkin, out Sprite sprite))
        {
            bodySprite = sprite;
            foreach (var bodyPart in snakeBodyPartList)
            {
                bodyPart.SetSprite(bodySprite);
            }
        }
    }

    public void SetTailSkin(string tailSkin)
    {
        if (GameAssets.i.tailSprites.TryGetValue(tailSkin, out Sprite sprite))
        {
            tailSprite = sprite;
            if (snakeBodyPartList.Count > 0)
            {
                snakeBodyPartList[snakeBodyPartList.Count - 1].SetSprite(tailSprite);
            }
        }
    }

    public void SetFoodSkin(string foodSkin)
    {
        if (GameAssets.i.foodSprites.TryGetValue(foodSkin, out Sprite sprite))
        {
            foodSprite = sprite;
        }
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


    private void ApplySkins()
    {
        if (GameAssets.i.headSprites.TryGetValue(GameHandler.selectedHead, out var sprite))
            GetComponent<SpriteRenderer>().sprite = sprite;

        foreach (var bodyPart in snakeBodyPartList)
            bodyPart.SetSprite(bodySprite);
    }
}
