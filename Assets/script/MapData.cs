using System.Collections.Generic;
using UnityEngine;

public class MapData
{
    public List<Vector2Int> ObstaclePositions { get; private set; }
    public List<GameObject> ObstaclePrefabs { get; private set; }
    public Color? BackgroundColor { get; private set; }

    public MapData(List<Vector2Int> obstaclePositions, List<GameObject> obstaclePrefabs, Color? backgroundColor = null)
    {
        ObstaclePositions = obstaclePositions;
        ObstaclePrefabs = obstaclePrefabs;
        BackgroundColor = backgroundColor;
    }
}
