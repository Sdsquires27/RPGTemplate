// Assets/Scripts/UI/UIManager.cs
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private PlayerInput playerInput;

    private Stack<UIPanel> panelStack = new Stack<UIPanel>();

    public bool isAnyPanelOpen => panelStack.Count > 0;

    void Awake()
{
    Instance = this;
}

    void Start()
    {
        foreach (var panel in FindObjectsByType<UIPanel>(FindObjectsSortMode.None))
            if (panel.hideOnStart)
                panel.gameObject.SetActive(false);
    }

    // -------------------------------------------------------------------------
    // Stack Management
    // -------------------------------------------------------------------------

    public void OpenPanel(UIPanel panel)
    {
        if (panel == null) return;
        if (panel.isOpen) return;

        panelStack.Push(panel);
        panel.Open();

        // Pause and switch input on first panel open
        if (panelStack.Count == 1)
            OnUIOpened();
    }

    public void CloseTopPanel()
    {
        if (panelStack.Count == 0) return;

        UIPanel top = panelStack.Pop();
        top.Close();

        if (panelStack.Count == 0)
            OnUIClosed();
        else
            panelStack.Peek().OnBecameTopOfStack();
    }

    public void CloseAllPanels()
    {
        while (panelStack.Count > 0)
        {
            UIPanel top = panelStack.Pop();
            top.Close();
        }
        OnUIClosed();
    }

    public UIPanel GetTopPanel()
    {
        return panelStack.Count > 0 ? panelStack.Peek() : null;
    }

    // -------------------------------------------------------------------------
    // Pause & Input Switching
    // -------------------------------------------------------------------------

    void OnUIOpened()
    {
        Time.timeScale = 0f;
        SwitchToUIInput();
    }

    void OnUIClosed()
    {
        Time.timeScale = 1f;
        SwitchToGameInput();
    }

    void SwitchToUIInput()
    {
        if (playerInput == null) return;
        playerInput.SwitchCurrentActionMap("UI");
    }

    void SwitchToGameInput()
    {
        if (playerInput == null) return;
        playerInput.SwitchCurrentActionMap("Player");
    }

    // -------------------------------------------------------------------------
    // Back Button
    // -------------------------------------------------------------------------

    void Update()
    {
        // Close top panel on Escape or gamepad B button
        if (isAnyPanelOpen && Keyboard.current != null && 
            Keyboard.current.escapeKey.wasPressedThisFrame)
            CloseTopPanel();
    }
}