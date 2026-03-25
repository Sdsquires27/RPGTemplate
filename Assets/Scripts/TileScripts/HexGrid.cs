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

    // -------------------------------------------------------------------------
    // Lifecycle
    // -------------------------------------------------------------------------

    void Start()
    {
        hexTiles = new Dictionary<Vector2Int, HexTile>();

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
                SpawnHex(new Vector2Int(q, r));
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
    /// When hexes were painted in the editor, register them into the
    /// runtime dictionary by scanning child transforms.
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
    /// <summary>
    /// Rebuilds the hexTiles dictionary from the live scene hierarchy.
    /// Must be called before every editor write operation because Dictionary
    /// is not serialized by Unity and will be null/stale after any domain
    /// reload, recompile, or enter/exit of play mode.
    /// </summary>
    private void RebuildEditorDictionary()
    {
        hexTiles = new Dictionary<Vector2Int, HexTile>();
        foreach (Transform child in transform)
        {
            HexTile tile = child.GetComponent<HexTile>();
            if (tile != null)
                hexTiles[tile.hex.axial] = tile;
        }
    }

    public void EditorSpawnHex(Vector2Int axial)
    {
        // Always sync from scene state first — dictionary is not serialized
        // and may be null or stale after a domain reload.
        RebuildEditorDictionary();

        if (hexTiles.ContainsKey(axial)) return;

        GameObject prefab = (tilePalette != null && tilePalette.Length > 0)
            ? tilePalette[selectedTileIndex]
            : hexPrefab;

        if (prefab == null) return;

        Vector2 worldPos = hexToPixel(axial, hexSize);
        GameObject go = (GameObject)UnityEditor.PrefabUtility.InstantiatePrefab(prefab, transform);
        go.transform.position = new Vector3(worldPos.x, worldPos.y, 0f);

        HexTile tile = go.GetComponent<HexTile>();
        tile.Init(axial);
        hexTiles[axial] = tile;

        UnityEditor.Undo.RegisterCreatedObjectUndo(go, "Paint Hex");
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }

    public void EditorRemoveHex(Vector2Int axial)
    {
        // Always sync from scene state first — dictionary is not serialized
        // and may be null or stale after a domain reload.
        RebuildEditorDictionary();

        if (!hexTiles.TryGetValue(axial, out HexTile tile)) return;

        hexTiles.Remove(axial);
        UnityEditor.Undo.DestroyObjectImmediate(tile.gameObject);
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
    }

    public void ClearAll()
    {
        RebuildEditorDictionary();

        foreach (var tile in hexTiles.Values)
            if (tile != null)
                UnityEditor.Undo.DestroyObjectImmediate(tile.gameObject);

        hexTiles.Clear();
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
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
            results[i] = hexLerp(a, b, 1f / N * i);
        return results;
    }

    #endregion

    // -------------------------------------------------------------------------
    // Pixel <-> Hex Conversions
    // -------------------------------------------------------------------------

    #region PixelHexConversions

    public void CalculateHexSize()
    {
        if (hexPrefab == null) return;
        SpriteRenderer sr = hexPrefab.GetComponent<SpriteRenderer>();
        if (sr != null && sr.sprite != null)
            hexSize = sr.sprite.bounds.size.x / Mathf.Sqrt(3f);
    }

    Vector2 hexToPixel(Vector2Int axial, float size)
    {
        float x = size * (3f / 2f * axial.x);
        float y = size * (Mathf.Sqrt(3f) / 2f * axial.x + Mathf.Sqrt(3f) * axial.y);
        return new Vector2(x, y);
    }

    // Kept for internal Hex struct usage
    Vector2 hexToPixel(Hex hex, float size)
    {
        float x = (Mathf.Sqrt(3f) * hex.q + Mathf.Sqrt(3f) / 2f * hex.r) * size;
        float y = (3f / 2f * hex.r) * size;
        return new Vector2(x, y);
    }

    Vector2Int pixelToAxial(Vector2 pixel, float size)
    {
        float q = (2f / 3f * pixel.x) / size;
        float r = (-1f / 3f * pixel.x + Mathf.Sqrt(3f) / 3f * pixel.y) / size;
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
            new Vector2Int( 1,  0), new Vector2Int(-1,  0),
            new Vector2Int( 0,  1), new Vector2Int( 0, -1),
            new Vector2Int( 1, -1), new Vector2Int(-1,  1)
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

    public HexTile GetNearestTile(Vector3 worldPos)
    {
        HexTile nearest = null;
        float nearestDist = float.MaxValue;

        foreach (var tile in hexTiles.Values)
        {
            float dist = Vector3.Distance(worldPos, tile.transform.position);
            if (dist < nearestDist)
            {
                nearestDist = dist;
                nearest = tile;
            }
        }
        return nearest;
    }

    List<Hex> movementRange(Hex center, int range)
    {
        List<Hex> results = new List<Hex>();
        for (int q = -range; q <= range; q++)
        {
            int rMin = Mathf.Max(-range, -q - range);
            int rMax = Mathf.Min(range, -q + range);
            for (int r = rMin; r <= rMax; r++)
                results.Add(axialAdd(center, new Hex(new Vector2Int(q, r))));
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
