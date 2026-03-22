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

    [SerializeField] private MenuPanel menuPanel;
    [SerializeField] private SettingsPanel settingsPanel;

    // Cooldown to prevent too-rapid movement on held input
    [SerializeField] private float inputCooldown = 0.2f;
    private float lastInputTime;
    [SerializeField] private Inventory inventory;


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
        Debug.Log("Vector 2 normalized x: " + x + "y: " + y);

        Vector2Int axialOffset;

        if      (x ==  0 && y >  0) axialOffset = new Vector2Int( 0, -1);  // N  (W key)
        else if (x >  0 && y > 0) axialOffset = new Vector2Int(1, -1);  // NE (E key)
        else if (x >  0 && y <  0)  axialOffset = new Vector2Int(1,  0);  // SE (D key)
        else if (x ==  0 && y <  0) axialOffset = new Vector2Int( 0,  1);  // S  (S key)
        else if (x <  0 && y <  0)  axialOffset = new Vector2Int(-1,  1);  // SW (A key)
        else if (x <  0 && y >  0)  axialOffset = new Vector2Int( -1, 0);  // NW (Q key)
        else return;

        Vector2Int targetAxial = currentTile.hex.axial + axialOffset;
        if (hexGrid.hexTiles.TryGetValue(targetAxial, out HexTile tile) && tile.isWalkable)
            MoveToTile(tile);
    }

    void OpenMenu()
    {
        menuPanel.ClearButtons();
        menuPanel.AddButton("Resume",   () => UIManager.Instance.CloseTopPanel());
        menuPanel.AddButton("Settings", () => UIManager.Instance.OpenPanel(settingsPanel));
        // menuPanel.AddButton("Inventory",() => UIManager.Instance.OpenPanel(inventoryPanel));
        menuPanel.AddButton("Quit",     () => Application.Quit());
        UIManager.Instance.OpenPanel(menuPanel);
    }
    protected override void HandleActions()
    {
        if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
            OpenMenu();

        if (UnityEngine.InputSystem.Keyboard.current.fKey.wasPressedThisFrame)
            TryInteract();
    }

void TryInteract()
{
    Vector2Int facingAxial = currentTile.hex.axial + facingDirection;
    if (!hexGrid.hexTiles.TryGetValue(facingAxial, out HexTile facingTile)) return;

    // Check for dialogue trigger first
    DialogueTrigger trigger = facingTile.GetComponentInChildren<DialogueTrigger>();
    if (trigger != null)
    {
        trigger.Trigger();
        return;
    }

    // Otherwise handle items as before
    if (!inventory.IsEmpty)
    {
        if (facingTile.occupiedBy == null && facingTile.isWalkable)
            inventory.Drop(facingTile);
    }
    else
    {
        if (facingTile.occupiedBy != null)
            inventory.PickUp(facingTile.occupiedBy);
    }
}

}