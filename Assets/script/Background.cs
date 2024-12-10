using UnityEngine;
public class Background : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        ApplyBackground();
    }

    private void ApplyBackground()
    {
        Debug.Log($"Selected map: {GameHandler.selectedMap}");
        if (GameAssets.i.mapSprites.TryGetValue(GameHandler.selectedMap, out Sprite mapSprite))
        {
            spriteRenderer.sprite = mapSprite;
        }
        else
        {
            Debug.LogError($"Map sprite '{GameHandler.selectedMap}' not found.");
            foreach (var key in GameAssets.i.mapSprites.Keys)
            {
                Debug.Log($"Available map sprite key: {key}");
            }
        }
    }
}
