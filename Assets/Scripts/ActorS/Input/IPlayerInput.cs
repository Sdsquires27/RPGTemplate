using UnityEngine;

public interface IPlayerInput
{
    void OnMove(Vector2 direction);
    void OnLook(Vector2 direction);
    void OnAttack();
    void OnInteract();
    void OnCrouch();
    void OnJump();
    

}
