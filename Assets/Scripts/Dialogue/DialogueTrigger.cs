using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private DialogueTree dialogueTree;
    [SerializeField] private DialoguePanel dialoguePanel;

    public void Trigger()
    {

        if (dialogueTree == null || dialoguePanel == null) return;

        DialogueData selected = dialogueTree.SelectBestOption();
        if (selected == null)
        {
            return;
        }

        dialoguePanel.StartDialogue(selected);
    }
}