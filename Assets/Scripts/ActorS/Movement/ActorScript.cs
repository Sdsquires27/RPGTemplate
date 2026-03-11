using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class ActorScript : MonoBehaviour
{
    #region Variables
    [Header("Actor Settings")]
    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected int health;
    [SerializeField] protected int maxHealth = 100;


    [Header("Hex Grid")]
    protected HexGrid hexGrid;
    protected HexTile currentTile;
    protected HexTile targetTile;
    protected bool isMoving = false;

    protected Rigidbody2D rb;
    #endregion

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        hexGrid = FindFirstObjectByType<HexGrid>();

        // Snap to nearest tile on spawn
        currentTile = hexGrid.GetTileAtWorldPos(transform.position);
        if (currentTile != null)
            transform.position = currentTile.transform.position;
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
        StartCoroutine(SmoothMove(tile));
    }

    IEnumerator SmoothMove(HexTile tile)
    {
        isMoving = true;
        targetTile = tile;

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
        isMoving = false;
    }

    // Get walkable neighbors of current tile
    protected List<HexTile> GetNeighbors()
    {
        return hexGrid.GetNeighbors(currentTile);
    }

    protected abstract void HandleMovement();
    protected abstract void HandleActions();

    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Health: {health}");
    }
}