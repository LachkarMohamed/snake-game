using UnityEngine;

public class MovementButtons : MonoBehaviour
{
    [SerializeField] private Snake snake;

    public void MoveUp()
    {
        snake.SetDirection(Snake.Direction.Up);
    }

    public void MoveDown()
    {
        snake.SetDirection(Snake.Direction.Down);
    }

    public void MoveLeft()
    {
        snake.SetDirection(Snake.Direction.Left);
    }

    public void MoveRight()
    {
        snake.SetDirection(Snake.Direction.Right);
    }
}