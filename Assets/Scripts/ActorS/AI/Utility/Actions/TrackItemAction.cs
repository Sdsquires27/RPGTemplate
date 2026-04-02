using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Tracks desired items on the grid without picking them up.
/// Remembers what items the NPC wants and where they are.
/// </summary>
public class TrackItemAction : AIAction
{
    private PersonalityData personality;

    public TrackItemAction(AIScript actor, Blackboard blackboard, GoalLayer goalLayer, PersonalityData personality)
        : base(actor, blackboard, goalLayer)
    {
        actionName = "TrackItem";
        this.personality = personality;
    }

    protected override float Score(AIContext ctx)
    {
        // Only score if we have active desired items
        ItemData[] desiredItems = personality.GetActiveDesiredItems();
        if (desiredItems.Length == 0) return 0f;

        // High priority if items are on the grid
        return blackboard.Has("trackedItems") ? 0.7f : 0.3f;
    }

    public override void Execute()
    {
        ItemData[] desiredItems = personality.GetActiveDesiredItems();
        if (desiredItems.Length == 0)
        {
            blackboard.Clear("trackedItems");
            return;
        }

        // Scan grid for desired items
        HexGrid grid = actor.HexGrid;
        List<Item> foundItems = new List<Item>();

        foreach (var tile in grid.hexTiles.Values)
        {
            Item itemOnTile = tile.occupiedBy as Item;
            if (itemOnTile == null) continue;

            // Check if this item is desired
            foreach (ItemData desired in desiredItems)
            {
                if (itemOnTile.data == desired)
                {
                    foundItems.Add(itemOnTile);
                    break;
                }
            }
        }

        // Store tracked items in blackboard
        if (foundItems.Count > 0)
        {
            blackboard.Set("trackedItems", foundItems);
            // Also store the nearest one for reference
            int nearestDist = int.MaxValue;
            Item nearest = null;
            foreach (Item item in foundItems)
            {
                int dist = grid.GetDistance(actor.CurrentTile, item.currentTile);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = item;
                }
            }
            if (nearest != null)
                blackboard.Set("nearestTrackedItem", nearest);
        }
        else
        {
            blackboard.Clear("trackedItems");
            blackboard.Clear("nearestTrackedItem");
        }
    }
}
