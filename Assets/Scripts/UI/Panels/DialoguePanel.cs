// Assets/Scripts/UI/Panels/DialoguePanel.cs
using UnityEngine;
using TMPro;
using System.Collections;

public class DialoguePanel : UIPanel
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;

    private DialogueData currentDialogue;
    private int currentLineIndex;
    private bool waitingForInput = false;

    public void StartDialogue(DialogueData dialogue)
    {
        currentDialogue = dialogue;
        currentLineIndex = 0;
        UIManager.Instance.OpenPanel(this);
        ShowCurrentLine();
    }

    protected override void OnOpen()
    {
        waitingForInput = false;
        StartCoroutine(WaitForInput());
    }

    protected override void OnClose()
    {
        StopAllCoroutines();
        currentDialogue = null;
        currentLineIndex = 0;
        waitingForInput = false;
    }

    void ShowCurrentLine()
    {
        var line = currentDialogue.lines[currentLineIndex];
        speakerText.text = line.speakerName;
        dialogueText.text = line.text;
        waitingForInput = true;
    }

    IEnumerator WaitForInput()
    {
        // Wait one frame to avoid immediately consuming the input that opened dialogue
        yield return null;

        while (isOpen)
        {
            if (waitingForInput && UnityEngine.InputSystem.Keyboard.current.anyKey.wasPressedThisFrame)
                AdvanceDialogue();

            yield return null;
        }
    }

    void AdvanceDialogue()
    {
        waitingForInput = false;
        currentLineIndex++;

        if (currentLineIndex >= currentDialogue.lines.Length)
        {
            UIManager.Instance.CloseTopPanel();
            return;
        }

        ShowCurrentLine();
    }
}