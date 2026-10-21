using UnityEngine;

public class WitchController
{
    private static readonly string[] MovementNames =
    {
        "Idle", "Run", "Attack", "Hurt", "Death", "FlyWithBroom"
    };

    private readonly Transform witchTransform;
    private readonly Animator witchAnimator;
    private readonly SpriteRenderer witchSpriteRenderer;
    private Vector2 targetPosition;
    private bool isMoving;

    public float MovementSpeed { get; set; }
    public bool IsMoving => isMoving;

    public WitchController(Transform witchTransform, Animator witchAnimator,
        SpriteRenderer witchSpriteRenderer, float movementSpeed)
    {
        this.witchTransform = witchTransform;
        this.witchAnimator = witchAnimator;
        this.witchSpriteRenderer = witchSpriteRenderer;
        MovementSpeed = movementSpeed;
        PlayIdle();
    }

    public void Tick(float deltaTime)
    {
        if (!isMoving) return;
        Vector2 nextPosition = Vector2.MoveTowards(CurrentPosition, targetPosition,
            MovementSpeed * deltaTime);
        SetLocalPosition(nextPosition);
        if (Vector2.Distance(nextPosition, targetPosition) <= 0.001f)
        {
            SetLocalPosition(targetPosition);
            isMoving = false;
            PlayIdle();
        }
    }

    public void MoveTo(Vector2 destination, string movementName = "FlyWithBroom")
    {
        targetPosition = destination;
        Vector2 displacement = targetPosition - CurrentPosition;
        if (MovementSpeed <= 0f || displacement.sqrMagnitude <= 0.000001f)
        {
            SetLocalPosition(targetPosition);
            isMoving = false;
            PlayIdle();
            return;
        }
        witchSpriteRenderer.flipX = displacement.x < 0f;
        isMoving = true;
        PlayMovement(movementName);
    }

    public void PlayIdle() => PlayMovement("Idle");
    public void PlayRun() => PlayMovement("Run");
    public void PlayAttack() => PlayMovement("Attack");
    public void PlayHurt() => PlayMovement("Hurt");
    public void PlayDeath() => PlayMovement("Death");
    public void PlayFlyWithBroom() => PlayMovement("FlyWithBroom");

    public void PlayMovement(string movementName)
    {
        bool valid = false;
        foreach (string name in MovementNames)
        {
            bool selected = name == movementName;
            witchAnimator.SetBool("Is" + name, selected);
            valid |= selected;
        }
        if (!valid)
            Debug.LogWarning("Unknown witch movement: " + movementName, witchTransform);
    }

    private Vector2 CurrentPosition =>
        new Vector2(witchTransform.localPosition.x, witchTransform.localPosition.y);

    private void SetLocalPosition(Vector2 position)
    {
        witchTransform.localPosition = new Vector3(position.x, position.y,
            witchTransform.localPosition.z);
    }
}
