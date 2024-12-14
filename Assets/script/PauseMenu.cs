using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Button continueButton;
    public Button returnToMainMenuButton;
    public Button tryAgainButton;
    public Button pauseButton;

    private void Start()
    {
        pauseMenuUI.SetActive(false);
        pauseButton.onClick.AddListener(OnPauseButton);
    }

    public void ShowPauseMenu(bool isGameOver)
    {
        pauseMenuUI.SetActive(true);
        continueButton.interactable = !isGameOver;
        Time.timeScale = 0f; // Pause the game
    }

    public void HidePauseMenu()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Resume the game
    }

    public void OnContinueButton()
    {
        HidePauseMenu();
        Snake snake = FindObjectOfType<Snake>();
        if (snake != null)
        {
            snake.SetPause(true);
        }
    }

    public void OnReturnToMainMenuButton()
    {
        // Load the main menu scene
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void OnTryAgainButton()
    {
        // Reset time scale before reloading the scene
        Time.timeScale = 1f;
        GameHandler.instance.ReloadScene();
    }

    public void OnPauseButton()
    {
        ShowPauseMenu(false);
    }
}
