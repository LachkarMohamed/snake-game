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

        ChangeMap(0);
        ChangeSkin(0);
        ChangeSpeed(0);

        mapLeftArrow.onClick.AddListener(() => ChangeMap(-1));
        mapRightArrow.onClick.AddListener(() => ChangeMap(1));
        skinLeftArrow.onClick.AddListener(() => ChangeSkin(-1));
        skinRightArrow.onClick.AddListener(() => ChangeSkin(1));
        speedLeftArrow.onClick.AddListener(() => ChangeSpeed(-1));
        speedRightArrow.onClick.AddListener(() => ChangeSpeed(1));
        startButton.onClick.AddListener(StartGame);
        returnButton.onClick.AddListener(ReturnToMainMenu);
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
