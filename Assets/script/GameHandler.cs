using TMPro;
using UnityEngine;
using System.Collections;
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

    private Dictionary<string, GameHandlerMapData> mapDataDictionary;

    private void Awake()
    {
        instance = this;
        InitializeStatic();
        StartCoroutine(InitializeMapData());
    }

    private IEnumerator Start()
    {
        levelGrid = new LevelGrid(16, 16);
        levelGrid.Setup(snake, GameAssets.i.obstaclePrefab1);
        snake.Setup(levelGrid);

        InitializeGameSettings();

        yield return StartCoroutine(InitializeMapData());

        LoadSelectedMap(selectedMap);
    }

    private void InitializeGameSettings()
    {
        if (GameAssets.i == null)
        {
            Debug.LogError("GameAssets is not initialized.");
            return;
        }

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

    private IEnumerator InitializeMapData()
    {
        if (GameAssets.i == null)
        {
            Debug.LogError("GameAssets is not initialized.");
            yield break;
        }

        if (GameAssets.i.obstaclePrefab1 == null)
        {
            Debug.LogError("Obstacle prefab is not initialized.");
            yield break;
        }

        if (GameAssets.i.mapBackgroundColors == null)
        {
            Debug.LogError("Map background colors are not initialized.");
            yield break;
        }

        mapDataDictionary = new Dictionary<string, GameHandlerMapData>();

        // No obstacles for map1
        mapDataDictionary["map1"] = new GameHandlerMapData(
            new List<Vector2Int>(), // No obstacles
            GameAssets.i.obstaclePrefab1,
            GameAssets.i.mapBackgroundColors.ContainsKey("map1") ? GameAssets.i.mapBackgroundColors["map1"] : (Color?)null
        );

        // Obstacles for map2
        List<Vector2Int> obstaclePositionsMap2 = new List<Vector2Int>
        {
            new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0),
            new Vector2Int(4, 0), new Vector2Int(5, 0), new Vector2Int(6, 0), new Vector2Int(7, 0),
            new Vector2Int(8, 0), new Vector2Int(9, 0), new Vector2Int(10, 0), new Vector2Int(11, 0),
            new Vector2Int(12, 0), new Vector2Int(13, 0), new Vector2Int(14, 0), new Vector2Int(15, 0),
            new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(0, 3), new Vector2Int(0, 4),
            new Vector2Int(0, 5), new Vector2Int(0, 6), new Vector2Int(0, 7), new Vector2Int(0, 8),
            new Vector2Int(0, 9), new Vector2Int(0, 10), new Vector2Int(0, 11), new Vector2Int(0, 12),
            new Vector2Int(0, 13), new Vector2Int(0, 14), new Vector2Int(0, 15), new Vector2Int(15, 1),
            new Vector2Int(15, 2), new Vector2Int(15, 3), new Vector2Int(15, 4), new Vector2Int(15, 5),
            new Vector2Int(15, 6), new Vector2Int(15, 7), new Vector2Int(15, 8), new Vector2Int(15, 9),
            new Vector2Int(15, 10), new Vector2Int(15, 11), new Vector2Int(15, 12), new Vector2Int(15, 13),
            new Vector2Int(15, 14), new Vector2Int(15, 15), new Vector2Int(1, 15), new Vector2Int(2, 15),
            new Vector2Int(3, 15), new Vector2Int(4, 15), new Vector2Int(5, 15), new Vector2Int(6, 15),
            new Vector2Int(7, 15), new Vector2Int(8, 15), new Vector2Int(9, 15), new Vector2Int(10, 15),
            new Vector2Int(11, 15), new Vector2Int(12, 15), new Vector2Int(13, 15), new Vector2Int(14, 15)
        };

        mapDataDictionary["map2"] = new GameHandlerMapData(
            obstaclePositionsMap2,
            GameAssets.i.obstaclePrefab1,
            GameAssets.i.mapBackgroundColors.ContainsKey("map2") ? GameAssets.i.mapBackgroundColors["map2"] : (Color?)null
        );

        // Obstacles for map3
        List<Vector2Int> obstaclePositionsMap3 = new List<Vector2Int>
        {
            new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0),
            new Vector2Int(4, 0), new Vector2Int(5, 0), new Vector2Int(6, 0), new Vector2Int(9, 0), new Vector2Int(10, 0), new Vector2Int(11, 0),
            new Vector2Int(12, 0), new Vector2Int(13, 0), new Vector2Int(14, 0), new Vector2Int(15, 0),
            new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(0, 3), new Vector2Int(0, 4),
            new Vector2Int(0, 5), new Vector2Int(0, 6),
            new Vector2Int(0, 9), new Vector2Int(0, 10), new Vector2Int(0, 11), new Vector2Int(0, 12),
            new Vector2Int(0, 13), new Vector2Int(0, 14), new Vector2Int(0, 15), new Vector2Int(15, 1),
            new Vector2Int(15, 2), new Vector2Int(15, 3), new Vector2Int(15, 4), new Vector2Int(15, 5),
            new Vector2Int(15, 6), new Vector2Int(15, 9),
            new Vector2Int(15, 10), new Vector2Int(15, 11), new Vector2Int(15, 12), new Vector2Int(15, 13),
            new Vector2Int(15, 14), new Vector2Int(15, 15), new Vector2Int(1, 15), new Vector2Int(2, 15),
            new Vector2Int(3, 15), new Vector2Int(4, 15), new Vector2Int(5, 15), new Vector2Int(6, 15),
            new Vector2Int(9, 15), new Vector2Int(10, 15),
            new Vector2Int(11, 15), new Vector2Int(12, 15), new Vector2Int(13, 15), new Vector2Int(14, 15),
            new Vector2Int(11, 4), new Vector2Int(11, 5), new Vector2Int(10, 4), new Vector2Int(10, 5),
            new Vector2Int(4, 11), new Vector2Int(5, 11), new Vector2Int(4, 10), new Vector2Int(5, 10),
        };

        mapDataDictionary["map3"] = new GameHandlerMapData(
            obstaclePositionsMap3,
            GameAssets.i.obstaclePrefab1,
            GameAssets.i.mapBackgroundColors.ContainsKey("map3") ? GameAssets.i.mapBackgroundColors["map3"] : (Color?)null
        );

        // Obstacles for map4
        List<Vector2Int> obstaclePositionsMap4 = new List<Vector2Int>
        {
            new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0),
            new Vector2Int(4, 0), new Vector2Int(5, 0), new Vector2Int(6, 0), new Vector2Int(7, 0),
            new Vector2Int(8, 0), new Vector2Int(9, 0), new Vector2Int(10, 0), new Vector2Int(11, 0),
            new Vector2Int(12, 0), new Vector2Int(13, 0), new Vector2Int(14, 0), new Vector2Int(15, 0),
            new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(0, 3), new Vector2Int(0, 4),
            new Vector2Int(0, 5), new Vector2Int(0, 6), new Vector2Int(0, 7), new Vector2Int(0, 8),
            new Vector2Int(0, 9), new Vector2Int(0, 10), new Vector2Int(0, 11), new Vector2Int(0, 12),
            new Vector2Int(0, 13), new Vector2Int(0, 14), new Vector2Int(0, 15), new Vector2Int(15, 1),
            new Vector2Int(15, 2), new Vector2Int(15, 3), new Vector2Int(15, 4), new Vector2Int(15, 5),
            new Vector2Int(15, 6), new Vector2Int(15, 7), new Vector2Int(15, 8), new Vector2Int(15, 9),
            new Vector2Int(15, 10), new Vector2Int(15, 11), new Vector2Int(15, 12), new Vector2Int(15, 13),
            new Vector2Int(15, 14), new Vector2Int(15, 15), new Vector2Int(1, 15), new Vector2Int(2, 15),
            new Vector2Int(3, 15), new Vector2Int(4, 15), new Vector2Int(5, 15), new Vector2Int(6, 15),
            new Vector2Int(7, 15), new Vector2Int(8, 15), new Vector2Int(9, 15), new Vector2Int(10, 15),
            new Vector2Int(11, 15), new Vector2Int(12, 15), new Vector2Int(13, 15), new Vector2Int(14, 15),
            new Vector2Int(1, 7), new Vector2Int(2, 7), new Vector2Int(3, 7), new Vector2Int(4, 7),
            new Vector2Int(5, 7), new Vector2Int(6, 7), new Vector2Int(8, 7),
            new Vector2Int(9, 7), new Vector2Int(10, 7), new Vector2Int(11, 7), new Vector2Int(12, 7),
            new Vector2Int(13, 7), new Vector2Int(14, 7), new Vector2Int(15, 7),
            new Vector2Int(1, 8), new Vector2Int(2, 8), new Vector2Int(3, 8), new Vector2Int(4, 8),
            new Vector2Int(5, 8), new Vector2Int(6, 8), new Vector2Int(8, 8),
            new Vector2Int(9, 8), new Vector2Int(10, 8), new Vector2Int(11, 8), new Vector2Int(12, 8),
            new Vector2Int(13, 8), new Vector2Int(14, 8), new Vector2Int(15, 8)
        };

        mapDataDictionary["map4"] = new GameHandlerMapData(
            obstaclePositionsMap4,
            GameAssets.i.obstaclePrefab1,
            GameAssets.i.mapBackgroundColors.ContainsKey("map4") ? GameAssets.i.mapBackgroundColors["map4"] : (Color?)null
        );

        // Obstacles for map5
        List<Vector2Int> obstaclePositionsMap5 = new List<Vector2Int>
        {
            new Vector2Int(0, 0), new Vector2Int(1, 0), new Vector2Int(2, 0), new Vector2Int(3, 0),
            new Vector2Int(12, 0), new Vector2Int(13, 0), new Vector2Int(14, 0), new Vector2Int(15, 0),
            new Vector2Int(0, 1), new Vector2Int(0, 2), new Vector2Int(0, 3),  new Vector2Int(0, 12),
            new Vector2Int(0, 13), new Vector2Int(0, 14), new Vector2Int(0, 15), new Vector2Int(15, 1),
            new Vector2Int(15, 2), new Vector2Int(15, 3), new Vector2Int(15, 12), new Vector2Int(15, 13),
            new Vector2Int(15, 14), new Vector2Int(15, 15), new Vector2Int(1, 15), new Vector2Int(2, 15),
            new Vector2Int(3, 15), new Vector2Int(12, 15), new Vector2Int(13, 15), new Vector2Int(14, 15),
            new Vector2Int(4, 6), new Vector2Int(5, 6), new Vector2Int(6, 6), new Vector2Int(6, 5), new Vector2Int(6, 4),
            new Vector2Int(9, 9), new Vector2Int(9, 10), new Vector2Int(9, 11), new Vector2Int(10, 9), new Vector2Int(11, 9),
            new Vector2Int(4, 9), new Vector2Int(5, 9), new Vector2Int(6, 9), new Vector2Int(6, 10), new Vector2Int(6, 11),
            new Vector2Int(10, 9), new Vector2Int(11, 9), new Vector2Int(9, 4), new Vector2Int(9, 5), new Vector2Int(9, 6),
            new Vector2Int(10, 6),new Vector2Int(11, 6),
            new Vector2Int(2, 2), new Vector2Int(3, 4), new Vector2Int(3, 9), new Vector2Int(1, 9),
            new Vector2Int(10, 1), new Vector2Int(10, 7), new Vector2Int(7, 11), new Vector2Int(14, 12),
        };

        mapDataDictionary["map5"] = new GameHandlerMapData(
            obstaclePositionsMap5,
            GameAssets.i.obstaclePrefab1,
            GameAssets.i.mapBackgroundColors.ContainsKey("map5") ? GameAssets.i.mapBackgroundColors["map5"] : (Color?)null
        );

        // Obstacles for map6
        List<Vector2Int> obstaclePositionsMap6 = new List<Vector2Int>
        {
            new Vector2Int(0, 2), new Vector2Int(1, 2), new Vector2Int(2, 2), new Vector2Int(3, 2), new Vector2Int(4, 2), new Vector2Int(5, 2), new Vector2Int(6, 2),
            new Vector2Int(4, 6), new Vector2Int(4, 7), new Vector2Int(4, 8), new Vector2Int(4, 9), new Vector2Int(4, 10), new Vector2Int(4, 11), new Vector2Int(4, 12), new Vector2Int(4, 13), new Vector2Int(4, 14), new Vector2Int(4, 15),
            new Vector2Int(9, 13), new Vector2Int(10, 13), new Vector2Int(11, 13), new Vector2Int(12, 13), new Vector2Int(13, 13), new Vector2Int(14, 13), new Vector2Int(15, 13),
            new Vector2Int(7, 5), new Vector2Int(7, 6), new Vector2Int(7, 7), new Vector2Int(7, 8), new Vector2Int(7, 7), new Vector2Int(8, 10), new Vector2Int(8, 8), new Vector2Int(8, 9),
            new Vector2Int(11, 0), new Vector2Int(11, 1), new Vector2Int(11, 2), new Vector2Int(11, 3), new Vector2Int(11, 4), new Vector2Int(11, 5), new Vector2Int(11, 6), new Vector2Int(11, 7), new Vector2Int(11, 8), new Vector2Int(11, 9),
        };   

        mapDataDictionary["map6"] = new GameHandlerMapData(
            obstaclePositionsMap6,
            GameAssets.i.obstaclePrefab1,
            GameAssets.i.mapBackgroundColors.ContainsKey("map6") ? GameAssets.i.mapBackgroundColors["map6"] : (Color?)null
        );

        // Obstacles for map7
        List<Vector2Int> obstaclePositionsMap7 = new List<Vector2Int>
        {
            new Vector2Int(0, 6), new Vector2Int(1, 6), new Vector2Int(2, 6), new Vector2Int(3, 6), new Vector2Int(4, 6), new Vector2Int(5, 6), new Vector2Int(6, 6), new Vector2Int(7, 6), new Vector2Int(8, 6), new Vector2Int(9, 6), new Vector2Int(10, 6), new Vector2Int(11, 6), new Vector2Int(12, 6), new Vector2Int(13, 6), new Vector2Int(14, 6), new Vector2Int(15, 6),
            new Vector2Int(9, 5), new Vector2Int(9, 4), new Vector2Int(9, 3), new Vector2Int(9, 2), new Vector2Int(9, 1), new Vector2Int(9, 0),
            new Vector2Int(0, 9), new Vector2Int(1, 9), new Vector2Int(2, 9), new Vector2Int(3, 9), new Vector2Int(4, 9), new Vector2Int(5, 9), new Vector2Int(6, 9), new Vector2Int(9, 9), new Vector2Int(10, 9), new Vector2Int(11, 9), new Vector2Int(12, 9), new Vector2Int(13, 9), new Vector2Int(14, 9), new Vector2Int(15, 9),
            new Vector2Int(0, 12), new Vector2Int(0, 13), new Vector2Int(0, 14), new Vector2Int(0, 15), new Vector2Int(1, 15),
            new Vector2Int(4, 15), new Vector2Int(5, 15), new Vector2Int(6, 15), new Vector2Int(7, 15), new Vector2Int(8, 15), new Vector2Int(9, 15), new Vector2Int(10, 15), new Vector2Int(11, 15), new Vector2Int(12, 15),
            new Vector2Int(6, 10), new Vector2Int(6, 11), new Vector2Int(6, 12), new Vector2Int(6, 13), new Vector2Int(6, 14),
        }; 

        mapDataDictionary["map7"] = new GameHandlerMapData(
            obstaclePositionsMap7,
            GameAssets.i.obstaclePrefab1,
            GameAssets.i.mapBackgroundColors.ContainsKey("map7") ? GameAssets.i.mapBackgroundColors["map7"] : (Color?)null
        );

        yield return null;
    }

    private void LoadSelectedMap(string selectedMap)
    {
        if (mapDataDictionary == null)
        {
            Debug.LogError("Map data dictionary is not initialized.");
            return;
        }

        if (string.IsNullOrEmpty(selectedMap))
        {
            Debug.LogError("Selected map is null or empty.");
            return;
        }

        if (mapDataDictionary.TryGetValue(selectedMap, out GameHandlerMapData mapData))
        {
            foreach (var position in mapData.ObstaclePositions)
            {
                levelGrid.AddObstacle(position);
            }

            if (mapData.BackgroundColor.HasValue)
            {
                mainCamera.backgroundColor = mapData.BackgroundColor.Value;
            }

            levelGrid.SpawnFood();
        }
        else
        {
            Debug.LogError($"Selected map '{selectedMap}' not found in map data dictionary.");
        }
    }

    public static int GetScore()
    {
        return score;
    }

    public void ReloadScene()
    {
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

public class GameHandlerMapData
{
    public List<Vector2Int> ObstaclePositions { get; }
    public GameObject ObstaclePrefab { get; }
    public Color? BackgroundColor { get; }

    public GameHandlerMapData(List<Vector2Int> obstaclePositions, GameObject obstaclePrefab, Color? backgroundColor = null)
    {
        ObstaclePositions = obstaclePositions;
        ObstaclePrefab = obstaclePrefab;
        BackgroundColor = backgroundColor;
    }
}