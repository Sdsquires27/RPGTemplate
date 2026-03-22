// Assets/Scripts/AI/Utility/AIAction.cs
public abstract class AIAction
{
    protected AIScript actor;
    protected Blackboard blackboard;
    protected GoalLayer goalLayer;

    public string actionName { get; protected set; }


    public AIAction(AIScript actor, Blackboard blackboard, GoalLayer goalLayer)
    {
        this.actor = actor;
        this.blackboard = blackboard;
        this.goalLayer = goalLayer;
    }

    public float GetWeightedScore(AIContext ctx)
    {
        float baseScore = Score(ctx);
        float goalMultiplier = goalLayer.GetGoalMultiplier(actionName);
        return baseScore * goalMultiplier;
    }

    // Each action implements its own scoring
    protected abstract float Score(AIContext ctx);
    public abstract void Execute();
}