
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }

    public bool IsDefending { get; private set; }

    private bool attackPressed;
    private bool dashPressed;
    private bool interactPressed;
    private bool pausePressed;

    public void OnMove(InputValue value)
    {
        MoveInput = value.Get<Vector2>();
    }

    public void OnAttack(InputValue value)
    {
        if (value.isPressed)
            attackPressed = true;
    }

    public void OnDefend(InputValue value)
    {
        IsDefending = value.isPressed;
    }

    public void OnDash(InputValue value)
    {
        if (value.isPressed)
            dashPressed = true;
    }

    public void OnInteract(InputValue value)
    {
        if (value.isPressed)
            interactPressed = true;
    }

    public void OnPause(InputValue value)
    {
        if (value.isPressed)
            pausePressed = true;
    }

    public bool ConsumeAttack()
    {
        bool pressed = attackPressed;
        attackPressed = false;
        return pressed;
    }

    public bool ConsumeDash()
    {
        bool pressed = dashPressed;
        dashPressed = false;
        return pressed;
    }

    public bool ConsumeInteract()
    {
        bool pressed = interactPressed;
        interactPressed = false;
        return pressed;
    }

    public bool ConsumePause()
    {
        bool pressed = pausePressed;
        pausePressed = false;
        return pressed;
    }
}
