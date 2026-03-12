// Assets/Editor/HexGridEditor.cs
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexGrid))]
public class HexGridEditor : Editor
{
    private HexGrid hexGrid;
    private bool isPainting = false;

    private void OnEnable()
    {
        hexGrid = (HexGrid)target;
    }

public override void OnInspectorGUI()
{
    DrawDefaultInspector();

    EditorGUILayout.Space();

    // Tile palette selector
    if (hexGrid.tilePalette != null && hexGrid.tilePalette.Length > 0)
    {
        EditorGUILayout.LabelField("Tile Palette", EditorStyles.boldLabel);

        // Draw a clickable button for each tile in the palette
        for (int i = 0; i < hexGrid.tilePalette.Length; i++)
        {
            if (hexGrid.tilePalette[i] == null) continue;

            bool isSelected = hexGrid.selectedTileIndex == i;
            Color prevColor = GUI.backgroundColor;
            GUI.backgroundColor = isSelected ? Color.cyan : Color.white;

            // Show prefab name as button, highlight selected
            if (GUILayout.Button(hexGrid.tilePalette[i].name))
                hexGrid.selectedTileIndex = i;

            GUI.backgroundColor = prevColor;
        }

        EditorGUILayout.Space();
    }

    // Paint / Stop button
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
        hexGrid.CalculateHexSize(); // ensure sizes are initialized in editor

        // Prevent scene from deselecting HexGrid when clicking
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        Event e = Event.current;

        if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag)
        {
            Vector2Int axial = GetAxialUnderMouse();

            if (e.button == 0) // Left click = place
            {
                hexGrid.EditorSpawnHex(axial);
                e.Use();
            }
            else if (e.button == 1) // Right click = erase
            {
                hexGrid.EditorRemoveHex(axial);
                e.Use();
            }
        }

        // Highlight hovered cell
        Vector2Int hovered = GetAxialUnderMouse();
        Vector2 worldPos = hexGrid.HexToPixelPublic(hovered);
        Handles.color = new Color(1f, 1f, 0f, 0.3f);
        Handles.DrawSolidDisc(new Vector3(worldPos.x, worldPos.y, 0), Vector3.forward, hexGrid.hexSize * 0.5f);

        SceneView.RepaintAll();
    }

    private Vector2Int GetAxialUnderMouse()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Vector2 worldPos = ray.origin;
        return hexGrid.PixelToAxialPublic(worldPos);
    }
}