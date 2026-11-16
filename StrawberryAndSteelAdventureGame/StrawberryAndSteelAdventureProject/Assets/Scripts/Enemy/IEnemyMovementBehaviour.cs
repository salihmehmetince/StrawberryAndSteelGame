
using UnityEngine;

public interface IEnemyMovementBehaviour
{
    bool IsMoving { get; }

    void Initialize(EnemyController controller);

    void Move(Vector2 direction);

    void Stop();

    void SetSpeed(float speed);

    void Update(float deltaTime);
}
