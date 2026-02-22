using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Provides A* pathfinding on a hexagonal grid.
/// </summary>
public class HexPathfinding : MonoBehaviour
{
    private HexGrid hexGrid;

    private void OnEnable()
    {
        hexGrid = GetComponent<HexGrid>();
    }

    /// <summary>
    /// Find the shortest path between two hex tiles using A*.
    /// </summary>
    public List<HexTile> FindPath(HexCoordinate start, HexCoordinate goal)
    {
        var openSet = new HashSet<HexCoordinate> { start };
        var cameFrom = new Dictionary<HexCoordinate, HexCoordinate>();
        var gScore = new Dictionary<HexCoordinate, float> { { start, 0 } };
        var fScore = new Dictionary<HexCoordinate, float> { { start, Heuristic(start, goal) } };

        while (openSet.Count > 0)
        {
            // Find node with lowest fScore
            HexCoordinate current = default;
            float lowestF = float.MaxValue;
            foreach (HexCoordinate coord in openSet)
            {
                if (!fScore.TryGetValue(coord, out float f))
                    f = float.MaxValue;
                if (f < lowestF)
                {
                    current = coord;
                    lowestF = f;
                }
            }

            if (current.Equals(goal))
            {
                return ReconstructPath(cameFrom, current);
            }

            openSet.Remove(current);
            HexTile[] neighbors = hexGrid.GetWalkableNeighbors(current);

            foreach (HexTile neighbor in neighbors)
            {
                HexCoordinate neighborCoord = neighbor.coordinate;
                float tentativeGScore = gScore[current] + 1f; // All hex neighbors are distance 1

                if (!gScore.TryGetValue(neighborCoord, out float neighborG))
                    neighborG = float.MaxValue;

                if (tentativeGScore < neighborG)
                {
                    cameFrom[neighborCoord] = current;
                    gScore[neighborCoord] = tentativeGScore;
                    fScore[neighborCoord] = tentativeGScore + Heuristic(neighborCoord, goal);

                    if (!openSet.Contains(neighborCoord))
                        openSet.Add(neighborCoord);
                }
            }
        }

        // No path found
        return new List<HexTile>();
    }

    /// <summary>
    /// Heuristic function for A* (hex distance).
    /// </summary>
    private float Heuristic(HexCoordinate a, HexCoordinate b)
    {
        return a.Distance(b);
    }

    /// <summary>
    /// Reconstruct the path from goal to start.
    /// </summary>
    private List<HexTile> ReconstructPath(Dictionary<HexCoordinate, HexCoordinate> cameFrom, HexCoordinate current)
    {
        var path = new List<HexTile> { hexGrid.GetTile(current) };

        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            HexTile tile = hexGrid.GetTile(current);
            if (tile != null)
                path.Insert(0, tile);
        }

        return path;
    }

    /// <summary>
    /// Get tiles reachable within a certain movement cost.
    /// </summary>
    public List<HexTile> GetReachableTiles(HexCoordinate start, int movementCost)
    {
        var reachable = new List<HexTile>();
        var visited = new HashSet<HexCoordinate>();
        var frontier = new Queue<(HexCoordinate coord, int cost)>();

        frontier.Enqueue((start, 0));
        visited.Add(start);

        while (frontier.Count > 0)
        {
            var (current, cost) = frontier.Dequeue();
            
            HexTile currentTile = hexGrid.GetTile(current);
            if (currentTile != null && currentTile.IsWalkable)
            {
                reachable.Add(currentTile);

                if (cost < movementCost)
                {
                    HexTile[] neighbors = hexGrid.GetWalkableNeighbors(current);
                    foreach (HexTile neighbor in neighbors)
                    {
                        if (!visited.Contains(neighbor.coordinate))
                        {
                            visited.Add(neighbor.coordinate);
                            frontier.Enqueue((neighbor.coordinate, cost + 1));
                        }
                    }
                }
            }
        }

        return reachable;
    }
}
