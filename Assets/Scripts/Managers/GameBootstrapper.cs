using UnityEngine;
using UnityEngine.InputSystem;

public class GameBootstrapper : MonoBehaviour
{
    [SerializeField]
    private PlayerScript player;
    [SerializeField]
    private PlayerInput playerInput;

    void Awake()
    {
        var inputHandler = new PlayerInputHandler(playerInput);
        
        GameServices.Register(
            player.GetComponent<Rigidbody2D>(),
            playerInput,
            inputHandler
        );
    }

    void OnDestroy()
    {
        GameServices.Clear();
    }
}