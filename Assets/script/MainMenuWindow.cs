using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuWindow : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject settingsWindow;

    private void Awake()
    {
        mainMenu.SetActive(true);
        settingsWindow.SetActive(false);

        transform.Find("PlayBut").GetComponent<Button>().onClick.AddListener(() =>
        {
            mainMenu.SetActive(false);
            settingsWindow.SetActive(true);
        });

        transform.Find("QuitBut").GetComponent<Button>().onClick.AddListener(() =>
        {
            Application.Quit();
            Debug.Log("Quit Button Pressed");
        });
    }
}
