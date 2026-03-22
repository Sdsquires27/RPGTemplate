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
    Debug.Log($"SeekItem Execute — IsMoving: {npcActor.IsMoving}");
    
    if (npcActor.IsMoving) return;

    Item item = blackboard.Get<Item>("seekItem");
    Debug.Log($"Item from blackboard: {(item == null ? "NULL" : item.data.itemName)}");
    
    if (item == null)
    {
        blackboard.Clear("seekItem");
        blackboard.Clear("seekTile");
        return;
    }

    int dist = npcActor.HexGrid.GetDistance(npcActor.CurrentTile, item.currentTile);
    Debug.Log($"Distance to item: {dist}");

    if (dist <= 1)
    {
        PickUpItem(item);
        return;
    }

    List<HexTile> path = Pathfinder.FindPath(
        npcActor.HexGrid, npcActor.CurrentTile, item.currentTile);
    Debug.Log($"Path count: {path.Count}");

    if (path.Count > 0)
        npcActor.MoveToTile(path[0]);
}

    void PickUpItem(Item item)
    {
        // NPC picks up item — attach to NPC transform
        item.PickUp(npcActor.transform);
        blackboard.Clear("seekItem");
        blackboard.Clear("seekTile");
        Debug.Log($"{npcActor.gameObject.name} picked up {item.data.itemName}!");
    }
}