using System.Collections.Generic;
using UnityEngine;

public class WanderAction : AIAction
{
    private NPCScript npcScript;

    public WanderAction(AIScript actor, Blackboard blackboard, GoalLayer goalLayer)
        : base(actor, blackboard, goalLayer) 
    { 
        actionName = "Wander";
        npcScript = actor as NPCScript; // Cast to access NPC-specific properties
    }

    protected override float Score(AIContext ctx)
    {
        int radius = npcScript?.wanderRadius ?? 3;
        if (radius <= 0) return 0f; // Don't wander if radius is 0 or less

        // Low constant score — always available as fallback
        return 0.2f;
    }

    public override void Execute()
    {
        if (actor.IsMoving) return;

        List<HexTile> walkableNeighbors = GetWalkableNeighborsWithinRadius();
        if (walkableNeighbors.Count == 0) return;

        actor.MoveToTile(walkableNeighbors[Random.Range(0, walkableNeighbors.Count)]);
    }

    private List<HexTile> GetWalkableNeighborsWithinRadius()
    {
        HexTile centerTile = npcScript?.homeTile ?? npcScript?.startingTile ?? actor.CurrentTile;
        int radius = npcScript?.wanderRadius ?? 3;

        // If radius is 0 or less, don't wander at all
        if (radius <= 0)
            return new List<HexTile>();

        List<HexTile> validTiles = new List<HexTile>();
        HexGrid grid = actor.HexGrid;

        // Get adjacent tiles that are within the allowed radius from center
        List<HexTile> adjacent = grid.GetNeighbors(actor.CurrentTile);
        foreach (HexTile neighbor in adjacent)
        {
            if (neighbor.isWalkable && grid.GetDistance(centerTile, neighbor) <= radius)
            {
                validTiles.Add(neighbor);
            }
        }

        return validTiles;
    }
}