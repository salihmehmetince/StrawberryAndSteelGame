
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 MoveInput { get; private set; }

    public bool IsDefending { get; private set; }

    private bool jumpPressed;
    private bool attackPressed;
    private bool dashPressed;
    private bool interactPressed;

    private bool wasUpPressed;

    // WASD, yön tuþlarý ve gamepad sol analog çubuðu.
    public void OnMove(InputValue value)
    {
        MoveInput = value.Get<Vector2>();

        // Yukarý yönüne ilk basýþ zýplama isteði oluþturur.
        bool isUpPressed = MoveInput.y > 0.5f;

        if (isUpPressed && !wasUpPressed)
            jumpPressed = true;

        wasUpPressed = isUpPressed;

    }

    // Sol fare tuþu / Gamepad West.
    public void OnAttack(InputValue value)
    {
        if (value.isPressed)
            attackPressed = true;
    }

    // F / Gamepad East.
    public void OnDefend(InputValue value)
    {
        IsDefending = value.isPressed;
    }

    // Shift / Gamepad South.
    public void OnDash(InputValue value)
    {
        if (value.isPressed)
            dashPressed = true;
    }

    // E / Gamepad North.
    public void OnInteract(InputValue value)
    {
        if (value.isPressed)
            interactPressed = true;
    }

    public bool ConsumeJump()
    {
        bool pressed = jumpPressed;
        jumpPressed = false;
        return pressed;
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

        jumpPressed = false;
        attackPressed = false;
        dashPressed = false;
        interactPressed = false;

        wasUpPressed = false;
    }
}
