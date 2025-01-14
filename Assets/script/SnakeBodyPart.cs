using UnityEngine;

public class SnakeBodyPart
{
    private SnakeMovePosition snakeMovePosition;
    public Transform transform;
    public SpriteRenderer spriteRenderer;

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
        float angle = snakeMovePosition.GetDirection() switch
        {
            Snake.Direction.Right => snakeMovePosition.GetPreviousDirection() switch
            {
                Snake.Direction.Down => 45,
                Snake.Direction.Up => 135,
                _ => 90
            },
            Snake.Direction.Left => snakeMovePosition.GetPreviousDirection() switch
            {
                Snake.Direction.Down => -45,
                Snake.Direction.Up => 225,
                _ => -90
            },
            Snake.Direction.Up => snakeMovePosition.GetPreviousDirection() switch
            {
                Snake.Direction.Left => 225,
                Snake.Direction.Right => 135,
                _ => 180
            },
            Snake.Direction.Down => snakeMovePosition.GetPreviousDirection() switch
            {
                Snake.Direction.Left => -45,
                Snake.Direction.Right => 45,
                _ => 0
            },
            _ => 0
        };

        // Flip the angle by 180 degrees
        angle += 180;

        // Normalize the angle to be within 0-360 degrees
        if (angle >= 360) angle -= 360;
        if (angle < 0) angle += 360;

        return angle;
    }

    public Vector2Int GetGridPosition() => snakeMovePosition.GetGridPosition();

    public void SetSprite(Sprite sprite) => spriteRenderer.sprite = sprite;
}