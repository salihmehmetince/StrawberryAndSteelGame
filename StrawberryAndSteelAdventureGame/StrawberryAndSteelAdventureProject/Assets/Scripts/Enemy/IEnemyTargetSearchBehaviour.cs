using UnityEngine;

public interface IEnemyTargetSearchBehaviour
{
    bool IsSearching { get; }

    void Initialize(
        EnemyController controller,
        float horizontalArrivalDistance,
        float verticalArrivalDistance,
        float searchSpeed);

    void StartSearching(Vector2 targetPosition);

    void SetSearchPosition(Vector2 targetPosition);

    void StopSearching();

    void Update(float deltaTime);
}