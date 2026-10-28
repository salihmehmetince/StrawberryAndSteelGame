
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
        bool result = attackPressed;
        attackPressed = false;
        return result;
    }

    public bool ConsumeDash()
    {
        bool result = dashPressed;
        dashPressed = false;
        return result;
    }

    public bool ConsumeInteract()
    {
        bool result = interactPressed;
        interactPressed = false;
        return result;
    }

    public bool ConsumePause()
    {
        bool result = pausePressed;
        pausePressed = false;
        return result;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
            ResetInputs();
    }

    private void OnDisable()
    {
        ResetInputs();
    }

    private void ResetInputs()
    {
        MoveInput = Vector2.zero;
        IsDefending = false;

        attackPressed = false;
        dashPressed = false;
        interactPressed = false;
        pausePressed = false;
    }
}
