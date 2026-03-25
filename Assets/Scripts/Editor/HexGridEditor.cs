// Assets/Editor/HexGridEditor.cs
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomEditor(typeof(HexGrid))]
public class HexGridEditor : Editor
{
    private HexGrid hexGrid;
    private bool isPainting = false;
    private int brushRadius = 1; // 1 = single hex

    private void OnEnable()
    {
        hexGrid = (HexGrid)target;
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();

        // ------------------------------------------------------------------
        // Tile palette
        // ------------------------------------------------------------------
        if (hexGrid.tilePalette != null && hexGrid.tilePalette.Length > 0)
        {
            EditorGUILayout.LabelField("Tile Palette", EditorStyles.boldLabel);

            for (int i = 0; i < hexGrid.tilePalette.Length; i++)
            {
                if (hexGrid.tilePalette[i] == null) continue;

                bool isSelected = hexGrid.selectedTileIndex == i;
                Color prevColor = GUI.backgroundColor;
                GUI.backgroundColor = isSelected ? Color.cyan : Color.white;

                if (GUILayout.Button(hexGrid.tilePalette[i].name))
                    hexGrid.selectedTileIndex = i;

                GUI.backgroundColor = prevColor;
            }

            EditorGUILayout.Space();
        }

        // ------------------------------------------------------------------
        // Brush size
        // ------------------------------------------------------------------
        EditorGUILayout.LabelField("Brush", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Radius", GUILayout.Width(52));

        // "-" button
        GUI.enabled = brushRadius > 1;
        if (GUILayout.Button("-", GUILayout.Width(24)))
        {
            brushRadius--;
            SceneView.RepaintAll();
        }
        GUI.enabled = true;

        // Current radius label
        GUILayout.Label(brushRadius.ToString(), GUILayout.Width(24));

        // "+" button
        if (GUILayout.Button("+", GUILayout.Width(24)))
        {
            brushRadius++;
            SceneView.RepaintAll();
        }

        // Slider (1-6)
        int newRadius = EditorGUILayout.IntSlider(brushRadius, 1, 6);
        if (newRadius != brushRadius)
        {
            brushRadius = newRadius;
            SceneView.RepaintAll();
        }

        EditorGUILayout.EndHorizontal();

        // Human-readable hex count hint
        int hexCount = HexesInRadius(brushRadius);
        EditorGUILayout.HelpBox(
            brushRadius == 1
                ? "Single hex"
                : $"Radius {brushRadius} — affects {hexCount} hexes",
            MessageType.None);

        EditorGUILayout.Space();

        // ------------------------------------------------------------------
        // Paint / Stop button
        // ------------------------------------------------------------------
        Color btnColor = GUI.backgroundColor;
        GUI.backgroundColor = isPainting ? Color.red : Color.green;
        if (GUILayout.Button(isPainting ? "Stop Painting" : "Start Painting"))
        {
            isPainting = !isPainting;
            SceneView.RepaintAll();
        }
        GUI.backgroundColor = btnColor;

        EditorGUILayout.Space();

        if (GUILayout.Button("Clear All Hexes"))
        {
            if (EditorUtility.DisplayDialog("Clear Grid", "Delete all hex tiles?", "Yes", "Cancel"))
                hexGrid.ClearAll();
        }
    }

    private void OnSceneGUI()
    {
        if (!isPainting) return;
        hexGrid.CalculateHexSize();

        // Prevent scene from deselecting HexGrid when clicking
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        Event e = Event.current;

        if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
        {
            Vector2Int center = GetAxialUnderMouse();
            List<Vector2Int> cells = GetBrushCells(center, brushRadius);

            if (e.button == 0) // Left click = paint
            {
                Undo.SetCurrentGroupName("Paint Hex Brush");
                int group = Undo.GetCurrentGroup();

                foreach (Vector2Int axial in cells)
                    hexGrid.EditorSpawnHex(axial);

                Undo.CollapseUndoOperations(group);
                e.Use();
            }
            else if (e.button == 1) // Right click = erase
            {
                Undo.SetCurrentGroupName("Erase Hex Brush");
                int group = Undo.GetCurrentGroup();

                foreach (Vector2Int axial in cells)
                    hexGrid.EditorRemoveHex(axial);

                Undo.CollapseUndoOperations(group);
                e.Use();
            }
        }

        // ------------------------------------------------------------------
        // Brush preview — highlight all cells in current brush
        // ------------------------------------------------------------------
        Vector2Int hoveredCenter = GetAxialUnderMouse();
        List<Vector2Int> previewCells = GetBrushCells(hoveredCenter, brushRadius);

        foreach (Vector2Int axial in previewCells)
        {
            Vector2 worldPos = hexGrid.HexToPixelPublic(axial);
            Handles.color = new Color(1f, 1f, 0f, 0.25f);
            Handles.DrawSolidDisc(new Vector3(worldPos.x, worldPos.y, 0f), Vector3.forward, hexGrid.hexSize * 0.5f);
            Handles.color = new Color(1f, 1f, 0f, 0.7f);
            Handles.DrawWireDisc(new Vector3(worldPos.x, worldPos.y, 0f), Vector3.forward, hexGrid.hexSize * 0.5f);
        }

        SceneView.RepaintAll();
    }

    // -----------------------------------------------------------------------
    // Helpers
    // -----------------------------------------------------------------------

    /// <summary>
    /// Returns all axial coordinates within <paramref name="radius"/> rings
    /// of <paramref name="center"/> (inclusive). Radius 1 = just the center.
    /// </summary>
    private List<Vector2Int> GetBrushCells(Vector2Int center, int radius)
    {
        List<Vector2Int> cells = new List<Vector2Int>();
        int r = radius - 1; // convert "radius 1 = single hex" to ring count

        for (int q = -r; q <= r; q++)
        {
            int rMin = Mathf.Max(-r, -q - r);
            int rMax = Mathf.Min(r, -q + r);
            for (int s = rMin; s <= rMax; s++)
                cells.Add(new Vector2Int(center.x + q, center.y + s));
        }

        return cells;
    }

    /// <summary>Returns total hex count for a given radius (1-based).</summary>
    private int HexesInRadius(int radius)
    {
        int r = radius - 1;
        return 3 * r * r + 3 * r + 1;
    }

    private Vector2Int GetAxialUnderMouse()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Vector2 worldPos = ray.origin;
        return hexGrid.PixelToAxialPublic(worldPos);
    }
}