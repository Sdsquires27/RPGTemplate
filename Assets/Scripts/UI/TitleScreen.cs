using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private MenuPanel menuPanel;
    [SerializeField] private DescriptionPanel descriptionPanel;
    [SerializeField] private string gameSceneName;

    void Start()
    {
        ShowMainMenu();
    }

    void ShowMainMenu()
    {
        menuPanel.ClearButtons();
        menuPanel.AddButton("Start Game", () => SceneManager.LoadScene(gameSceneName));
        menuPanel.AddButton("About", () => descriptionPanel.Show(ShowMainMenu));
        menuPanel.AddButton("Quit", () => Application.Quit());
    }
}