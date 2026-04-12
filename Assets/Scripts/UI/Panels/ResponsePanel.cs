using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class ResponsePanel : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform buttonContainer;
    [SerializeField] private GameObject buttonPrefab;

    [Header("Navigation")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;

    private List<ResponseButton> buttons = new List<ResponseButton>();
    private int selectedIndex = 0;
    private System.Action<DialogueResponse> onSelected;

    public void Show(DialogueResponse[] responses, System.Action<DialogueResponse> callback)
    {
        onSelected = callback;
        ClearButtons();

        Debug.Log($"[ResponsePanel] Show() called with {responses.Length} responses");

        int buttonCount = 0;
        foreach (var response in responses)
        {
            Debug.Log($"  [ResponsePanel] Checking response: '{response.responseText}' - ConditionsMet={response.ConditionsMet()}");
            if (!response.ConditionsMet()) continue;

            GameObject go = Instantiate(buttonPrefab, buttonContainer);
            ResponseButton btn = go.GetComponent<ResponseButton>();
            if (btn == null)
            {
                Debug.LogError("[ResponsePanel] Button prefab missing ResponseButton script!");
                continue;
            }
            btn.Setup(response, this);
            buttons.Add(btn);
            Debug.Log($"[ResponsePanel] Created button #{buttonCount}: '{response.responseText}'");
            buttonCount++;
        }

        Debug.Log($"[ResponsePanel] Show() complete: created {buttonCount} valid response buttons");
        selectedIndex = 0;
        HighlightSelected();
        gameObject.SetActive(true);
        Debug.Log($"[ResponsePanel] GameObject activated, selectedIndex={selectedIndex}, buttons.Count={buttons.Count}");
    }

    public void Hide()
    {
        ClearButtons();
        gameObject.SetActive(false);
    }

    void ClearButtons()
    {
        foreach (var btn in buttons)
            Destroy(btn.gameObject);
        buttons.Clear();
    }

    void Update()
    {
        if (!gameObject.activeSelf || buttons.Count == 0) return;

        // Keyboard navigation
        if (UnityEngine.InputSystem.Keyboard.current.downArrowKey.wasPressedThisFrame ||
            UnityEngine.InputSystem.Gamepad.current?.leftStick.down.wasPressedThisFrame == true)
        {
            Debug.Log("[ResponsePanel] Down arrow pressed");
            NavigateDown();
        }

        if (UnityEngine.InputSystem.Keyboard.current.upArrowKey.wasPressedThisFrame ||
            UnityEngine.InputSystem.Gamepad.current?.leftStick.up.wasPressedThisFrame == true)
        {
            Debug.Log("[ResponsePanel] Up arrow pressed");
            NavigateUp();
        }

        if (UnityEngine.InputSystem.Keyboard.current.enterKey.wasPressedThisFrame ||
            UnityEngine.InputSystem.Gamepad.current?.buttonSouth.wasPressedThisFrame == true)
        {
            Debug.Log("[ResponsePanel] Enter/Confirm pressed");
            ConfirmSelection();
        }

        // Mouse detection (hover + click)
        HandleMouseInput();
    }

    void HandleMouseInput()
    {
        Vector2 mousePos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
        
        // Check which button is under the mouse
        int hoveredButton = -1;
        for (int i = 0; i < buttons.Count; i++)
        {
            RectTransform rt = buttons[i].GetComponent<RectTransform>();
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, mousePos, null, out Vector2 localPoint))
            {
                if (rt.rect.Contains(localPoint))
                {
                    hoveredButton = i;
                    break;
                }
            }
        }

        // Update highlight if hovering over a button
        if (hoveredButton >= 0 && hoveredButton != selectedIndex)
        {
            selectedIndex = hoveredButton;
            HighlightSelected();
        }

        // Detect click
        if (!UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
            return;

        if (hoveredButton >= 0)
        {
            Debug.Log($"[ResponsePanel] Mouse clicked button #{hoveredButton}: {buttons[hoveredButton].response.responseText}");
            SelectResponse(buttons[hoveredButton].response);
        }
    }

    void NavigateDown()
    {
        selectedIndex = (selectedIndex + 1) % buttons.Count;
        HighlightSelected();
    }

    void NavigateUp()
    {
        selectedIndex = (selectedIndex - 1 + buttons.Count) % buttons.Count;
        HighlightSelected();
    }

    void ConfirmSelection()
    {
        if (buttons.Count == 0)
        {
            Debug.Log("[ResponsePanel] ConfirmSelection called but buttons.Count == 0");
            return;
        }
        Debug.Log($"[ResponsePanel] ConfirmSelection: selectedIndex={selectedIndex}, button={buttons[selectedIndex].response.responseText}");
        SelectResponse(buttons[selectedIndex].response);
    }

    void HighlightSelected()
    {
        for (int i = 0; i < buttons.Count; i++)
            buttons[i].SetHighlight(i == selectedIndex ? selectedColor : normalColor);
    }

    public void OnButtonHovered(ResponseButton button)
    {
        selectedIndex = buttons.IndexOf(button);
        Debug.Log($"[ResponsePanel] Button hovered, index: {selectedIndex}");
        HighlightSelected();
    }

    public void SelectResponse(DialogueResponse response)
    {
        Debug.Log($"[ResponsePanel] Response selected: {response.responseText}");
        Hide();
        onSelected?.Invoke(response);
    }
}