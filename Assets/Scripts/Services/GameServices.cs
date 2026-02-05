using UnityEngine;
using UnityEngine.InputSystem;

public static class GameServices
{
    private static Rigidbody2D playerRigidbody;
    private static PlayerInput playerInput;
    private static IInputHandler inputHandler;
    
    public static void Register(Rigidbody2D rb, PlayerInput input, IInputHandler handler)
    {
        playerRigidbody = rb;
        playerInput = input;
        inputHandler = handler;
    }
    
    public static Rigidbody2D GetPlayerRigidbody() => playerRigidbody;
    public static PlayerInput GetPlayerInput() => playerInput;
    public static IInputHandler GetInputHandler() => inputHandler;
    
    public static void Clear()
    {
        playerRigidbody = null;
        playerInput = null;
        inputHandler = null;
    }
}