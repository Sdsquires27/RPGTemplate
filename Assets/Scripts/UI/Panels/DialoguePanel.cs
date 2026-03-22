using UnityEngine;
using TMPro;
using System.Collections;

public class DialoguePanel : UIPanel
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private ResponsePanel responsePanel;

    private DialogueData currentDialogue;
    private int currentLineIndex;
    private bool waitingForInput = false;
    private bool waitingForResponse = false;

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
        waitingForResponse = false;
        StartCoroutine(WaitForInput());
    }

    protected override void OnClose()
    {
        StopAllCoroutines();
        responsePanel.Hide();
        currentDialogue = null;
        currentLineIndex = 0;
        waitingForInput = false;
        waitingForResponse = false;
    }

    void ShowCurrentLine()
    {
        var line = currentDialogue.lines[currentLineIndex];
        speakerText.text = line.speakerName;
        dialogueText.text = line.text;

        if (line.HasResponses)
        {
            // Filter to only responses whose conditions are met
            waitingForResponse = true;
            waitingForInput = false;
            responsePanel.Show(line.responses, OnResponseSelected);
        }
        else
        {
            waitingForInput = true;
            waitingForResponse = false;
        }
    }

IEnumerator WaitForInput()
{
    yield return null;

    while (isOpen)
    {
        if (waitingForInput && !waitingForResponse &&
            UnityEngine.InputSystem.Keyboard.current.anyKey.wasPressedThisFrame)
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
            currentDialogue.ApplyStateChanges();
            UIManager.Instance.CloseTopPanel();
            return;
        }

        ShowCurrentLine();
    }

    void OnResponseSelected(DialogueResponse response)
    {
        waitingForResponse = false;

        // Apply response state changes
        response.Apply();

        if (response.followUp != null)
        {
            // Switch to follow-up dialogue
            currentDialogue = response.followUp;
            currentLineIndex = 0;
            ShowCurrentLine();
        }
        else
        {
            // No follow-up — advance to next line or end
            AdvanceDialogue();
        }
    }
}