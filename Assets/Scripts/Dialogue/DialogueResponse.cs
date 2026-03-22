using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueResponse")]
public class DialogueResponse : ScriptableObject
{
    [Header("Response Text")]
    public string responseText;

    [Header("Conditions — all must pass for this response to appear")]
    public DialogueCondition[] conditions;

    [Header("Follow-up Dialogue — optional, plays after this response")]
    public DialogueData followUp;

    [Header("State Changes — applied when this response is selected")]
    public DialogueStateChange[] onSelected;

    public bool ConditionsMet()
    {
        foreach (var condition in conditions)
            if (!condition.Evaluate()) return false;
        return true;
    }

    public void Apply()
    {
        foreach (var change in onSelected)
            change.Apply();
    }
}