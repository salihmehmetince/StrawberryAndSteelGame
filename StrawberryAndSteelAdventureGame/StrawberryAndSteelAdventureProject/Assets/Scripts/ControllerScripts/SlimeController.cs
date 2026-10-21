using UnityEngine;

public class SlimeController
{
    private static readonly string[] MovementNames =
    {
        "Idle",
        "Attack",
        "Death",
        "Hurt",
        "Walk"
    };

    private readonly Transform slimeTransform;
    private readonly Animator slimeAnimator;
    private readonly SpriteRenderer slimeSpriteRenderer;

    private Vector2 targetPosition;
    private bool isWalking;

    public float MovementSpeed { get; set; }
    public bool IsMoving => isWalking;

    public SlimeController(
        Transform slimeTransform,
        Animator slimeAnimator,
        SpriteRenderer slimeSpriteRenderer,
        float movementSpeed)
    {
        this.slimeTransform = slimeTransform;
        this.slimeAnimator = slimeAnimator;
        this.slimeSpriteRenderer = slimeSpriteRenderer;
        MovementSpeed = movementSpeed;

        PlayIdle();
    }

    public void Tick(float deltaTime)
    {
        if (!isWalking)
            return;

        Vector2 currentPosition = CurrentPosition;
        Vector2 newPosition = Vector2.MoveTowards(
            currentPosition,
            targetPosition,
            MovementSpeed * deltaTime
        );

        SetLocalPosition(newPosition);

        if (Vector2.Distance(newPosition, targetPosition) <= 0.001f)
        {
            SetLocalPosition(targetPosition);
            isWalking = false;
            PlayIdle();
        }
    }

    public void WalkTo(Vector2 destination)
    {
        targetPosition = destination;
        Vector2 displacement = targetPosition - CurrentPosition;

        if (MovementSpeed <= 0f || Mathf.Abs(displacement.x) <= 0.001f)
        {
            SetLocalPosition(targetPosition);
            PlayIdle();
            return;
        }

        slimeSpriteRenderer.flipX = displacement.x < 0f;
        isWalking = true;
        PlayMovement("Walk");
    }

    public void PlayIdle() => PlayMovement("Idle");
    public void PlayAttack() => PlayMovement("Attack");
    public void PlayDeath() => PlayMovement("Death");
    public void PlayHurt() => PlayMovement("Hurt");
    public void PlayWalk() => PlayMovement("Walk");

    public void PlayMovement(string movementName)
    {
        bool isValidMovement = false;

        foreach (string name in MovementNames)
        {
            bool isSelected = name == movementName;
            slimeAnimator.SetBool("Is" + name, isSelected);
            isValidMovement |= isSelected;
        }

        if (!isValidMovement)
            Debug.LogWarning("Unknown slime movement: " + movementName, slimeTransform);
    }

    private Vector2 CurrentPosition =>
        new Vector2(slimeTransform.localPosition.x, slimeTransform.localPosition.y);

    private void SetLocalPosition(Vector2 position)
    {
        slimeTransform.localPosition = new Vector3(
            position.x,
            position.y,
            slimeTransform.localPosition.z
        );
    }
}
