using UnityEngine;

public interface IInputHandler
{
    Vector2 GetMoveDirection();
    Vector2 GetLookDirection();
    bool IsJumpPressed();
    bool IsAttackPressed();
    bool IsCrouchPressed();
    bool IsInteractPressed();
}

