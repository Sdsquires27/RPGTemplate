using UnityEngine;
using UnityEngine.InputSystem;

public static class GameServices
{
    private static Rigidbody2D playerRigidbody;
    private static PlayerInput playerInput;
    private static IInputHandler inputHandler;
    private static Inventory playerInventory;
    
    public static void Register(Rigidbody2D rb, PlayerInput input, IInputHandler handler, Inventory inventory)
    {
        playerRigidbody = rb;
        playerInput = input;
        inputHandler = handler;
        playerInventory = inventory;
    }
    
    public static Rigidbody2D GetPlayerRigidbody() => playerRigidbody;
    public static PlayerInput GetPlayerInput() => playerInput;
    public static IInputHandler GetInputHandler() => inputHandler;
    public static Inventory GetPlayerInventory() => playerInventory;
    
    public static void Clear()
    {
        playerRigidbody = null;
        playerInput = null;
        inputHandler = null;
        playerInventory = null;
    }
}