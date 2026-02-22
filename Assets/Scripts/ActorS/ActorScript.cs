using UnityEngine;

public abstract class ActorScript : MonoBehaviour
{
    #region Variables
    [Header("Actor Settings")]
    [SerializeField]
    protected float moveSpeed = 5f;
    [SerializeField]
    protected int health;
    
    protected Rigidbody2D rb;

    // Hex Grid Integration
    protected HexTile currentTile;
    protected HexCoordinate hexPosition;

    #endregion
    
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        InitializeHexPosition();
    }

    protected virtual void Update()
    {
        UpdateHexPosition();
        HandleMovement();
        HandleActions();
    }

    // Abstract methods - child classes must implement
    protected abstract void HandleMovement();
    protected abstract void HandleActions();
    
    // Virtual methods - can be overridden
    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Health: {health}");
    }

    /// <summary>
    /// Initialize actor's hex tile position based on current world position.
    /// </summary>
    protected virtual void InitializeHexPosition()
    {
        TileManager tileManager = GameServices.GetTileManager();
        if (tileManager == null)
        {
            Debug.LogWarning($"{gameObject.name}: TileManager not registered in GameServices");
            return;
        }

        hexPosition = tileManager.WorldToHexPosition(transform.position);
        currentTile = tileManager.GetOrCreateTile(hexPosition);
        
        if (currentTile != null)
        {
            currentTile.RegisterActor(this);
            Debug.Log($"{gameObject.name} initialized at hex tile {hexPosition}");
        }
    }

    /// <summary>
    /// Update hex position if actor has moved to a new tile.
    /// </summary>
    protected virtual void UpdateHexPosition()
    {
        TileManager tileManager = GameServices.GetTileManager();
        if (tileManager == null)
            return;

        HexCoordinate newHexPos = tileManager.WorldToHexPosition(transform.position);
        
        if (!newHexPos.Equals(hexPosition))
        {
            // Unregister from old tile
            if (currentTile != null)
                currentTile.UnregisterActor(this);

            // Register with new tile
            hexPosition = newHexPos;
            currentTile = tileManager.GetOrCreateTile(hexPosition);
            
            if (currentTile != null)
            {
                currentTile.RegisterActor(this);
            }
        }
    }

    /// <summary>
    /// Set actor position on a specific hex tile.
    /// </summary>
    public virtual void SetHexTile(HexTile tile)
    {
        if (tile == null)
            return;

        // Unregister from old tile
        if (currentTile != null)
            currentTile.UnregisterActor(this);

        // Move to new tile
        currentTile = tile;
        hexPosition = tile.coordinate;
        transform.position = tile.WorldPosition;
        tile.RegisterActor(this);
    }

    /// <summary>
    /// Get the current hex tile this actor is on.
    /// </summary>
    public HexTile GetCurrentHexTile()
    {
        return currentTile;
    }

    /// <summary>
    /// Get the current hex coordinate.
    /// </summary>
    public HexCoordinate GetHexPosition()
    {
        return hexPosition;
    }
}