public class AttackAction : AIAction
{
    public AttackAction(AIScript actor, Blackboard blackboard, GoalLayer goalLayer)
        : base(actor, blackboard, goalLayer) { actionName = "Attack"; }

    protected override float Score(AIContext ctx)
    {
        // TODO: score high when target is adjacent
        return 0f;
    }

    public override void Execute()
    {
        // TODO: deal damage to target
    }
}