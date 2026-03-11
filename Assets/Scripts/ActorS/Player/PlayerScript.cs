using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlayerScript : ActorScript
{
    [Header("Controls")]
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction clickAction;

    // Cooldown to prevent too-rapid movement on held input
    [SerializeField] private float inputCooldown = 0.2f;
    private float lastInputTime;

    private List<HexTile> currentPath = new List<HexTile>();
    private Coroutine pathCoroutine;

    protected override void Start()
    {
        base.Start();

        playerInput = GetComponent<PlayerInput>();
        moveAction = playerInput.actions.FindActionMap("Player").FindAction("Move");
        clickAction = playerInput.actions.FindActionMap("Player").FindAction("Click");

        // Enable the action map
        playerInput.actions.FindActionMap("Player").Enable();

        if (playerInput == null) Debug.LogError("PlayerInput component not found on " + gameObject.name);
        else Debug.Log("PlayerInput found, actions: " + playerInput.actions);
    }

    protected override void HandleMovement()
    {
        HandleDirectionalInput();
        HandleClickInput();
    }

    void HandleDirectionalInput()
    {
        if (Time.time - lastInputTime < inputCooldown) return;

        Vector2 input = moveAction.ReadValue<Vector2>();
        if (input.magnitude < 0.5f) return;

        // Invert Y to match hex grid orientation
        input.y = -input.y;

        MoveInDirection(input);
        lastInputTime = Time.time;
    }

    void HandleClickInput()
    {
        if (!clickAction.WasPressedThisFrame()) return;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        HexTile clicked = hexGrid.GetTileAtWorldPos(worldPos);
        if (clicked == null || !clicked.isWalkable) return;

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

    void MoveInDirection(Vector2 input)
    {
        if (currentTile == null)
        {
            Debug.LogError("currentTile is null — player not snapped to grid yet");
            return;
        }
        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
        Vector2Int bestDir = GetHexDirectionFromAngle(angle);
        Vector2Int targetAxial = currentTile.hex.axial + bestDir;

        if (hexGrid.hexTiles.TryGetValue(targetAxial, out HexTile tile) && tile.isWalkable)
            MoveToTile(tile);
    }

    // Flat-top hex direction mapping
    private static readonly (Vector2Int dir, float min, float max)[] directionMap =
    {
        (new Vector2Int( 1,  0),   -30f,  30f),
        (new Vector2Int( 1, -1),    30f,  90f),
        (new Vector2Int( 0, -1),    90f, 150f),
        (new Vector2Int(-1,  0),   150f, 180f),
        (new Vector2Int(-1,  0),  -180f,-150f),
        (new Vector2Int(-1,  1),  -150f, -90f),
        (new Vector2Int( 0,  1),   -90f, -30f),
    };

    Vector2Int GetHexDirectionFromAngle(float angle)
    {
        foreach (var (dir, min, max) in directionMap)
            if (angle >= min && angle < max)
                return dir;
        return new Vector2Int(1, 0);
    }

    protected override void HandleActions()
    {
        // TODO: hook up Action1 etc.
    }
}