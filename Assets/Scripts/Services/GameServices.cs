using UnityEngine;
using UnityEngine.InputSystem;

public static class GameServices
{
    private static Rigidbody2D playerRigidbody;
    private static PlayerInput playerInput;
    private static IInputHandler inputHandler;
    private static TileManager tileManager;
    
    public static void Register(Rigidbody2D rb, PlayerInput input, IInputHandler handler)
    {
        playerRigidbody = rb;
        playerInput = input;
        inputHandler = handler;
    }
    
    public static Rigidbody2D GetPlayerRigidbody() => playerRigidbody;
    public static PlayerInput GetPlayerInput() => playerInput;
    public static IInputHandler GetInputHandler() => inputHandler;

    /// <summary>
    /// Register the TileManager (called by TileManager.Awake).
    /// </summary>
    public static void RegisterTileManager(TileManager manager)
    {
        tileManager = manager;
    }

    /// <summary>
    /// Get the TileManager instance.
    /// </summary>
    public static TileManager GetTileManager() => tileManager;

    /// <summary>
    /// Clear TileManager reference (called by TileManager.OnDestroy).
    /// </summary>
    public static void ClearTileManager()
    {
        tileManager = null;
    }
    
    public static void Clear()
    {
        playerRigidbody = null;
        playerInput = null;
        inputHandler = null;
        tileManager = null;
    }
}