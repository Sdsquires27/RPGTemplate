using UnityEngine;

/// <summary>
/// Monitors if the NPC's quest objectives are being met in GameState.
/// Adjusts behavior based on objective completion.
/// </summary>
public class MonitorObjectiveAction : AIAction
{
    private PersonalityData personality;

    public MonitorObjectiveAction(AIScript actor, Blackboard blackboard, GoalLayer goalLayer, PersonalityData personality)
        : base(actor, blackboard, goalLayer)
    {
        actionName = "MonitorObjective";
        this.personality = personality;
    }

    protected override float Score(AIContext ctx)
    {
        // Low constant score - this action runs in the background
        return 0.1f;
    }

    public override void Execute()
    {
        QuestState active = personality.GetActiveQuest();
        if (active == null) return;

        // Check if objectives are met
        bool objectivesMet = active.AreObjectivesMet();
        
        // Store in blackboard for other systems to check
        blackboard.Set("objectivesMet", objectivesMet);
        blackboard.Set("currentQuestName", active.questName);
        blackboard.Set("currentQuestPriority", active.questPriority);

        // Debug visualization
        if (objectivesMet)
        {
            // Quest is complete - other systems can react to this
            blackboard.Set("questComplete", true);
        }
        else
        {
            blackboard.Set("questComplete", false);
        }
    }
}
