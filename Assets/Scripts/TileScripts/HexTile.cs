using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Represents a single hexagonal tile in the grid.
/// </summary>
public class HexTile : MonoBehaviour
{
    public HexCoordinate coordinate;
    
    [Header("Tile Properties")]
    [SerializeField]
    private string terrainType = "Grass";
    [SerializeField]
    private float heightLevel = 0f;
    [SerializeField]
    private bool isWalkable = true;
    
    private Vector3 worldPosition;
    private List<ActorScript> occupants = new();

    public string TerrainType => terrainType;
    public float HeightLevel => heightLevel;
    public bool IsWalkable => isWalkable;
    public Vector3 WorldPosition => worldPosition;
    public IReadOnlyList<ActorScript> Occupants => occupants;

    /// <summary>
    /// Initialize the tile with coordinate and world position.
    /// </summary>
    public void Initialize(HexCoordinate coord, Vector3 position, string terrain = "Grass", bool walkable = true)
    {
        coordinate = coord;
        worldPosition = position;
        terrainType = terrain;
        isWalkable = walkable;
        transform.position = position;
    }

    /// <summary>
    /// Register an actor as occupying this tile.
    /// </summary>
    public void RegisterActor(ActorScript actor)
    {
        if (!occupants.Contains(actor))
        {
            occupants.Add(actor);
        }
    }

    /// <summary>
    /// Unregister an actor from this tile.
    /// </summary>
    public void UnregisterActor(ActorScript actor)
    {
        occupants.Remove(actor);
    }

    /// <summary>
    /// Check if this tile is occupied.
    /// </summary>
    public bool IsOccupied()
    {
        return occupants.Count > 0;
    }

    /// <summary>
    /// Get the first occupant (usually only one per tile).
    /// </summary>
    public ActorScript GetPrimaryOccupant()
    {
        return occupants.Count > 0 ? occupants[0] : null;
    }

    /// <summary>
    /// Set whether this tile is walkable.
    /// </summary>
    public void SetWalkable(bool walkable)
    {
        isWalkable = walkable;
    }

    /// <summary>
    /// Set the terrain type.
    /// </summary>
    public void SetTerrainType(string terrain)
    {
        terrainType = terrain;
    }

    /// <summary>
    /// Set the height level (for elevation).
    /// </summary>
    public void SetHeightLevel(float height)
    {
        heightLevel = height;
    }

    private void OnDrawGizmos()
    {
        // Draw a small sphere at tile position
        Gizmos.color = isWalkable ? Color.green : Color.red;
        if (occupants.Count > 0)
            Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
