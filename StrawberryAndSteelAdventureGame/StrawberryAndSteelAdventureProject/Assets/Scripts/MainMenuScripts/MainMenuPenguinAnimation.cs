using UnityEngine;

public sealed class MainMenuPenguinAnimation : MonoBehaviour
{
    private PenguinController controller;

    public void Initialize(
        float startX,
        float endX,
        float laneY,
        float movementSpeed,
        PenguinController.Action action,
        float jumpDuration,
        float jumpHeight)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        Animator animator = GetComponent<Animator>();
        if (spriteRenderer == null || animator == null)
        {
            Debug.LogError("Penguin needs a SpriteRenderer and Animator.", this);
            Destroy(gameObject);
            return;
        }

        // Run and jump face left in the source; the horizontal slide art faces right.
        spriteRenderer.flipX = action == PenguinController.Action.Slide
            ? endX < startX
            : endX > startX;
        spriteRenderer.sortingOrder = 6;
        controller = new PenguinController(
            transform, animator, startX, endX, laneY, movementSpeed,
            action, jumpDuration, jumpHeight);
    }

    private void Update()
    {
        controller?.Tick(Time.deltaTime);
        if (controller != null && controller.IsComplete)
            Destroy(gameObject);
    }
}
