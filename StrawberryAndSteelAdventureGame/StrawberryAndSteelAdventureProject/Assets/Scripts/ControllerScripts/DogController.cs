using UnityEngine;

public class DogController
{
    private static readonly string[] MovementNames =
    {
        "Idle",
        "Bark",
        "Itching",
        "Licking1",
        "Licking2",
        "LyingDown",
        "Run",
        "Sitting",
        "Sleeping",
        "Stretching",
        "Walk"
    };

    private readonly Transform dogTransform;
    private readonly Animator dogAnimator;
    private readonly SpriteRenderer dogSpriteRenderer;

    private Vector2 targetPosition;
    private bool isRunning;

    public float MovementSpeed { get; set; }
    public bool IsMoving => isRunning;

    public DogController(
        Transform dogTransform,
        Animator dogAnimator,
        SpriteRenderer dogSpriteRenderer,
        float movementSpeed)
    {
        this.dogTransform = dogTransform;
        this.dogAnimator = dogAnimator;
        this.dogSpriteRenderer = dogSpriteRenderer;
        MovementSpeed = movementSpeed;

        PlayIdle();
    }

    public void Tick(float deltaTime)
    {
        if (!isRunning)
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
            isRunning = false;
            PlayIdle();
        }
    }

    public void RunTo(Vector2 destination)
    {
        targetPosition = destination;
        Vector2 displacement = targetPosition - CurrentPosition;

        if (MovementSpeed <= 0f || Mathf.Abs(displacement.x) <= 0.001f)
        {
            SetLocalPosition(targetPosition);
            PlayIdle();
            return;
        }

        dogSpriteRenderer.flipX = displacement.x < 0f;
        isRunning = true;
        PlayMovement("Run");
    }

    public void PlayIdle() => PlayMovement("Idle");
    public void PlayBark() => PlayMovement("Bark");
    public void PlayItching() => PlayMovement("Itching");
    public void PlayLicking1() => PlayMovement("Licking1");
    public void PlayLicking2() => PlayMovement("Licking2");
    public void PlayLyingDown() => PlayMovement("LyingDown");
    public void PlayRun() => PlayMovement("Run");
    public void PlaySitting() => PlayMovement("Sitting");
    public void PlaySleeping() => PlayMovement("Sleeping");
    public void PlayStretching() => PlayMovement("Stretching");
    public void PlayWalk() => PlayMovement("Walk");

    public void PlayMovement(string movementName)
    {
        bool isValidMovement = false;

        foreach (string name in MovementNames)
        {
            bool isSelected = name == movementName;
            dogAnimator.SetBool("Is" + name, isSelected);
            isValidMovement |= isSelected;
        }

        if (!isValidMovement)
            Debug.LogWarning("Unknown dog movement: " + movementName, dogTransform);
    }

    private Vector2 CurrentPosition =>
        new Vector2(dogTransform.localPosition.x, dogTransform.localPosition.y);

    private void SetLocalPosition(Vector2 position)
    {
        dogTransform.localPosition = new Vector3(
            position.x,
            position.y,
            dogTransform.localPosition.z
        );
    }
}
