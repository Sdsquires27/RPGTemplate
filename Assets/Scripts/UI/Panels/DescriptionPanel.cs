using UnityEngine;
using TMPro;

public class DescriptionPanel : UIPanel
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private MenuPanel menuPanel;

    [Header("Content")]
    [TextArea(3, 10)]
    [SerializeField] private string[] pages;

    private int currentPage = 0;
    private System.Action onComplete;

    public void Show(System.Action onBack)
    {
        onComplete = onBack;
        currentPage = 0;
        UIManager.Instance.OpenPanel(this);
        ShowCurrentPage();
    }

    void ShowCurrentPage()
    {
        descriptionText.text = pages[currentPage];
        menuPanel.ClearButtons();

        bool isLastPage = currentPage >= pages.Length - 1;

        if (currentPage > 0)
            menuPanel.AddLabelButton("Back", PreviousPage);

        if (isLastPage)
            menuPanel.AddLabelButton("Return to Menu", () =>
            {
                UIManager.Instance.CloseTopPanel();
                onComplete?.Invoke();
            });
        else
            menuPanel.AddLabelButton("Continue", NextPage);
    }

    void NextPage()
    {
        currentPage++;
        ShowCurrentPage();
    }

    void PreviousPage()
    {
        currentPage--;
        ShowCurrentPage();
    }

    protected override void OnOpen() { }
    protected override void OnClose() 
    {
        menuPanel.ClearButtons();
    }
}