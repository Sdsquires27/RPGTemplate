public class FleeAction : AIAction
{
    public FleeAction(AIScript actor, Blackboard blackboard, GoalLayer goalLayer)
        : base(actor, blackboard, goalLayer) { actionName = "Flee"; }

    protected override float Score(AIContext ctx)
    {
        // TODO: score based on health percent and target proximity
        return 0f;
    }

    public override void Execute()
    {
        // TODO: move to neighbor furthest from target
    }
}