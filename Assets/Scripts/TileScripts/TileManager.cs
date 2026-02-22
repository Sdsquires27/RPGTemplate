using UnityEngine;

/// <summary>
/// Main tile manager that extends HexGrid with additional game logic.
/// </summary>
public class TileManager : HexGrid
{
    [Header("Generation Settings")]
    [SerializeField]
    private bool autoGenerateOnStart = true;
    [SerializeField]
    private int gridWidth = 10;
    [SerializeField]
    private int gridHeight = 10;

    protected void Awake()
    {
        // Register this instance in GameServices
        GameServices.RegisterTileManager(this);
    }

    private void Start()
    {
        if (autoGenerateOnStart)
        {
            GenerateRectangularGrid(gridWidth, gridHeight, "Grass");
            Debug.Log($"Hex grid generated with {TileCount} tiles");
        }
    }

    private void OnDestroy()
    {
        GameServices.ClearTileManager();
    }
}

