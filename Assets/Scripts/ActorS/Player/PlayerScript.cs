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
        if (isMoving) return;

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

    // Flat-top hex direction mapping
    void MoveInDirection(Vector2 input)
    {
        if (currentTile == null) return;

        Vector2 normalized = input.normalized;
        float x = Mathf.Round(normalized.x / 0.866f);
        float y = Mathf.Round(normalized.y);

        Vector2Int axialOffset;

        if      (x ==  0 && y >  0) axialOffset = new Vector2Int( 0, -1);  // N  (W key)
        else if (x >  0 && y > 0) axialOffset = new Vector2Int(-1, 0);  // NE (E key)
        else if (x >  0 && y <  0)  axialOffset = new Vector2Int(-1,  1);  // SE (D key)
        else if (x ==  0 && y <  0) axialOffset = new Vector2Int( 0,  1);  // S  (S key)
        else if (x <  0 && y <  0)  {axialOffset = new Vector2Int( 1,  0);}  // SW (A key)
        else if (x <  0 && y >  0)  axialOffset = new Vector2Int( 1, -1);  // NW (Q key)
        else return;

        Vector2Int targetAxial = currentTile.hex.axial + axialOffset;
        if (hexGrid.hexTiles.TryGetValue(targetAxial, out HexTile tile) && tile.isWalkable)
            MoveToTile(tile);
    }

    protected override void HandleActions()
    {
        // TODO: hook up Action1 etc.
    }
}