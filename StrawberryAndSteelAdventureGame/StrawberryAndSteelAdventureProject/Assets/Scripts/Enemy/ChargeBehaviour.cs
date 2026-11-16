
using UnityEngine;

public class ChargeBehaviour : IEnemyMovementAction
{
    private Rigidbody2D enemyRigidbody;

    private float chargeSpeed;
    private float chargeDuration;
    private float remainingDuration;

    private Vector2 chargeDirection;

    public EnemyMovementActionPriority Priority =>
        EnemyMovementActionPriority.Charge;

    public bool IsActive { get; private set; }

    public void Initialize(
        EnemyController controller,
        float chargeSpeed,
        float chargeDuration)
    {
        enemyRigidbody = controller.EnemyRigidbody;

        this.chargeSpeed = chargeSpeed;
        this.chargeDuration = chargeDuration;

        remainingDuration = 0f;
        IsActive = false;

        if (enemyRigidbody == null)
        {
            Debug.LogError(
                "ChargeBehaviour requires a Rigidbody2D component.");

            return;
        }
    }

    public bool StartCharge(Vector2 targetPosition)
    {
        if (enemyRigidbody == null || IsActive)
            return false;

        Vector2 direction =
            targetPosition - enemyRigidbody.position;

        if (direction.sqrMagnitude == 0f || chargeDuration <= 0f)
            return false;

        chargeDirection = direction.normalized;
        remainingDuration = chargeDuration;
        IsActive = true;

        ApplyChargeVelocity();

        return true;
    }

    public void Update(float deltaTime)
    {
        if (!IsActive || enemyRigidbody == null)
            return;

        remainingDuration -= deltaTime;

        if (remainingDuration <= 0f)
        {
            Stop();
            return;
        }

        ApplyChargeVelocity();
    }

    private void ApplyChargeVelocity()
    {
        enemyRigidbody.linearVelocity =
            chargeDirection * chargeSpeed;
    }

    public void Stop()
    {
        IsActive = false;
        remainingDuration = 0f;
    }
}
