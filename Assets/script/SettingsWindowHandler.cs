using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SettingsWindowHandler : MonoBehaviour
{
    [SerializeField] private Image mapPreviewImage;
    [SerializeField] private Image headPreviewImage;
    [SerializeField] private Image bodyPreviewImage;
    [SerializeField] private Image tailPreviewImage;
    [SerializeField] private Image foodPreviewImage;

    [SerializeField] private Image mapSelectedImage;
    [SerializeField] private Image headSelectedImage;
    [SerializeField] private Image speedSelectedImage;

    [SerializeField] private Button startButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private Button mapLeftArrow;
    [SerializeField] private Button mapRightArrow;
    [SerializeField] private Button skinLeftArrow;
    [SerializeField] private Button skinRightArrow;
    [SerializeField] private Button speedLeftArrow;
    [SerializeField] private Button speedRightArrow;

    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsWindow;

    [SerializeField] private Image[] starImages; // Array of star images (4 stars)

    private int currentMapIndex;
    private int currentHeadIndex;
    private int currentSpeedIndex;

    private List<Sprite> mapSprites;
    private List<Sprite> headSprites;
    private List<Sprite> bodySprites;
    private List<Sprite> tailSprites;
    private List<Sprite> foodSprites;
    private List<Sprite> speedSprites;

    private Sprite selectedMap;
    private Sprite selectedHead;
    private Sprite selectedBody;
    private Sprite selectedTail;
    private Sprite selectedFood;
    private Sprite selectedSpeed;

    private void Awake()
    {
        currentMapIndex = 0;
        currentHeadIndex = 0;
        currentSpeedIndex = 1;

        mapSprites = new List<Sprite>(LoadSprites("texture/Maps"));
        headSprites = new List<Sprite>(LoadSprites("texture/Skins/Heads"));
        bodySprites = new List<Sprite>(LoadSprites("texture/Skins/Bodies"));
        tailSprites = new List<Sprite>(LoadSprites("texture/Skins/Tails"));
        foodSprites = new List<Sprite>(LoadSprites("texture/Skins/Food"));
        speedSprites = new List<Sprite>(LoadSprites("texture/Speed"));

        // Initialize selections based on previously selected values
        if (!string.IsNullOrEmpty(GameHandler.selectedMap))
        {
            for (int i = 0; i < mapSprites.Count; i++)
            {
                if (mapSprites[i].name == GameHandler.selectedMap)
                {
                    currentMapIndex = i;
                    break;
                }
            }
        }

        if (!string.IsNullOrEmpty(GameHandler.selectedHead))
        {
            for (int i = 0; i < headSprites.Count; i++)
            {
                if (headSprites[i].name == GameHandler.selectedHead)
                {
                    currentHeadIndex = i;
                    break;
                }
            }
        }

        if (!string.IsNullOrEmpty(GameHandler.selectedSpeed))
        {
            for (int i = 0; i < speedSprites.Count; i++)
            {
                if (speedSprites[i].name == GameHandler.selectedSpeed)
                {
                    currentSpeedIndex = i;
                    break;
                }
            }
        }

        ChangeMap(0); // Update UI with the current map selection
        ChangeSkin(0); // Update UI with the current skin selection
        ChangeSpeed(0); // Update UI with the current speed selection

        mapLeftArrow.onClick.AddListener(() => ChangeMap(-1));
        mapRightArrow.onClick.AddListener(() => ChangeMap(1));
        skinLeftArrow.onClick.AddListener(() => ChangeSkin(-1));
        skinRightArrow.onClick.AddListener(() => ChangeSkin(1));
        speedLeftArrow.onClick.AddListener(() => ChangeSpeed(-1));
        speedRightArrow.onClick.AddListener(() => ChangeSpeed(1));
        startButton.onClick.AddListener(StartGame);
        returnButton.onClick.AddListener(ReturnToMainMenu);

        // Initialize star images
        if (starImages != null && starImages.Length == 4)
        {
            UpdateStarRating(currentMapIndex);
        }
    }

    private Sprite[] LoadSprites(string path)
    {
        return Resources.LoadAll<Sprite>(path);
    }

    private void ChangeMap(int direction)
    {
        currentMapIndex = (currentMapIndex + direction + mapSprites.Count) % mapSprites.Count;
        selectedMap = mapSprites[currentMapIndex];
        UpdateUI();
        UpdateStarRating(currentMapIndex); // Update star rating when map changes
    }

    private void ChangeSkin(int direction)
    {
        currentHeadIndex = (currentHeadIndex + direction + headSprites.Count) % headSprites.Count;
        selectedHead = headSprites[currentHeadIndex];
        selectedBody = bodySprites[currentHeadIndex];
        selectedTail = tailSprites[currentHeadIndex];
        selectedFood = foodSprites[currentHeadIndex];
        UpdateUI();
    }

    private void ChangeSpeed(int direction)
    {
        currentSpeedIndex = (currentSpeedIndex + direction + speedSprites.Count) % speedSprites.Count;
        selectedSpeed = speedSprites[currentSpeedIndex];
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (mapSprites.Count == 0 || headSprites.Count == 0 || bodySprites.Count == 0 || tailSprites.Count == 0 || foodSprites.Count == 0 || speedSprites.Count == 0)
        {
            Debug.LogError("One or more sprite lists are empty. Cannot update UI.");
            return;
        }

        mapPreviewImage.sprite = mapSprites[currentMapIndex];
        headPreviewImage.sprite = headSprites[currentHeadIndex];
        bodyPreviewImage.sprite = bodySprites[currentHeadIndex];
        tailPreviewImage.sprite = tailSprites[currentHeadIndex];
        foodPreviewImage.sprite = foodSprites[currentHeadIndex];

        mapSelectedImage.sprite = mapSprites[currentMapIndex];
        headSelectedImage.sprite = headSprites[currentHeadIndex];
        speedSelectedImage.sprite = speedSprites[currentSpeedIndex];
    }

    private void UpdateStarRating(int mapIndex)
    {
        int stars = GetStarRatingForMap(mapIndex);

        // Set the appropriate star images
        for (int i = 0; i < starImages.Length; i++)
        {
            if (i < stars)
            {
                starImages[i].sprite = Resources.Load<Sprite>("texture/Stars/star" + stars); // Filled star
            }
            else
            {
                starImages[i].sprite = Resources.Load<Sprite>("texture/Stars/star0"); // Empty star
            }
        }
    }

    private int GetStarRatingForMap(int mapIndex)
    {
        // Map1: 0 stars
        if (mapIndex == 0)
        {
            return 0;
        }
        // Map2 and Map3: 1 star
        else if (mapIndex == 1 || mapIndex == 2)
        {
            return 1;
        }
        // Map4: 2 stars
        else if (mapIndex == 3)
        {
            return 2;
        }
        // Map5, Map6, Map7: 3 stars
        else if (mapIndex == 4 || mapIndex == 5 || mapIndex == 6)
        {
            return 3;
        }
        return 0; // Default
    }

    private void StartGame()
    {
        if (selectedMap == null || selectedHead == null || selectedBody == null || selectedTail == null || selectedFood == null || selectedSpeed == null)
        {
            Debug.LogError("One or more selected parameters are null. Please ensure all selections are made.");
            return;
        }

        GameHandler.selectedMap = selectedMap.name;
        GameHandler.selectedHead = selectedHead.name;
        GameHandler.selectedBody = selectedBody.name;
        GameHandler.selectedTail = selectedTail.name;
        GameHandler.selectedFood = selectedFood.name;
        GameHandler.selectedSpeed = selectedSpeed.name;

        SceneManager.LoadScene("GameScene");
    }

    private void ReturnToMainMenu()
    {
        mainMenu.SetActive(true);
        settingsWindow.SetActive(false);
    }
}