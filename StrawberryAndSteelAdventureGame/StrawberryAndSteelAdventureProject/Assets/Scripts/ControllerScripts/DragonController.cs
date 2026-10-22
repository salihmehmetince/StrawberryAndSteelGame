using UnityEngine;

/// <summary>
/// Owns the Dragon animator commands and the transform operations used while it flies.
/// </summary>
public sealed class DragonController
{
    private const float DragonScaleMultiplier = 0.5f;

    private static readonly string[] AnimationStates =
    {
        "Idle",
        "Move",
        "Talk",
        "Climb",
        "Jump",
        "Attack",
        "Stuned",
        "Death",
        "SpecialAttack",
        "Fly"
    };

    private readonly Transform dragonTransform;
    private readonly Animator animator;
    private readonly Vector3 originalScale;

    public DragonController(Transform dragonTransform, Animator animator)
    {
        this.dragonTransform = dragonTransform;
        this.animator = animator;
        originalScale = dragonTransform != null ? dragonTransform.localScale : Vector3.one;
    }

    public void PlayIdle() => PlayAnimation("Idle");
    public void PlayMove() => PlayAnimation("Move");
    public void PlayTalk() => PlayAnimation("Talk");
    public void PlayClimb() => PlayAnimation("Climb");
    public void PlayJump() => PlayAnimation("Jump");
    public void PlayAttack() => PlayAnimation("Attack");
    public void PlayStuned() => PlayAnimation("Stuned");
    public void PlayDeath() => PlayAnimation("Death");
    public void PlaySpecialAttack() => PlayAnimation("SpecialAttack");
    public void PlayFly() => PlayAnimation("Fly");

    public void SetFlightPose(float x, float y, bool movingRight, float rotationZ)
    {
        if (dragonTransform == null)
            return;

        dragonTransform.localPosition = new Vector3(x, y, dragonTransform.localPosition.z);
        SetFacingDirection(movingRight);
        SetRotationZ(rotationZ);
    }

    public void MoveToLocalPosition(Vector2 position)
    {
        if (dragonTransform == null)
            return;

        dragonTransform.localPosition = new Vector3(
            position.x,
            position.y,
            dragonTransform.localPosition.z);
    }

    public void MoveTowardsLocalPosition(Vector2 target, float speed, float deltaTime)
    {
        if (dragonTransform == null)
            return;

        Vector2 current = new Vector2(
            dragonTransform.localPosition.x,
            dragonTransform.localPosition.y);
        MoveToLocalPosition(Vector2.MoveTowards(current, target, Mathf.Max(0f, speed) * deltaTime));
    }

    public void SetFacingDirection(bool movingRight)
    {
        if (dragonTransform == null)
            return;

        Vector3 scale = originalScale;
        scale.x = Mathf.Abs(originalScale.x) * DragonScaleMultiplier * (movingRight ? -1f : 1f);
        scale.y = Mathf.Abs(originalScale.y) * DragonScaleMultiplier;
        dragonTransform.localScale = scale;
    }

    public void SetRotationZ(float degrees)
    {
        if (dragonTransform != null)
            dragonTransform.localRotation = Quaternion.Euler(0f, 0f, degrees);
    }

    private void PlayAnimation(string state)
    {
        if (animator == null)
            return;

        for (int i = 0; i < AnimationStates.Length; i++)
            animator.SetBool("Is" + AnimationStates[i], AnimationStates[i] == state);

        // Keep the bools in sync with the controller while allowing commands to switch
        // directly between any of the existing animation states.
        animator.Play(state, 0, 0f);
    }
}
