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
    private DialogueData rootDialogue; // Store the original dialogue to apply its onComplete at the end
    private int currentLineIndex;
    private bool waitingForInput = false;
    private bool waitingForResponse = false;

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
        StartCoroutine(WaitForInput());
    }

    protected override void OnClose()
    {
        StopAllCoroutines();
        responsePanel.Hide();
        currentDialogue = null;
        rootDialogue = null;
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
            // Dialogue chain complete - apply ROOT dialogue's state changes
            Debug.Log($"[DialoguePanel] Dialogue chain complete. Applying '{rootDialogue.name}' onComplete ({rootDialogue.onComplete.Length} entries)");
            for (int i = 0; i < rootDialogue.onComplete.Length; i++)
            {
                var change = rootDialogue.onComplete[i];
                Debug.Log($"[DialoguePanel] Change #{i}: {change.type} - varName='{change.variableName}' boolVal={change.boolValue} intVal={change.intValue}");
            }
            rootDialogue.ApplyStateChanges();
            Debug.Log("[DialoguePanel] GameState after ApplyStateChanges:");
            GameState.PrintDebug();
            UIManager.Instance.CloseTopPanel();
            rootDialogue = null;
            return;
        }

        ShowCurrentLine();
    }

    void OnResponseSelected(DialogueResponse response)
    {
        waitingForResponse = false;

        // Apply response state changes IMMEDIATELY
        Debug.Log($"[DialoguePanel] Response selected: '{response.responseText}'");
        Debug.Log($"[DialoguePanel] Applying {response.onSelected.Length} response state changes");
        response.Apply();
        Debug.Log("[DialoguePanel] Response state changes applied. GameState:");
        GameState.PrintDebug();

        if (response.followUp != null)
        {
            Debug.Log($"[DialoguePanel] Response has followUp dialogue, switching to it");
            // Switch to follow-up dialogue
            currentDialogue = response.followUp;
            currentLineIndex = 0;
            ShowCurrentLine();
        }
        else
        {
            Debug.Log($"[DialoguePanel] Response has no followUp, advancing dialogue");
            // No follow-up — advance to next line or end
            AdvanceDialogue();
        }
    }
}