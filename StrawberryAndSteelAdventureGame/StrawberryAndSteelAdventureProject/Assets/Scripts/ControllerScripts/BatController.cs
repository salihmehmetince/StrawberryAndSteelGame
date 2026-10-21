
using UnityEngine;

public class BatController
{
    private readonly Transform batTransform;
    private readonly Animator batAnimator;
    private readonly SpriteRenderer batSpriteRenderer;

    public float MovementSpeed { get; set; }

    public Vector2 CurrentPosition =>
        new Vector2(
            batTransform.localPosition.x,
            batTransform.localPosition.y
        );

    public bool IsMoving { get; private set; }

    private Vector2 targetPosition;
    private Vector2 somersaultStartPosition;
    private Vector2 somersaultCenter;
    private Vector2 movementDirection;

    private enum SomersaultPhase
    {
        None,
        BeforeSomersault,
        DuringSomersault,
        AfterSomersault
    }

    private SomersaultPhase somersaultPhase;

    private const float SomersaultRadius = 2f;

    private float somersaultAngle;
    private float somersaultProgress;
    private float somersaultAngularSpeed;

    private readonly int isBatIdle = Animator.StringToHash("IsBatIdle");
    private readonly int isBatFlying = Animator.StringToHash("IsBatFlying");
    private readonly int isBatFlyingToFall = Animator.StringToHash("IsBatFlyingToFall");
    private readonly int isBatFalling = Animator.StringToHash("IsBatFalling");
    private readonly int isBatHurt = Animator.StringToHash("IsBatHurt");
    private readonly int isBatDeath = Animator.StringToHash("IsBatDeath");
    private readonly int isBatAttack = Animator.StringToHash("IsBatAttack");

    public BatController(
        Transform batTransform,
        Animator batAnimator,
        SpriteRenderer batSpriteRenderer,
        float movementSpeed)
    {
        this.batTransform = batTransform;
        this.batAnimator = batAnimator;
        this.batSpriteRenderer = batSpriteRenderer;
        MovementSpeed = movementSpeed;
    }

    public void Tick(float deltaTime)
    {
        if (!IsMoving)
            return;

        if (somersaultPhase != SomersaultPhase.None)
        {
            UpdateSomersaultMovement(deltaTime);
            return;
        }

        UpdateStraightMovement(deltaTime);
    }

    public void FlyTo(Vector2 destination)
    {
        StopMovement();

        targetPosition = destination;

        IsMoving = Vector2.Distance(CurrentPosition, targetPosition) > 0.001f;

        if (!IsMoving)
        {
            PlayIdle();
            return;
        }

        UpdateSpriteDirection(targetPosition - CurrentPosition);
        PlayFlying();
    }

    public void FlyWithSomersaultTo(Vector2 destination)
    {
        Vector2 startPosition = CurrentPosition;
        Vector2 displacement = destination - startPosition;

        if (displacement.magnitude <= 0.001f || MovementSpeed <= 0f)
        {
            FlyTo(destination);
            return;
        }

        targetPosition = destination;
        movementDirection = displacement.normalized;

        somersaultStartPosition =
            startPosition + displacement * 0.5f;

        somersaultCenter =
            somersaultStartPosition + Vector2.up * SomersaultRadius;

        somersaultAngle = -Mathf.PI / 2f;
        somersaultProgress = 0f;
        somersaultAngularSpeed = MovementSpeed / SomersaultRadius;

        somersaultPhase = SomersaultPhase.BeforeSomersault;
        IsMoving = true;

        UpdateSpriteDirection(movementDirection);
        PlayFlying();
    }

    private void UpdateStraightMovement(float deltaTime)
    {
        Vector2 currentPosition = CurrentPosition;

        Vector2 newPosition = Vector2.MoveTowards(
            currentPosition,
            targetPosition,
            MovementSpeed * deltaTime
        );

        SetLocalPosition(newPosition);
        UpdateSpriteDirection(targetPosition - currentPosition);

        if (Vector2.Distance(newPosition, targetPosition) <= 0.001f)
            FinishMovement();
    }

    private void UpdateSomersaultMovement(float deltaTime)
    {
        switch (somersaultPhase)
        {
            case SomersaultPhase.BeforeSomersault:
                {
                    Vector2 newPosition = Vector2.MoveTowards(
                        CurrentPosition,
                        somersaultStartPosition,
                        MovementSpeed * deltaTime
                    );

                    SetLocalPosition(newPosition);
                    UpdateSpriteDirection(movementDirection);

                    if (Vector2.Distance(newPosition, somersaultStartPosition) <= 0.001f)
                    {
                        SetLocalPosition(somersaultStartPosition);
                        somersaultProgress = 0f;
                        somersaultPhase = SomersaultPhase.DuringSomersault;
                    }

                    break;
                }

            case SomersaultPhase.DuringSomersault:
                {
                    float remainingAngle =
                        Mathf.PI * 2f - somersaultProgress;

                    float angleStep =
                        somersaultAngularSpeed * deltaTime;

                    if (angleStep > remainingAngle)
                        angleStep = remainingAngle;

                    somersaultAngle += angleStep;
                    somersaultProgress += angleStep;

                    float horizontalDirection =
                        movementDirection.x < 0f ? -1f : 1f;

                    float circleX =
                        Mathf.Cos(somersaultAngle) *
                        SomersaultRadius *
                        horizontalDirection;

                    float circleY =
                        Mathf.Sin(somersaultAngle) *
                        SomersaultRadius;

                    Vector2 newPosition = new Vector2(
                        somersaultCenter.x + circleX,
                        somersaultCenter.y + circleY
                    );

                    SetLocalPosition(newPosition);

                    Vector2 tangent = new Vector2(
                        -Mathf.Sin(somersaultAngle) * horizontalDirection,
                        Mathf.Cos(somersaultAngle)
                    );

                    UpdateSpriteRotation(tangent);

                    if (somersaultProgress >= Mathf.PI * 2f - 0.0001f)
                    {
                        SetLocalPosition(somersaultStartPosition);
                        somersaultPhase = SomersaultPhase.AfterSomersault;

                        batTransform.localRotation = Quaternion.identity;
                        UpdateSpriteDirection(movementDirection);
                    }

                    break;
                }

            case SomersaultPhase.AfterSomersault:
                {
                    Vector2 currentPosition = CurrentPosition;

                    Vector2 newPosition = Vector2.MoveTowards(
                        currentPosition,
                        targetPosition,
                        MovementSpeed * deltaTime
                    );

                    SetLocalPosition(newPosition);
                    UpdateSpriteDirection(targetPosition - currentPosition);

                    if (Vector2.Distance(newPosition, targetPosition) <= 0.001f)
                        FinishMovement();

                    break;
                }
        }
    }

    private void SetLocalPosition(Vector2 position)
    {
        batTransform.localPosition = new Vector3(
            position.x,
            position.y,
            batTransform.localPosition.z
        );
    }

    private void UpdateSpriteDirection(Vector2 direction)
    {
        if (batSpriteRenderer == null)
            return;

        if (Mathf.Abs(direction.x) > 0.001f)
            batSpriteRenderer.flipX = direction.x < 0f;
    }

    private void UpdateSpriteRotation(Vector2 direction)
    {
        float angle =
            Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // flipX yatay aynalama yaptýðý için dönüþ açýsýný da düzelt.
        if (batSpriteRenderer != null && batSpriteRenderer.flipX)
            angle += 180f;

        batTransform.localRotation =
            Quaternion.Euler(0f, 0f, angle);
    }

    private void FinishMovement()
    {
        IsMoving = false;
        somersaultPhase = SomersaultPhase.None;

        batTransform.localRotation = Quaternion.identity;

        PlayIdle();
    }

    private void StopMovement()
    {
        IsMoving = false;
        somersaultPhase = SomersaultPhase.None;

        batTransform.localRotation = Quaternion.identity;
    }

    public void PlayIdle()
    {
        SetAnimation(isBatIdle);
    }

    public void PlayFlying()
    {
        SetAnimation(isBatFlying);
    }

    public void FlyToFall()
    {
        StopMovement();
        SetAnimation(isBatFlyingToFall);
    }

    public void Fall()
    {
        StopMovement();
        SetAnimation(isBatFalling);
    }

    public void Hurt()
    {
        StopMovement();
        SetAnimation(isBatHurt);
    }

    public void Attack()
    {
        StopMovement();
        SetAnimation(isBatAttack);
    }

    public void Die()
    {
        StopMovement();
        SetAnimation(isBatDeath);
    }

    private void SetAnimation(int targetAnimation)
    {
        batAnimator.SetBool(isBatIdle, false);
        batAnimator.SetBool(isBatFlying, false);
        batAnimator.SetBool(isBatFlyingToFall, false);
        batAnimator.SetBool(isBatFalling, false);
        batAnimator.SetBool(isBatHurt, false);
        batAnimator.SetBool(isBatDeath, false);
        batAnimator.SetBool(isBatAttack, false);

        batAnimator.SetBool(targetAnimation, true);
    }
}
