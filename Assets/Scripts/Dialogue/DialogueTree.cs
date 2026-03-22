using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/DialogueTree")]
public class DialogueTree : ScriptableObject
{
    public DialogueData[] options;

    public DialogueData SelectBestOption()
    {
        DialogueData best = null;
        int bestPriority = int.MinValue;

        foreach (var option in options)
        {
            if (!option.ConditionsMet()) continue;
            if (option.priority > bestPriority)
            {
                bestPriority = option.priority;
                best = option;
            }
        }

        return best;
    }
}