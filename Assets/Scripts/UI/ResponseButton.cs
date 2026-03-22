using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ResponseButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Image background;

    public DialogueResponse response { get; private set; }
    private ResponsePanel parentPanel;

    public void Setup(DialogueResponse r, ResponsePanel panel)
    {
        response = r;
        label.text = r.responseText;
        parentPanel = panel;
    }

    public void SetHighlight(Color color)
    {
        if (background != null)
            background.color = color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        parentPanel?.OnButtonHovered(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        parentPanel?.SelectResponse(response);
    }
}