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

        foreach (var response in responses)
        {
            if (!response.ConditionsMet()) continue;

            GameObject go = Instantiate(buttonPrefab, buttonContainer);
            ResponseButton btn = go.GetComponent<ResponseButton>();
            btn.Setup(response, this);
            buttons.Add(btn);
        }

        selectedIndex = 0;
        HighlightSelected();
        gameObject.SetActive(true);
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

        if (UnityEngine.InputSystem.Keyboard.current.downArrowKey.wasPressedThisFrame ||
            UnityEngine.InputSystem.Gamepad.current?.leftStick.down.wasPressedThisFrame == true)
            NavigateDown();

        if (UnityEngine.InputSystem.Keyboard.current.upArrowKey.wasPressedThisFrame ||
            UnityEngine.InputSystem.Gamepad.current?.leftStick.up.wasPressedThisFrame == true)
            NavigateUp();

        if (UnityEngine.InputSystem.Keyboard.current.enterKey.wasPressedThisFrame ||
            UnityEngine.InputSystem.Gamepad.current?.buttonSouth.wasPressedThisFrame == true)
            ConfirmSelection();
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
        if (buttons.Count == 0) return;
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
        HighlightSelected();
    }

    public void SelectResponse(DialogueResponse response)
    {
        Hide();
        onSelected?.Invoke(response);
    }
}