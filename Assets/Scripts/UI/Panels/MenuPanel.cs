// Assets/Scripts/UI/Panels/MenuPanel.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuPanel : UIPanel
{
    [Header("References")]
    [SerializeField] private RectTransform buttonContainer;
    [SerializeField] private GameObject buttonPrefab;

    [Header("Navigation")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color selectedColor = Color.yellow;

    private List<MenuButton> buttons = new List<MenuButton>();
    private int selectedIndex = 0;

    protected override void OnOpen()
    {
        selectedIndex = 0;
        HighlightSelected();
    }

    // -------------------------------------------------------------------------
    // Button Registration
    // -------------------------------------------------------------------------

    public void AddButton(string label, System.Action onClick)
    {
        GameObject go = Instantiate(buttonPrefab, buttonContainer);
        MenuButton btn = go.GetComponent<MenuButton>();
        btn.Setup(label, onClick, this);
        buttons.Add(btn);
        LayoutRebuilder.ForceRebuildLayoutImmediate(buttonContainer);
    }

    public void ClearButtons()
    {
        foreach (var btn in buttons)
            Destroy(btn.gameObject);
        buttons.Clear();
    }

    // -------------------------------------------------------------------------
    // Keyboard / Gamepad Navigation
    // -------------------------------------------------------------------------

    void Update()
    {
        if (!isOpen) return;

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
        buttons[selectedIndex].Click();
    }

    void HighlightSelected()
    {
        for (int i = 0; i < buttons.Count; i++)
            buttons[i].SetHighlight(i == selectedIndex ? selectedColor : normalColor);
    }

    // Called by MenuButton on mouse hover
    public void OnButtonHovered(MenuButton button)
    {
        selectedIndex = buttons.IndexOf(button);
        HighlightSelected();
    }
}