// Assets/Scripts/AI/Utility/Actions/ChaseAction.cs
public class ChaseAction : AIAction
{
    public ChaseAction(AIScript actor, Blackboard blackboard, GoalLayer goalLayer)
        : base(actor, blackboard, goalLayer) { actionName = "Chase"; }

    protected override float Score(AIContext ctx)
    {
        // TODO: score based on target distance, detection range etc.
        return 0f;
    }

    public override void Execute()
    {
        // TODO: pathfind toward target one step at a time
    }
}