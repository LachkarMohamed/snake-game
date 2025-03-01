using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GameAssets.cs
public class GameAssets : MonoBehaviour
{
    public static GameAssets i;

    public Dictionary<string, Sprite> headSprites;
    public Dictionary<string, Sprite> bodySprites;
    public Dictionary<string, Sprite> tailSprites;
    public Dictionary<string, Sprite> foodSprites;
    public Dictionary<string, Sprite> mapSprites;
    public Dictionary<string, Color> mapBackgroundColors;

    public Sprite defaultFoodSprite;

    public Sprite headSprite1;
    public Sprite headSprite2;
    public Sprite headSprite3;
    public Sprite bodySprite1;
    public Sprite bodySprite2;
    public Sprite bodySprite3;
    public Sprite tailSprite1;
    public Sprite tailSprite2;
    public Sprite tailSprite3;
    public Sprite foodSprite1;
    public Sprite foodSprite2;
    public Sprite foodSprite3;
    public Sprite mapSprite1;
    public Sprite mapSprite2;
    public Sprite mapSprite3;
    public Sprite mapSprite4;
    public Sprite mapSprite5;
    public Sprite mapSprite6;
    public Sprite mapSprite7;

    public GameObject obstaclePrefab1;

    private void Awake()
    {
        i = this;

        // Initialize the dictionaries in Awake
        headSprites = new Dictionary<string, Sprite>
        {
            { "head1", headSprite1 },
            { "head2", headSprite2 },
            { "head3", headSprite3 }
        };

        bodySprites = new Dictionary<string, Sprite>
        {
            { "body1", bodySprite1 },
            { "body2", bodySprite2 },
            { "body3", bodySprite3 }
        };

        tailSprites = new Dictionary<string, Sprite>
        {
            { "tail1", tailSprite1 },
            { "tail2", tailSprite2 },
            { "tail3", tailSprite3 }
        };

        foodSprites = new Dictionary<string, Sprite>
        {
            { "food1", foodSprite1 },
            { "food2", foodSprite2 },
            { "food3", foodSprite3 }
        };

        mapSprites = new Dictionary<string, Sprite>
        {
            { "map1", mapSprite1 },
            { "map2", mapSprite2 },
            { "map3", mapSprite3 },
            { "map4", mapSprite4 },
            { "map5", mapSprite5 },
            { "map6", mapSprite6 },
            { "map7", mapSprite7 }
        };

        mapBackgroundColors = new Dictionary<string, Color>
        {
            { "map1", new Color(0.83f, 0.83f, 0.83f) },
            { "map2", Color.black },
            { "map3", Color.green },
            { "map4", new Color(0.29f, 0.40f, 0.55f) },
            { "map5", Color.red },
            { "map6", Color.blue },
            { "map7", Color.yellow }
        };

        LogMapBackgroundColors();
    }

    private void LogMapBackgroundColors()
    {
        foreach (var entry in mapBackgroundColors)
        {
            Debug.Log($"Map: {entry.Key}, Color: {entry.Value}");
        }
    }
}
