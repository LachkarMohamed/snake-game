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
    }

    public void OnReturnToMainMenuButton()
    {
        // Load the main menu scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public void OnTryAgainButton()
    {
        // Reload the current scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void OnPauseButton()
    {
        Debug.Log("Pause button clicked");
        ShowPauseMenu(false);
    }
}
