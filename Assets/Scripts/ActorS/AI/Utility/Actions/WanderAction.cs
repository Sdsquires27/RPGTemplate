using System.Collections.Generic;
using UnityEngine;

public class WanderAction : AIAction
{
    public WanderAction(AIScript actor, Blackboard blackboard, GoalLayer goalLayer)
        : base(actor, blackboard, goalLayer) { actionName = "Wander"; }

    protected override float Score(AIContext ctx)
    {
        // Low constant score — always available as fallback
        return 0.2f;
    }

    public override void Execute()
    {
        if (actor.IsMoving) return;

        List<HexTile> neighbors = actor.HexGrid.GetNeighbors(actor.CurrentTile);
        List<HexTile> walkable = neighbors.FindAll(t => t.isWalkable);
        if (walkable.Count == 0) return;

        actor.MoveToTile(walkable[Random.Range(0, walkable.Count)]);
    }
}