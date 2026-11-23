
using UnityEngine;

public class GroundTargetSearchBehaviour : IEnemyTargetSearchBehaviour
{
    private EnemyController controller;

    private Vector2 targetPosition;

    private float horizontalArrivalDistance;
    private float verticalArrivalDistance;
    private float searchSpeed;

    private float originalMovementSpeed;

    public bool IsInitialized => controller != null;
    public bool IsSearching { get; private set; }

    public void Initialize(
        EnemyController controller,
        float horizontalArrivalDistance,
        float verticalArrivalDistance,
        float searchSpeed)
    {
        StopSearching();
        this.controller = null;
        horizontalArrivalDistance = 0f;
        verticalArrivalDistance = 0f;
        searchSpeed = 0f;
        originalMovementSpeed = 0f;

        if (controller == null)
        {
            Debug.LogError(
                "GroundTargetSearchBehaviour requires an EnemyController.");

            return;
        }

        if (!IsFinite(horizontalArrivalDistance) || horizontalArrivalDistance < 0f ||
            !IsFinite(verticalArrivalDistance) || verticalArrivalDistance < 0f ||
            !IsFinite(searchSpeed) || searchSpeed <= 0f || controller.Definition == null)
        {
            Debug.LogError(
                "GroundTargetSearchBehaviour received invalid settings.");

            return;
        }

        this.controller = controller;

        this.horizontalArrivalDistance = horizontalArrivalDistance;
        this.verticalArrivalDistance = verticalArrivalDistance;
        this.searchSpeed = searchSpeed;

        originalMovementSpeed = controller.Definition.MovementSpeed;

        IsSearching = false;
    }

    public void StartSearching(Vector2 targetPosition)
    {
        if (controller == null || !IsFinite(targetPosition))
            return;

        this.targetPosition = targetPosition;

        if (IsSearching)
            return;

        controller.SetMovementSpeed(searchSpeed);

        IsSearching = true;
    }

    public void SetSearchPosition(Vector2 targetPosition)
    {
        if (!IsSearching || !IsFinite(targetPosition))
            return;

        this.targetPosition = targetPosition;
    }

    public void StopSearching()
    {
        if (!IsSearching || controller == null)
            return;

        IsSearching = false;

        controller.StopNormalMovement();
        controller.SetMovementSpeed(originalMovementSpeed);
    }

    public void Update(float deltaTime)
    {
        if (!IsSearching || controller == null ||
            !IsFinite(deltaTime) || deltaTime < 0f)
            return;

        Vector2 currentPosition = controller.transform.position;

        float horizontalDistance = Mathf.Abs(
            targetPosition.x - currentPosition.x);

        float verticalDistance = Mathf.Abs(
            targetPosition.y - currentPosition.y);

        if (horizontalDistance <= horizontalArrivalDistance &&
            verticalDistance <= verticalArrivalDistance)
        {
            StopSearching();
            return;
        }

        Vector2 offset = targetPosition - currentPosition;
        bool canMoveVertically =
            controller.Definition.HasCapability(EnemyCapability.Flying) ||
            controller.Definition.HasCapability(EnemyCapability.Swimming);

        Vector2 direction = canMoveVertically
            ? offset.normalized
            : Vector2.right * Mathf.Sign(offset.x);

        if (Mathf.Abs(direction.x) > 0.001f)
        {
            controller.Face(direction.x < 0f
                ? EnemyFacingDirection.Left
                : EnemyFacingDirection.Right);
        }

        controller.Move(direction);
    }

    private static bool IsFinite(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }

    private static bool IsFinite(Vector2 value)
    {
        return IsFinite(value.x) && IsFinite(value.y);
    }
}
