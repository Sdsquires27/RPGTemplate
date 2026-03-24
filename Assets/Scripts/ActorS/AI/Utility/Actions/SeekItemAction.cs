using System.Collections.Generic;
using UnityEngine;

public class SeekItemAction : AIAction
{
    private AIScript npcActor;

    public SeekItemAction(AIScript actor, Blackboard blackboard, GoalLayer goalLayer)
        : base(actor, blackboard, goalLayer) 
        { 
            actionName = "SeekItem";
            npcActor = actor;
        }

    protected override float Score(AIContext ctx)
    {
        return blackboard.Has("seekItem") ? 0.9f : 0f;
    }

    public override void Execute()
{
    
    if (npcActor.IsMoving) return;

    Item item = blackboard.Get<Item>("seekItem");
    
    if (item == null)
    {
        blackboard.Clear("seekItem");
        blackboard.Clear("seekTile");
        return;
    }

    int dist = npcActor.HexGrid.GetDistance(npcActor.CurrentTile, item.currentTile);

    if (dist <= 1)
    {
        PickUpItem(item);
        return;
    }

    List<HexTile> path = Pathfinder.FindPath(
        npcActor.HexGrid, npcActor.CurrentTile, item.currentTile);

    if (path.Count > 0)
        npcActor.MoveToTile(path[0]);
}

    void PickUpItem(Item item)
    {
        // NPC picks up item — attach to NPC transform
        item.PickUp(npcActor.transform);
        blackboard.Clear("seekItem");
        blackboard.Clear("seekTile");
    }
}