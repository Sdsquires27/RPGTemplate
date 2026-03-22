// Assets/Scripts/AI/Utility/Actions/DetectItemAction.cs
using UnityEngine;

public class DetectItemAction : AIAction
{
    public DetectItemAction(AIScript actor, Blackboard blackboard, GoalLayer goalLayer)
        : base(actor, blackboard, goalLayer) { actionName = "DetectItem"; }

    protected override float Score(AIContext ctx)
    {
        // Always scan for items
        return 0.6f;
    }

    public override void Execute()
    {
        // Find all items on the grid
        HexGrid grid = actor.HexGrid;
        Item nearestItem = null;
        int nearestDist = int.MaxValue;

        foreach (var tile in grid.hexTiles.Values)
        {
            if (tile.occupiedBy == null) continue;

            int dist = grid.GetDistance(actor.CurrentTile, tile);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearestItem = tile.occupiedBy;
            }
        }

        if (nearestItem != null)
        {
            blackboard.Set("seekItem", nearestItem);
            blackboard.Set("seekTile", nearestItem.currentTile);
        }
        else
        {
            blackboard.Clear("seekItem");
            blackboard.Clear("seekTile");
        }
    }
}