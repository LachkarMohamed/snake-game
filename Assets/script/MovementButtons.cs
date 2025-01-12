using UnityEngine;

public class MovementButtons : MonoBehaviour
{
    [SerializeField] private Snake snake;

    public void MoveUp()
    {
        snake.SetDirectionUp();
    }

    public void MoveDown()
    {
        snake.SetDirectionDown();
    }

    public void MoveLeft()
    {
        snake.SetDirectionLeft();
    }

    public void MoveRight()
    {
        snake.SetDirectionRight();
    }
}