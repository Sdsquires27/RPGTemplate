using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ResponseButton : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Image background;
    private GraphicRaycaster raycaster;

    public DialogueResponse response { get; private set; }
    private ResponsePanel parentPanel;

    private void OnEnable()
    {
        Debug.Log($"[ResponseButton] OnEnable - ID: {gameObject.GetInstanceID()}");
    }

    public void Setup(DialogueResponse r, ResponsePanel panel)
    {
        response = r;
        parentPanel = panel;

        // Auto-find children if not assigned
        if (label == null)
            label = GetComponentInChildren<TextMeshProUGUI>();
        if (background == null)
            background = GetComponentInChildren<Image>();

        // Ensure button can receive raycasts
        if (raycaster == null)
            raycaster = GetComponent<GraphicRaycaster>();
        if (raycaster == null)
            raycaster = gameObject.AddComponent<GraphicRaycaster>();

        // Ensure CanvasGroup doesn't block raycasts
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null)
            Debug.Log($"[ResponseButton] CanvasGroup found: blocksRaycasts={cg.blocksRaycasts}");

        // Set label text
        if (label != null)
            label.text = r.responseText;

        // Debug: Verify setup
        Debug.Log($"[ResponseButton] Set up button: {r.responseText}");
        Debug.Log($"[ResponseButton] Label found: {(label != null ? label.text : "NOT FOUND")}");
        Debug.Log($"[ResponseButton] Background found: {(background != null ? background.name : "NOT FOUND")}");
        Debug.Log($"[ResponseButton] GraphicRaycaster: {(raycaster != null ? "OK" : "FAILED")}");
        Debug.Log($"[ResponseButton] OnEnable will be called on next frame");
    }

    public void SetHighlight(Color color)
    {
        if (background != null)
        {
            background.color = color;
            Debug.Log($"[ResponseButton] Highlight set to {color}, Image active: {background.enabled}, Image visible: {background.gameObject.activeInHierarchy}");
        }
        else
            Debug.LogWarning("[ResponseButton] Cannot highlight — Background is null!");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("[ResponseButton] Pointer Enter detected");
        parentPanel?.OnButtonHovered(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[ResponseButton] Clicked: {response.responseText}");
        parentPanel?.SelectResponse(response);
    }
}