using UnityEngine;
using System;

/// <summary>
/// Represents a hexagonal tile coordinate using the cubic coordinate system.
/// Constraint: x + y + z = 0
/// </summary>
[System.Serializable]
public struct HexCoordinate : IEquatable<HexCoordinate>
{
    public int x;
    public int y;
    public int z;

    public HexCoordinate(int x, int y, int z)
    {
        if (x + y + z != 0)
        {
            Debug.LogWarning($"Invalid hex coordinate ({x}, {y}, {z}). Sum must equal 0. Adjusting z.");
            z = -x - y;
        }
        this.x = x;
        this.y = y;
        this.z = z;
    }

    /// <summary>
    /// Calculate Manhattan distance to another hex coordinate.
    /// </summary>
    public int Distance(HexCoordinate other)
    {
        return (Mathf.Abs(x - other.x) + Mathf.Abs(y - other.y) + Mathf.Abs(z - other.z)) / 2;
    }

    /// <summary>
    /// Get all 6 neighboring hex coordinates.
    /// </summary>
    public HexCoordinate[] GetNeighbors()
    {
        return new[]
        {
            new HexCoordinate(x + 1, y - 1, z),
            new HexCoordinate(x + 1, y, z - 1),
            new HexCoordinate(x, y + 1, z - 1),
            new HexCoordinate(x - 1, y + 1, z),
            new HexCoordinate(x - 1, y, z + 1),
            new HexCoordinate(x, y - 1, z + 1)
        };
    }

    /// <summary>
    /// Get a specific neighbor by direction (0-5).
    /// </summary>
    public HexCoordinate GetNeighbor(int direction)
    {
        direction = direction % 6;
        return GetNeighbors()[direction];
    }

    /// <summary>
    /// Convert to axial coordinates (q, r) for storage/serialization.
    /// </summary>
    public (int q, int r) ToAxial()
    {
        return (x, z);
    }

    /// <summary>
    /// Create from axial coordinates.
    /// </summary>
    public static HexCoordinate FromAxial(int q, int r)
    {
        return new HexCoordinate(q, -q - r, r);
    }

    /// <summary>
    /// Lerp between two hex coordinates (for smooth animation).
    /// </summary>
    public static HexCoordinate Lerp(HexCoordinate a, HexCoordinate b, float t)
    {
        float x = Mathf.Lerp(a.x, b.x, t);
        float y = Mathf.Lerp(a.y, b.y, t);
        float z = Mathf.Lerp(a.z, b.z, t);

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
    /// Get all hex coordinates within a certain range.
    /// </summary>
    public static HexCoordinate[] GetRange(HexCoordinate center, int radius)
    {
        var results = new System.Collections.Generic.List<HexCoordinate>();
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = Mathf.Max(-radius, -x - radius); y <= Mathf.Min(radius, -x + radius); y++)
            {
                results.Add(new HexCoordinate(x + center.x, y + center.y, -x - y + center.z));
            }
        }
        return results.ToArray();
    }

    /// <summary>
    /// Get hex coordinates in a ring at a specific radius.
    /// </summary>
    public static HexCoordinate[] GetRing(HexCoordinate center, int radius)
    {
        if (radius == 0)
            return new[] { center };

        var results = new System.Collections.Generic.List<HexCoordinate>();
        var cube = center.GetNeighbor(4);
        cube = new HexCoordinate(cube.x * radius, cube.y * radius, cube.z * radius);

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < radius; j++)
            {
                results.Add(cube);
                cube = cube.GetNeighbor(i);
            }
        }
        return results.ToArray();
    }

    public override bool Equals(object obj)
    {
        return obj is HexCoordinate coordinate && Equals(coordinate);
    }

    public bool Equals(HexCoordinate other)
    {
        return x == other.x && y == other.y && z == other.z;
    }

    public override int GetHashCode()
    {
        return x.GetHashCode() ^ (y.GetHashCode() << 2) ^ (z.GetHashCode() >> 2);
    }

    public override string ToString()
    {
        return $"HexCoord({x}, {y}, {z})";
    }

    public static bool operator ==(HexCoordinate left, HexCoordinate right) => left.Equals(right);
    public static bool operator !=(HexCoordinate left, HexCoordinate right) => !left.Equals(right);
}
