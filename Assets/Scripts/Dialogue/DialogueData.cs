using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public class DialogueLine
    {
        public string speakerName;
        [TextArea(2, 5)]
        public string text;

        [Header("Responses — if empty, any key advances dialogue")]
        public DialogueResponse[] responses;

        public bool HasResponses => responses != null && responses.Length > 0;
    }

    [Header("Content")]
    public DialogueLine[] lines;

    [Header("Conditions — all must pass for this option to be selected")]
    public DialogueCondition[] conditions;

    [Header("Priority — higher wins when multiple options pass")]
    public int priority = 0;

    [Header("State Changes — applied when this dialogue completes")]
    public DialogueStateChange[] onComplete;

    public bool ConditionsMet()
    {
        foreach (var condition in conditions)
            if (!condition.Evaluate()) return false;
        return true;
    }

    public void ApplyStateChanges()
    {
        foreach (var change in onComplete)
            change.Apply();
    }
}