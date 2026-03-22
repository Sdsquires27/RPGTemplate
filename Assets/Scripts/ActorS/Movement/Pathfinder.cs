// Assets/Scripts/Pathfinder.cs
using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder
{
    static float GetScore(Dictionary<HexTile, float> scores, HexTile tile)
    {
        return scores.TryGetValue(tile, out float val) ? val : float.MaxValue;
    }

    public static List<HexTile> FindPath(HexGrid hexGrid, HexTile start, HexTile goal)
    {
        if (start == null || goal == null) return new List<HexTile>();

        Dictionary<HexTile, float> gScore = new Dictionary<HexTile, float>();
        Dictionary<HexTile, float> fScore = new Dictionary<HexTile, float>();
        Dictionary<HexTile, HexTile> cameFrom = new Dictionary<HexTile, HexTile>();
        List<HexTile> openSet = new List<HexTile>();
        HashSet<HexTile> closedSet = new HashSet<HexTile>();

        gScore[start] = 0;
        fScore[start] = hexGrid.GetDistance(start, goal);
        openSet.Add(start);

        while (openSet.Count > 0)
        {
            HexTile current = openSet[0];
            foreach (var tile in openSet)
                if (GetScore(fScore, tile) < GetScore(fScore, current))
                    current = tile;

            if (current == goal)
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);
            closedSet.Add(current);

            foreach (HexTile neighbor in hexGrid.GetNeighbors(current))
            {
                bool passable = neighbor.isWalkable || neighbor == goal;
                if (closedSet.Contains(neighbor) || !passable) continue;

                float tentativeG = GetScore(gScore, current) + neighbor.movementCost;

                if (tentativeG < GetScore(gScore, neighbor))
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    fScore[neighbor] = tentativeG + hexGrid.GetDistance(neighbor, goal);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return new List<HexTile>();
    }

    static List<HexTile> ReconstructPath(Dictionary<HexTile, HexTile> cameFrom, HexTile current)
    {
        List<HexTile> path = new List<HexTile>();
        while (cameFrom.ContainsKey(current))
        {
            path.Insert(0, current);
            current = cameFrom[current];
        }
        return path;
    }
}