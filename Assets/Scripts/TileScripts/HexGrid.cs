using UnityEngine;
using System.Collections.Generic;

public class HexGrid : MonoBehaviour
{

    
    [Header("Hex Settings")]
    public GameObject hexPrefab;
    public float hexSize = 1f;
    public int gridRadius = 5;
    [Header("Tile Palette")]
    public GameObject[] tilePalette;
    public int selectedTileIndex = 0;

    public Dictionary<Vector2Int, HexTile> hexTiles { get; private set; }

void Start()
{
    hexTiles = new Dictionary<Vector2Int, HexTile>();

    // Auto-calculate hexSize from the prefab's sprite
    SpriteRenderer sr = hexPrefab.GetComponent<SpriteRenderer>();
    if (sr != null && sr.sprite != null)
    {
        // Use .y because the sprite is rotated 90°, so y becomes the visual width
        hexSize = sr.sprite.bounds.size.y / 2f;
        Debug.Log($"Auto-calculated hexSize: {hexSize}");
    }
    if (transform.childCount == 0)
        GenerateGrid();
    else
        LoadEditorPlacedHexes();
}

    // -------------------------------------------------------------------------
    // Grid Generation
    // -------------------------------------------------------------------------

    void GenerateGrid()
    {
        for (int q = -gridRadius; q <= gridRadius; q++)
        {
            int rMin = Mathf.Max(-gridRadius, -q - gridRadius);
            int rMax = Mathf.Min(gridRadius, -q + gridRadius);
            for (int r = rMin; r <= rMax; r++)
            {
                SpawnHex(new Vector2Int(q, r));
            }
        }
    }

    void SpawnHex(Vector2Int axial)
    {
        Vector2 worldPos = hexToPixel(axial, hexSize);
        GameObject go = Instantiate(hexPrefab, new Vector3(worldPos.x, worldPos.y, 0), Quaternion.identity);
        go.transform.parent = transform;

        HexTile tile = go.GetComponent<HexTile>();
        tile.Init(axial);
        hexTiles[axial] = tile;
    }

    /// <summary>
    /// When hexes were painted in the editor, register them into the dictionary at runtime.
    /// </summary>
    void LoadEditorPlacedHexes()
    {
        foreach (Transform child in transform)
        {
            HexTile tile = child.GetComponent<HexTile>();
            if (tile != null)
                hexTiles[tile.hex.axial] = tile;
        }
    }

    // -------------------------------------------------------------------------
    // Editor Methods (called by HexGridEditor)
    // -------------------------------------------------------------------------

#if UNITY_EDITOR
    public void EditorSpawnHex(Vector2Int axial)
    {
        if (hexTiles == null) hexTiles = new Dictionary<Vector2Int, HexTile>();
        if (hexTiles.ContainsKey(axial)) return;

        // Use selected tile from palette instead of hexPrefab
        GameObject prefab = tilePalette != null && tilePalette.Length > 0
            ? tilePalette[selectedTileIndex]
            : hexPrefab;

        Vector2 worldPos = hexToPixel(axial, hexSize);
        GameObject go = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, transform);
        go.transform.position = new Vector3(worldPos.x, worldPos.y, 0);

        HexTile tile = go.GetComponent<HexTile>();
        tile.Init(axial);
        hexTiles[axial] = tile;

        UnityEditor.EditorUtility.SetDirty(go);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
        );
    }

    public void EditorRemoveHex(Vector2Int axial)
    {
        if (hexTiles == null || !hexTiles.ContainsKey(axial)) return;

        GameObject go = hexTiles[axial].gameObject;
        hexTiles.Remove(axial);
        DestroyImmediate(go);

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
        );
    }

    public void ClearAll()
    {
        if (hexTiles == null) return;
        foreach (var tile in hexTiles.Values)
            if (tile != null) DestroyImmediate(tile.gameObject);
        hexTiles.Clear();

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene()
        );
    }
#endif

    // -------------------------------------------------------------------------
    // Public Wrappers (used by HexGridEditor)
    // -------------------------------------------------------------------------

    public Vector2 HexToPixelPublic(Vector2Int axial) => hexToPixel(axial, hexSize);
    public Vector2Int PixelToAxialPublic(Vector2 pixel) => pixelToAxial(pixel, hexSize);

    // -------------------------------------------------------------------------
    // Hex Math
    // -------------------------------------------------------------------------

    #region BasicHexMath

    Hex axialSubtract(Hex a, Hex b)
    {
        return new Hex(new Vector2Int(a.axial.x - b.axial.x, a.axial.y - b.axial.y));
    }

    Hex axialAdd(Hex hex, Hex vec)
    {
        return new Hex(new Vector2Int(hex.axial.x + vec.axial.x, hex.axial.y + vec.axial.y));
    }

    int axialDistance(Hex a, Hex b)
    {
        Hex diff = axialSubtract(a, b);
        return (int)(Mathf.Abs(diff.axial.x) + Mathf.Abs(diff.axial.y) + Mathf.Abs(findSCoordinate(diff.axial))) / 2;
    }

    float axialDistance(HexTile a, HexTile b)
    {
        return axialDistance(a.hex, b.hex);
    }

    float findSCoordinate(Vector2Int axial)
    {
        return -axial.x - axial.y;
    }

    #endregion

    // -------------------------------------------------------------------------
    // Line Draw
    // -------------------------------------------------------------------------

    #region LineDraw

    float lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }


    Hex hexLerp(Hex a, Hex b, float t)
    {
        return new Hex(new Vector2Int(
            Mathf.RoundToInt(lerp(a.axial.x, b.axial.x, t)),
            Mathf.RoundToInt(lerp(a.axial.y, b.axial.y, t))
        ));
    }

    Hex[] hexLineDraw(Hex a, Hex b)
    {
        int N = axialDistance(a, b);
        Hex[] results = new Hex[N + 1];
        for (int i = 0; i <= N; i++)
        {
            results[i] = hexLerp(a, b, 1f / N * i);
        }
        return results;
    }

    #endregion

    // -------------------------------------------------------------------------
    // Pixel <-> Hex Conversions
    // -------------------------------------------------------------------------

    #region PixelHexConversions

    Vector2 hexToPixel(Vector2Int axial, float size)
    {
        float overlap = 1.005f; // nudge tiles 0.5% closer together
        var x = size * overlap * (3f / 2f * axial.x);
        var y = size * overlap * (Mathf.Sqrt(3) / 2f * axial.x + Mathf.Sqrt(3) * axial.y);
        return new Vector2(x, y);
    }

    // Kept for internal Hex struct usage
    Vector2 hexToPixel(Hex hex, float size)
    {
        var x = (Mathf.Sqrt(3) * hex.q + Mathf.Sqrt(3) / 2f * hex.r) * size;
        var y = (3f / 2f * hex.r) * size;
        return new Vector2(x, y);
    }

    Vector2Int pixelToAxial(Vector2 pixel, float size)
    {
        var q = (2f / 3f * pixel.x) / size;
        var r = (-1f / 3f * pixel.x + Mathf.Sqrt(3) / 3f * pixel.y) / size;
        return new Vector2Int(Mathf.RoundToInt(q), Mathf.RoundToInt(r));
    }

    HexTile pixelToHex(Vector2 pixel, float size)
    {
        Vector2Int axial = pixelToAxial(pixel, size);
        hexTiles.TryGetValue(axial, out HexTile tile);
        return tile;
    }

    public HexTile GetTileAtWorldPos(Vector3 worldPos)
    {
        Vector2Int axial = pixelToAxial(worldPos, hexSize);
        hexTiles.TryGetValue(axial, out HexTile tile);
        return tile;
    }

    public List<HexTile> GetNeighbors(HexTile tile)
    {
        List<HexTile> neighbors = new List<HexTile>();
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(1, 0),  new Vector2Int(-1, 0),
            new Vector2Int(0, 1),  new Vector2Int(0, -1),
            new Vector2Int(1, -1), new Vector2Int(-1, 1)
        };

        foreach (var dir in directions)
        {
            Vector2Int neighborAxial = tile.hex.axial + dir;
            if (hexTiles.TryGetValue(neighborAxial, out HexTile neighbor))
                neighbors.Add(neighbor);
        }
        return neighbors;
    }

    #endregion

    // -------------------------------------------------------------------------
    // Other Hex Functions
    // -------------------------------------------------------------------------

    #region OtherHexFunctions

    List<Hex> movementRange(Hex center, int range)
    {
        List<Hex> results = new List<Hex>();
        for (int q = -range; q <= range; q++)
        {
            int rMin = Mathf.Max(-range, -q - range);
            int rMax = Mathf.Min(range, -q + range);
            for (int r = rMin; r <= rMax; r++)
            {
                results.Add(axialAdd(center, new Hex(new Vector2Int(q, r))));
            }
        }
        return results;
    }

    public int GetDistance(HexTile a, HexTile b)
    {
        Vector2Int diff = a.hex.axial - b.hex.axial;
        return (Mathf.Abs(diff.x) + Mathf.Abs(diff.y) + Mathf.Abs(diff.x + diff.y)) / 2;
    }
    #endregion
}