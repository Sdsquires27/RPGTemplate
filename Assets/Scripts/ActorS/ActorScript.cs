using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ActorScript : MonoBehaviour
{
    #region Variables
    [Header("Actor Settings")]
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float rotationOffset = 0f; // Degrees to offset rotation
    [SerializeField] protected int health;
    [SerializeField] protected int maxHealth = 100;


    [Header("Hex Grid")]
    protected HexGrid hexGrid;
    protected HexTile currentTile;
    protected HexTile targetTile;
    protected bool isMoving = false;


    protected Rigidbody2D rb;
    protected Vector2Int facingDirection = new Vector2Int(0, 1); // default facing N
    #endregion

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        hexGrid = FindFirstObjectByType<HexGrid>();

        currentTile = hexGrid.GetNearestTile(transform.position);
        if (currentTile != null)
        {
            transform.position = currentTile.transform.position;
            currentTile.SetActor(this);
        }

        RotateTowards(facingDirection);
    }
    protected virtual void Update()
    {
        if (!isMoving)
        {
            HandleMovement();
            HandleActions();
        }
    }

    // Move to an adjacent tile
    public virtual void MoveToTile(HexTile tile)
    {
        if (tile == null || !tile.isWalkable || isMoving) return;
        facingDirection = tile.hex.axial - currentTile.hex.axial;
        RotateTowards(facingDirection);
        StartCoroutine(SmoothMove(tile));
    }

    IEnumerator SmoothMove(HexTile tile)
    {
        isMoving = true;
        targetTile = tile;

        // Unregister from current tile
        currentTile?.ClearActor();

        Vector3 startPos = transform.position;
        Vector3 endPos = tile.transform.position;
        float elapsed = 0f;
        float duration = 1f / moveSpeed;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = endPos;
        currentTile = tile;
        targetTile = null;

        // Register on new tile
        currentTile.SetActor(this);

        isMoving = false;
    }

    // Get walkable neighbors of current tile
    protected List<HexTile> GetNeighbors()
    {
        return hexGrid.GetNeighbors(currentTile);
    }

    // Rotate the actor towards the given direction
    protected void RotateTowards(Vector2Int direction)
    {
        float angle = 0f;
        if (direction == new Vector2Int(1, 0)) angle = 0f;      // East
        else if (direction == new Vector2Int(0, 1)) angle = 60f;   // Northeast
        else if (direction == new Vector2Int(-1, 1)) angle = 120f; // Northwest
        else if (direction == new Vector2Int(-1, 0)) angle = 180f; // West
        else if (direction == new Vector2Int(0, -1)) angle = 240f; // Southwest
        else if (direction == new Vector2Int(1, -1)) angle = 300f; // Southeast

        transform.rotation = Quaternion.Euler(0, 0, angle + rotationOffset);
    }

    protected abstract void HandleMovement();
    protected abstract void HandleActions();

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Health: {health}");
    }
    protected virtual void OnDestroy()
    {
        currentTile?.ClearActor();
    }
}