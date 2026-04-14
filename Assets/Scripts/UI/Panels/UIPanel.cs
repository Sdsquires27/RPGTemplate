// Assets/Scripts/UI/UIPanel.cs
using UnityEngine;

public abstract class UIPanel : MonoBehaviour
{
    public bool isOpen { get; private set; }
    public bool hideOnStart = true;
    // Called by UIManager — don't call directly
    public void Open()
    {
        isOpen = true;
        gameObject.SetActive(true);
        OnOpen();
    }

    public void Close()
    {
        isOpen = false;
        gameObject.SetActive(false);
        OnClose();
    }

    // Override in subclasses for panel-specific setup
    protected virtual void OnOpen() { }
    protected virtual void OnClose() { }

    // Called when a panel above this one is closed and this becomes top of stack
    public virtual void OnBecameTopOfStack() { }
}