// Assets/Scripts/AI/Utility/UtilityLayer.cs
using System.Collections.Generic;

public class UtilityLayer
{
    private List<AIAction> actions = new List<AIAction>();
    private AIScript actor;
    private Blackboard blackboard;
    private GoalLayer goalLayer;

    public UtilityLayer(AIScript actor, Blackboard blackboard, GoalLayer goalLayer)
    {
        this.actor = actor;
        this.blackboard = blackboard;
        this.goalLayer = goalLayer;
    }

    public void Tick(AIContext ctx)
    {
        AIAction best = GetBestAction(ctx);
        best?.Execute();
    }

    AIAction GetBestAction(AIContext ctx)
    {
        AIAction best = null;
        float bestScore = float.MinValue;

        foreach (var action in actions)
        {
            float score = action.GetWeightedScore(ctx);
            if (score > bestScore)
            {
                bestScore = score;
                best = action;
            }
        }
        return best;
    }
    public void AddAction(AIAction action)
    {
        actions.Add(action);
    }
}