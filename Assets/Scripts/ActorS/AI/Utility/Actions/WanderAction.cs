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

        List<HexTile> validTiles = new List<HexTile>();
        HexGrid grid = actor.HexGrid;

        // Get all tiles within radius
        for (int q = -radius; q <= radius; q++)
        {
            int r1 = Mathf.Max(-radius, -q - radius);
            int r2 = Mathf.Min(radius, -q + radius);
            for (int r = r1; r <= r2; r++)
            {
                Vector2Int axial = new Vector2Int(centerTile.hex.q + q, centerTile.hex.r + r);
                if (grid.hexTiles.TryGetValue(axial, out HexTile tile) && tile.isWalkable && tile != actor.CurrentTile)
                {
                    validTiles.Add(tile);
                }
            }
        }

        return validTiles;
    }
}