using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class DialoguePanel : UIPanel
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI speakerText;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private ResponsePanel responsePanel;

    private DialogueData currentDialogue;
    private DialogueData rootDialogue; // Store the original dialogue to apply its onComplete at the end
    private int currentLineIndex;
    private bool waitingForInput = false;
    private bool waitingForResponse = false;
    private Stack<(DialogueData dialogue, int lineIndex)> dialogueStack = new Stack<(DialogueData, int)>();

    public void StartDialogue(DialogueData dialogue)
    {
        Debug.Log($"[DialoguePanel] StartDialogue called with: {dialogue.name}");
        Debug.Log($"[DialoguePanel] DialogueData has {dialogue.onComplete.Length} onComplete entries");
        for (int i = 0; i < dialogue.onComplete.Length; i++)
        {
            Debug.Log($"  [{i}] {dialogue.onComplete[i].type} - {dialogue.onComplete[i].variableName}");
        }

        currentDialogue = dialogue;
        rootDialogue = dialogue; // Store root for later
        currentLineIndex = 0;
        UIManager.Instance.OpenPanel(this);
        ShowCurrentLine();
    }

    protected override void OnOpen()
    {
        waitingForInput = false;
        waitingForResponse = false;
    }

protected override void OnClose()
{
    StopAllCoroutines();
    responsePanel.Hide();
    currentDialogue = null;
    rootDialogue = null;
    currentLineIndex = 0;
    dialogueStack.Clear(); // Add this
    waitingForInput = false;
    waitingForResponse = false;
}

void ShowCurrentLine()
{
    gameObject.SetActive(true);
    responsePanel.Hide();

    var line = currentDialogue.lines[currentLineIndex];
    speakerText.text = line.speakerName;
    dialogueText.text = line.text;

    var validResponses = line.responses != null
        ? System.Array.FindAll(line.responses, r => r.ConditionsMet())
        : new DialogueResponse[0];

    if (validResponses.Length > 0)
    {
        waitingForInput = false;
        waitingForResponse = false;
        StopAllCoroutines();
        StartCoroutine(WaitThenShowResponses(validResponses));
    }
    else
    {
        waitingForInput = true;
        waitingForResponse = false;
        gameObject.SetActive(true);
        responsePanel.Hide();
        StopAllCoroutines();
        StartCoroutine(WaitForInput());
    }
}

IEnumerator WaitThenShowResponses(DialogueResponse[] validResponses)
{
    // Show the dialogue line and wait for a keypress before showing responses
    yield return null;
    yield return null;

    var keyboard = UnityEngine.InputSystem.Keyboard.current;
    while (true)
    {
        if (keyboard.spaceKey.wasPressedThisFrame ||
            keyboard.enterKey.wasPressedThisFrame ||
            keyboard.escapeKey.wasPressedThisFrame)
            break;
        yield return null;
    }

    waitingForResponse = true;
    gameObject.SetActive(false);
    responsePanel.Show(validResponses, OnResponseSelected);
}

IEnumerator WaitForInput()
{
    yield return null; // skip the frame the coroutine was started on
    yield return null; // skip one more frame as a buffer against carry-over input

    while (isOpen)
    {
        
        if (waitingForInput && !waitingForResponse)
        {
            var keyboard = UnityEngine.InputSystem.Keyboard.current;
            if (keyboard == null)
            {
                yield return null;
                continue;
            }
            
            // Check for specific keys instead of anyKey
            bool spacePressed = keyboard.spaceKey.wasPressedThisFrame;
            bool enterPressed = keyboard.enterKey.wasPressedThisFrame;
            bool escPressed = keyboard.escapeKey.wasPressedThisFrame;
            
            
            if (spacePressed || enterPressed || escPressed)
            {
                AdvanceDialogue();
            }
        }

        yield return null;
    }
}

void AdvanceDialogue()
{
    waitingForInput = false;
    currentLineIndex++;

    while (currentLineIndex >= currentDialogue.lines.Length)
    {
        if (dialogueStack.Count > 0)
        {
            var (parentDialogue, returnIndex) = dialogueStack.Pop();
            currentDialogue = parentDialogue;
            currentLineIndex = returnIndex;
            // loop again to check if the parent is also exhausted
        }
        else
        {
            rootDialogue.ApplyStateChanges();
            UIManager.Instance.CloseTopPanel();
            rootDialogue = null;
            return;
        }
    }

    ShowCurrentLine();
}

void OnResponseSelected(DialogueResponse response)
{
    waitingForResponse = false;
    response.Apply();

    if (response.followUp != null)
    {
        // Push current position so we can return to it
        dialogueStack.Push((currentDialogue, currentLineIndex + 1));
        currentDialogue = response.followUp;
        currentLineIndex = 0;
        ShowCurrentLine();
    }
    else
    {
        AdvanceDialogue();
    }
}
}