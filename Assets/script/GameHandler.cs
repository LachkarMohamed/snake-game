using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    public static GameHandler instance;

    private static int score;

    [SerializeField] private Snake snake;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private PauseMenu pauseMenu;

    private LevelGrid levelGrid;

    public static string selectedMap;
    public static string selectedHead;
    public static string selectedBody;
    public static string selectedTail;
    public static string selectedFood;
    public static string selectedSpeed;

    private void Awake()
    {
        instance = this;
        InitializeStatic();
    }

    private void Start()
    {
        levelGrid = new LevelGrid(16, 16);
        levelGrid.Setup(snake);
        snake.Setup(levelGrid);

        InitializeGameSettings();
    }

    private void InitializeGameSettings()
    {
        if (string.IsNullOrEmpty(selectedHead) || !GameAssets.i.headSprites.ContainsKey(selectedHead))
        {
            Debug.LogError("Invalid or missing head skin selection.");
            return;
        }
        if (string.IsNullOrEmpty(selectedBody) || !GameAssets.i.bodySprites.ContainsKey(selectedBody))
        {
            Debug.LogError("Invalid or missing body skin selection.");
            return;
        }
        if (string.IsNullOrEmpty(selectedTail) || !GameAssets.i.tailSprites.ContainsKey(selectedTail))
        {
            Debug.LogError("Invalid or missing tail skin selection.");
            return;
        }
        if (string.IsNullOrEmpty(selectedFood) || !GameAssets.i.foodSprites.ContainsKey(selectedFood))
        {
            Debug.LogError("Invalid or missing food skin selection.");
            return;
        }
        if (string.IsNullOrEmpty(selectedSpeed))
        {
            Debug.LogError("Invalid or missing speed selection.");
            return;
        }

        snake.SetHeadSkin(selectedHead);
        snake.SetBodySkin(selectedBody);
        snake.SetTailSkin(selectedTail);
        snake.SetFoodSkin(selectedFood);
        snake.SetSpeed(selectedSpeed);
    }

    public static int GetScore()
    {
        return score;
    }

    public void ReloadScene()
    {
        // Reset time scale before reloading the scene
        Time.timeScale = 1f;
        Loader.Load(Loader.Scene.GameScene);
    }

    public static void AddScore(int amount)
    {
        score += amount;
        instance.UpdateScoreText();
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    private static void InitializeStatic()
    {
        score = 0;
    }
}
