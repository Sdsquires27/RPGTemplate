// Assets/Scripts/UI/Panels/MenuPanel.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MenuPanel : UIPanel
{
    [Header("References")]
    [SerializeField] private RectTransform buttonContainer;
    [SerializeField] private RectTransform horizontalContainer;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private GameObject labelPrefab;
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
        SpawnButton(label, onClick, buttonPrefab, buttonContainer);
    }

    public void AddLabelButton(string label, System.Action onClick)
    {
        SpawnButton(label, onClick, labelPrefab, horizontalContainer);
    }

    private void SpawnButton(string label, System.Action onClick, GameObject prefab, RectTransform container)
    {
        GameObject go = Instantiate(prefab, container);
        MenuButton btn = go.GetComponent<MenuButton>();
        btn.Setup(label, onClick, this);
        buttons.Add(btn);
        LayoutRebuilder.ForceRebuildLayoutImmediate(container);
    }

    public void ClearButtons()
    {
        foreach (var btn in buttons)
            Destroy(btn.gameObject);
        buttons.Clear();
    }

    void HandleMouseInput()
{
    Vector2 mousePos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();

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

    if (hoveredButton >= 0 && hoveredButton != selectedIndex)
    {
        selectedIndex = hoveredButton;
        HighlightSelected();
    }

    if (UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame && hoveredButton >= 0)
        buttons[hoveredButton].Click();
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
        HandleMouseInput();
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