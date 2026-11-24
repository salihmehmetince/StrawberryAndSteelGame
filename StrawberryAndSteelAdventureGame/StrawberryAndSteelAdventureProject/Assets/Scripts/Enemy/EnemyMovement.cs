
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyDefinition enemyDefinition;
    [SerializeField] private Animator animator;

    private Rigidbody2D rb;
    private EnemyAnimationController animationController;

    private MovementMode currentMode = MovementMode.None;

    private Vector2 targetPosition;
    private Vector2 firstPoint;
    private Vector2 secondPoint;

    private bool movingToFirstPoint = true;

    private const float ArrivalDistance = 0.05f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (animator == null)
            animator = GetComponentInChildren<Animator>();

        if (animator != null)
            animationController = new EnemyAnimationController(animator);
    }

    private void FixedUpdate()
    {
        if (currentMode == MovementMode.None)
            return;

        MoveTowardsTarget();
    }

    public void Walk(float x)
    {
        targetPosition = new Vector2(x, transform.localPosition.y);
        currentMode = MovementMode.Walk;

        PlayMovementAnimation();
    }

    public void Run(float x)
    {
        targetPosition = new Vector2(x, transform.localPosition.y);
        currentMode = MovementMode.Run;

        PlayMovementAnimation();
    }

    public void WalkBetween(float x1, float x2)
    {
        firstPoint = new Vector2(x1, transform.localPosition.y);
        secondPoint = new Vector2(x2, transform.localPosition.y);

        StartBetweenMovement(MovementMode.WalkBetween);
    }

    public void RunBetween(float x1, float x2)
    {
        firstPoint = new Vector2(x1, transform.localPosition.y);
        secondPoint = new Vector2(x2, transform.localPosition.y);

        StartBetweenMovement(MovementMode.RunBetween);
    }

    public void Fly(float x, float y)
    {
        targetPosition = new Vector2(x, y);
        currentMode = MovementMode.Fly;

        PlayMovementAnimation();
    }

    public void FlyBetween(float x1, float y1, float x2, float y2)
    {
        firstPoint = new Vector2(x1, y1);
        secondPoint = new Vector2(x2, y2);

        StartBetweenMovement(MovementMode.FlyBetween);
    }

    private void StartBetweenMovement(MovementMode movementMode)
    {
        movingToFirstPoint = true;
        targetPosition = firstPoint;
        currentMode = movementMode;

        PlayMovementAnimation();
    }

    private void MoveTowardsTarget()
    {
        Vector2 currentPosition = transform.localPosition;
        Vector2 difference = targetPosition - currentPosition;
        bool isFlying = currentMode == MovementMode.Fly ||
            currentMode == MovementMode.FlyBetween;

        bool hasReachedTarget = isFlying
            ? difference.magnitude <= ArrivalDistance
            : Mathf.Abs(difference.x) <= ArrivalDistance;

        if (hasReachedTarget)
        {
            if (currentMode == MovementMode.WalkBetween ||
                currentMode == MovementMode.RunBetween ||
                currentMode == MovementMode.FlyBetween)
            {
                movingToFirstPoint = !movingToFirstPoint;

                targetPosition = movingToFirstPoint
                    ? firstPoint
                    : secondPoint;

                difference = targetPosition - currentPosition;
            }
            else
            {
                Stop();
                return;
            }
        }

        float speed = enemyDefinition.MovementSpeed;

        if (currentMode == MovementMode.Run ||
            currentMode == MovementMode.RunBetween)
        {
            speed *= 2f;
        }

        // Ground enemies only patrol/chase horizontally. Including the Y
        // difference here makes them steer back toward their original height,
        // counteracting gravity whenever they are dropped or knocked upward.
        if (!isFlying)
        {
            float localXDirection = Mathf.Sign(difference.x);
            float worldXDirection = transform.parent != null
                ? transform.parent.TransformDirection(new Vector3(localXDirection, 0f, 0f)).x
                : localXDirection;

            rb.linearVelocity = new Vector2(
                worldXDirection * speed,
                rb.linearVelocity.y);
            return;
        }

        Vector2 localDirection = difference.normalized;
        Vector2 worldDirection = transform.parent != null
            ? transform.parent.TransformDirection(localDirection)
            : localDirection;

        rb.linearVelocity = worldDirection * speed;
    }

    private void PlayMovementAnimation()
    {
        animationController?.PlayRun();
    }

    public void Stop(bool playIdleAnimation = true, bool preserveVerticalVelocity = false)
    {
        bool wasFlying = currentMode == MovementMode.Fly ||
            currentMode == MovementMode.FlyBetween;
        float verticalVelocity = preserveVerticalVelocity || !wasFlying
            ? rb.linearVelocity.y
            : 0f;

        currentMode = MovementMode.None;
        rb.linearVelocity = new Vector2(0f, verticalVelocity);

        if (playIdleAnimation)
            animationController?.PlayIdle();
    }
}
