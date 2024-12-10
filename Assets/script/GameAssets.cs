using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// GameAssets.cs
public class GameAssets : MonoBehaviour
{
    public static GameAssets i;

    private void Awake()
    {
        i = this;
    }

    public Dictionary<string, Sprite> headSprites;
    public Dictionary<string, Sprite> bodySprites;
    public Dictionary<string, Sprite> tailSprites;
    public Dictionary<string, Sprite> foodSprites;
    public Dictionary<string, Sprite> mapSprites;

    public Sprite defaultFoodSprite;

    private void Start()
    {
        headSprites = new Dictionary<string, Sprite> {
            { "head1", headSprite1 },
            { "head2", headSprite2 },
            { "head3", headSprite3 },
        };

        bodySprites = new Dictionary<string, Sprite> {
            { "body1", bodySprite1 },
            { "body2", bodySprite2 },
            { "body3", bodySprite3 },
        };

        tailSprites = new Dictionary<string, Sprite> {
            { "tail1", tailSprite1 },
            { "tail2", tailSprite2 },
            { "tail3", tailSprite3 }
        };

        foodSprites = new Dictionary<string, Sprite> {
            { "food1", foodSprite1 },
            { "food2", foodSprite2 },
            { "food3", foodSprite3 },
        };

        mapSprites = new Dictionary<string, Sprite> {
            { "map1", mapSprite1 },
            { "map2", mapSprite2 },
        };
    }

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
}
