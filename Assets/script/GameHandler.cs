using TMPro;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameHandler : MonoBehaviour
{
    public static GameHandler instance;

    private static int score;

    [SerializeField] private Snake snake;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private PauseMenu pauseMenu;
    [SerializeField] private Camera mainCamera;

    private LevelGrid levelGrid;

    public static string selectedMap;
    public static string selectedHead;
    public static string selectedBody;
    public static string selectedTail;
    public static string selectedFood;
    public static string selectedSpeed;

    private Dictionary<string, MapData> mapDataDictionary;

    private void Awake()
    {
        instance = this;
        InitializeStatic();
        InitializeMapData();
    }

    private void Start()
    {
        levelGrid = new LevelGrid(16, 16);
        levelGrid.Setup(snake);
        snake.Setup(levelGrid);

        InitializeGameSettings();
        LoadSelectedMap(selectedMap);
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

    private void InitializeMapData()
    {
        mapDataDictionary = new Dictionary<string, MapData>();

        // Example map data
        mapDataDictionary["map1"] = new MapData(
            new List<Vector2Int> { new Vector2Int(5, 5), new Vector2Int(10, 10) },
            new List<GameObject> { GameAssets.i.obstaclePrefab1, GameAssets.i.obstaclePrefab2 },
            GameAssets.i.mapBackgroundColors["map1"]
        );

        mapDataDictionary["map2"] = new MapData(
            new List<Vector2Int> { new Vector2Int(3, 3), new Vector2Int(7, 7) },
            new List<GameObject> { GameAssets.i.obstaclePrefab2, GameAssets.i.obstaclePrefab1 },
            GameAssets.i.mapBackgroundColors["map2"]
        );
    }

    private void LoadSelectedMap(string selectedMap)
    {
        if (mapDataDictionary.TryGetValue(selectedMap, out MapData mapData))
        {
            for (int i = 0; i < mapData.ObstaclePositions.Count; i++)
            {
                levelGrid.AddObstacle(mapData.ObstaclePositions[i], mapData.ObstaclePrefabs[i]);
            }

            // Apply map-specific effects
            if (mapData.BackgroundColor.HasValue)
            {
                mainCamera.backgroundColor = mapData.BackgroundColor.Value;
            }
        }
        else
        {
            Debug.LogError("Selected map not found!");
        }
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
