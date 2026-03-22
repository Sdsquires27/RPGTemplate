using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueTree dialogueTree;
    [SerializeField] private DialoguePanel dialoguePanel;

    public void Trigger()
    {
       Debug.Log($"Trigger called on {gameObject.name}");

        if (dialogueTree == null || dialoguePanel == null) return;

        DialogueData selected = dialogueTree.SelectBestOption();
        if (selected == null)
        {
            Debug.Log($"{gameObject.name} has no valid dialogue for current state.");
            return;
        }

        dialoguePanel.StartDialogue(selected);
    }
}