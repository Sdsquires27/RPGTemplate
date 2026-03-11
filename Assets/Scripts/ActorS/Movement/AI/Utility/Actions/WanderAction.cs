// Assets/Scripts/AI/Utility/Actions/WanderAction.cs
public class WanderAction : AIAction
{
    public WanderAction(AIScript actor, Blackboard blackboard, GoalLayer goalLayer)
        : base(actor, blackboard, goalLayer) { actionName = "Wander"; }

    protected override float Score(AIContext ctx)
    {
        // TODO: low constant score as fallback
        return 0f;
    }

    public override void Execute()
    {
        // TODO: move to random walkable neighbor
    }
}