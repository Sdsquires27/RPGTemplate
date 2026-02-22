using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages the hexagonal grid and tile creation/retrieval.
/// </summary>
public class HexGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField]
    private float hexRadius = 1f;
    [SerializeField]
    private Vector3 gridOrigin = Vector3.zero;
    
    private Dictionary<HexCoordinate, HexTile> tiles = new();
    private Transform tilesContainer;

    public float HexRadius => hexRadius;
    public Vector3 GridOrigin => gridOrigin;

    private void Awake()
    {
        // Create a container for all tiles
        tilesContainer = transform.Find("TilesContainer");
        if (tilesContainer == null)
        {
            GameObject container = new("TilesContainer");
            tilesContainer = container.transform;
            tilesContainer.SetParent(transform);
            tilesContainer.localPosition = Vector3.zero;
        }
    }

    /// <summary>
    /// Convert hex coordinates to world position.
    /// Uses the "pointy-top" hex orientation.
    /// </summary>
    public Vector3 HexToWorldPosition(HexCoordinate hex)
    {
        float x = hexRadius * (3f / 2f * hex.x);
        float y = hexRadius * (Mathf.Sqrt(3f) / 2f * hex.x + Mathf.Sqrt(3f) * hex.z);
        return gridOrigin + new Vector3(x, 0, y);
    }

    /// <summary>
    /// Convert world position to hex coordinates (rounded to nearest hex).
    /// </summary>
    public HexCoordinate WorldToHexPosition(Vector3 worldPos)
    {
        Vector3 local = worldPos - gridOrigin;
        
        float q = (2f / 3f * local.x) / hexRadius;
        float r = (-1f / 3f * local.x + Mathf.Sqrt(3f) / 3f * local.z) / hexRadius;
        
        float x = q;
        float z = r;
        float y = -x - z;

        float rx = Mathf.Round(x);
        float ry = Mathf.Round(y);
        float rz = Mathf.Round(z);

        float x_diff = Mathf.Abs(rx - x);
        float y_diff = Mathf.Abs(ry - y);
        float z_diff = Mathf.Abs(rz - z);

        if (x_diff > y_diff && x_diff > z_diff)
            rx = -ry - rz;
        else if (y_diff > z_diff)
            ry = -rx - rz;
        else
            rz = -rx - ry;

        return new HexCoordinate((int)rx, (int)ry, (int)rz);
    }

    /// <summary>
    /// Create or get a tile at the specified coordinate.
    /// </summary>
    public HexTile GetOrCreateTile(HexCoordinate coord, string terrain = "Grass", bool walkable = true)
    {
        if (tiles.TryGetValue(coord, out HexTile existing))
            return existing;

        // Create new tile
        GameObject tileGO = new($"Tile_{coord}");
        tileGO.transform.SetParent(tilesContainer);
        
        HexTile tile = tileGO.AddComponent<HexTile>();
        Vector3 worldPos = HexToWorldPosition(coord);
        tile.Initialize(coord, worldPos, terrain, walkable);
        
        tiles[coord] = tile;
        return tile;
    }

    /// <summary>
    /// Get a tile at the specified coordinate.
    /// </summary>
    public HexTile GetTile(HexCoordinate coord)
    {
        tiles.TryGetValue(coord, out HexTile tile);
        return tile;
    }

    /// <summary>
    /// Get all tiles in a range from center.
    /// </summary>
    public HexTile[] GetTilesInRange(HexCoordinate center, int radius)
    {
        HexCoordinate[] coords = HexCoordinate.GetRange(center, radius);
        var result = new List<HexTile>();
        
        foreach (HexCoordinate coord in coords)
        {
            if (tiles.TryGetValue(coord, out HexTile tile))
                result.Add(tile);
        }
        
        return result.ToArray();
    }

    /// <summary>
    /// Get all walkable neighbors of a tile.
    /// </summary>
    public HexTile[] GetWalkableNeighbors(HexCoordinate coord)
    {
        var neighbors = new List<HexTile>();
        HexCoordinate[] neighborCoords = coord.GetNeighbors();
        
        foreach (HexCoordinate neighborCoord in neighborCoords)
        {
            if (tiles.TryGetValue(neighborCoord, out HexTile tile) && tile.IsWalkable)
                neighbors.Add(tile);
        }
        
        return neighbors.ToArray();
    }

    /// <summary>
    /// Generate a rectangular grid of hex tiles.
    /// </summary>
    public void GenerateRectangularGrid(int width, int height, string defaultTerrain = "Grass")
    {
        for (int x = -width / 2; x < width / 2; x++)
        {
            for (int z = -height / 2; z < height / 2; z++)
            {
                HexCoordinate coord = new(x, -x - z, z);
                GetOrCreateTile(coord, defaultTerrain, true);
            }
        }
    }

    /// <summary>
    /// Generate a circular grid of hex tiles.
    /// </summary>
    public void GenerateCircularGrid(int radius, string defaultTerrain = "Grass")
    {
        HexCoordinate center = new(0, 0, 0);
        HexCoordinate[] coords = HexCoordinate.GetRange(center, radius);
        
        foreach (HexCoordinate coord in coords)
        {
            GetOrCreateTile(coord, defaultTerrain, true);
        }
    }

    /// <summary>
    /// Clear all tiles from the grid.
    /// </summary>
    public void ClearGrid()
    {
        tiles.Clear();
        foreach (Transform child in tilesContainer)
        {
            Destroy(child.gameObject);
        }
    }

    /// <summary>
    /// Get total tile count.
    /// </summary>
    public int TileCount => tiles.Count;
}
