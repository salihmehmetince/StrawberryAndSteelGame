
using UnityEngine;

public class GroundTargetSearchBehaviour : IEnemyTargetSearchBehaviour
{
    private EnemyController controller;

    private Vector2 targetPosition;

    private float horizontalArrivalDistance;
    private float verticalArrivalDistance;
    private float searchSpeed;

    private float originalMovementSpeed;

    public bool IsSearching { get; private set; }

    public void Initialize(
        EnemyController controller,
        float horizontalArrivalDistance,
        float verticalArrivalDistance,
        float searchSpeed)
    {
        if (controller == null)
        {
            Debug.LogError(
                "GroundTargetSearchBehaviour requires an EnemyController.");

            return;
        }

        if (horizontalArrivalDistance < 0f ||
            verticalArrivalDistance < 0f ||
            searchSpeed <= 0f)
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
        if (controller == null)
            return;

        this.targetPosition = targetPosition;

        if (IsSearching)
            return;

        controller.SetMovementSpeed(searchSpeed);

        IsSearching = true;
    }

    public void SetSearchPosition(Vector2 targetPosition)
    {
        if (!IsSearching)
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
        if (!IsSearching || controller == null)
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

        float directionX = Mathf.Sign(
            targetPosition.x - currentPosition.x);

        controller.Move(Vector2.right * directionX);
    }
}
