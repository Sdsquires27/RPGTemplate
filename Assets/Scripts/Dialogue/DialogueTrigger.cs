using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueData dialogue;
    [SerializeField] private DialoguePanel dialoguePanel;

    public void Trigger()
    {
        if (dialogue == null || dialoguePanel == null) return;
        dialoguePanel.StartDialogue(dialogue);
    }
}