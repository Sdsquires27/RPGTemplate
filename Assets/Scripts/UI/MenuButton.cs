// Assets/Scripts/UI/MenuButton.cs
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Image background;

    private System.Action onClick;
    private MenuPanel parentPanel;

    public void Setup(string text, System.Action action, MenuPanel panel)
    {
        label.text = text;
        onClick = action;
        parentPanel = panel;
    }

    public void Click() => onClick?.Invoke();

    public void SetHighlight(Color color)
    {
        if (background != null)
            background.color = color;
    }

    // Mouse hover — sync keyboard selection to mouse position
    public void OnPointerEnter(PointerEventData eventData)
    {
        parentPanel?.OnButtonHovered(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Click();
    }
}