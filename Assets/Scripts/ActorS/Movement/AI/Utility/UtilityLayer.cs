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

        // Register all available actions
        actions.Add(new ChaseAction(actor, blackboard, goalLayer));
        actions.Add(new AttackAction(actor, blackboard, goalLayer));
        actions.Add(new FleeAction(actor, blackboard, goalLayer));
        actions.Add(new WanderAction(actor, blackboard, goalLayer));
        // TODO: Add more actions as needed
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
}