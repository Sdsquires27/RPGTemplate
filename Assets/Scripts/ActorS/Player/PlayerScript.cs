// Assets/Scripts/Player/PlayerScript.cs
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : ActorScript
{
    [Header("Controls")]
    [SerializeField] private ControlScheme controlScheme = ControlScheme.MouseClick;

    // WASD / Joystick
    private float inputCooldown = 0.2f;   // prevents too-rapid movement
    private float lastInputTime = 0f;

    // Click to move
    private List<HexTile> currentPath = new List<HexTile>();
    private Coroutine pathCoroutine;

    // Flat-top hex directions mapped to angles
    // Each entry is (axial direction, angle range min, angle range max)
    private static readonly (Vector2Int dir, float min, float max)[] directionMap =
    {
        (new Vector2Int( 1,  0),   -30f,  30f),  // Right
        (new Vector2Int( 1, -1),    30f,  90f),  // Upper Right
        (new Vector2Int( 0, -1),    90f, 150f),  // Upper Left
        (new Vector2Int(-1,  0),   150f, 180f),  // Left (and -180 to -150)
        (new Vector2Int(-1,  0),  -180f,-150f),  // Left (wrapped)
        (new Vector2Int(-1,  1),  -150f, -90f),  // Lower Left
        (new Vector2Int( 0,  1),   -90f, -30f),  // Lower Right
    };

    protected override void HandleMovement()
    {
        switch (controlScheme)
        {
            case ControlScheme.MouseClick: HandleMouseClick(); break;
            case ControlScheme.WASD:       HandleWASD();       break;
            case ControlScheme.Joystick:   HandleJoystick();   break;
        }
    }

    // -------------------------------------------------------------------------
    // Mouse Click
    // -------------------------------------------------------------------------

    void HandleMouseClick()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        HexTile clicked = hexGrid.GetTileAtWorldPos(worldPos);
        if (clicked == null || !clicked.isWalkable) return;

        // Cancel existing path
        if (pathCoroutine != null) StopCoroutine(pathCoroutine);
        currentPath = Pathfinder.FindPath(hexGrid, currentTile, clicked);
        pathCoroutine = StartCoroutine(FollowPath());
    }

    IEnumerator FollowPath()
    {
        foreach (HexTile tile in currentPath)
        {
            MoveToTile(tile);
            yield return new WaitUntil(() => !isMoving);
        }
        currentPath.Clear();
    }

    // -------------------------------------------------------------------------
    // WASD
    // -------------------------------------------------------------------------

    void HandleWASD()
    {
        if (Time.time - lastInputTime < inputCooldown) return;

        Vector2 input = Vector2.zero;

        // Map WASD to a directional vector
        if (Input.GetKey(KeyCode.W)) input.y += 1;
        if (Input.GetKey(KeyCode.S)) input.y -= 1;
        if (Input.GetKey(KeyCode.D)) input.x += 1;
        if (Input.GetKey(KeyCode.A)) input.x -= 1;

        if (input == Vector2.zero) return;

        MoveInDirection(input);
        lastInputTime = Time.time;
    }

    // -------------------------------------------------------------------------
    // Joystick
    // -------------------------------------------------------------------------

    void HandleJoystick()
    {
        if (Time.time - lastInputTime < inputCooldown) return;

        Vector2 input = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );

        // Dead zone
        if (input.magnitude < 0.5f) return;

        MoveInDirection(input);
        lastInputTime = Time.time;
    }

    // -------------------------------------------------------------------------
    // Shared directional movement (used by both WASD and Joystick)
    // -------------------------------------------------------------------------

    void MoveInDirection(Vector2 input)
    {
        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        Vector2Int bestDir = GetHexDirectionFromAngle(angle);
        Vector2Int targetAxial = currentTile.hex.axial + bestDir;

        if (hexGrid.hexTiles.TryGetValue(targetAxial, out HexTile tile) && tile.isWalkable)
            MoveToTile(tile);
    }

    Vector2Int GetHexDirectionFromAngle(float angle)
    {
        foreach (var (dir, min, max) in directionMap)
            if (angle >= min && angle < max)
                return dir;

        // Fallback - shouldn't normally reach here
        return new Vector2Int(1, 0);
    }

    // -------------------------------------------------------------------------
    // Switching schemes at runtime
    // -------------------------------------------------------------------------

    public void SetControlScheme(ControlScheme scheme)
    {
        controlScheme = scheme;

        // Cancel any in-progress path when switching
        if (pathCoroutine != null) StopCoroutine(pathCoroutine);
        currentPath.Clear();
    }

    protected override void HandleActions()
    {
        // Player actions e.g. attack, interact
    }
}