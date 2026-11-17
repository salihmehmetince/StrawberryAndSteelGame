
using UnityEngine;

public class EnemyFacingSystem
{
    private Transform visualRoot;
    private Vector3 originalScale;

    public EnemyFacingDirection CurrentDirection { get; private set; }

    public void Initialize(Transform visualRoot)
    {
        if (visualRoot == null)
        {
            Debug.LogError("EnemyFacingSystem requires a Visual Root.");
            return;
        }

        this.visualRoot = visualRoot;
        originalScale = visualRoot.localScale;

        CurrentDirection = originalScale.x < 0f
            ? EnemyFacingDirection.Left
            : EnemyFacingDirection.Right;
    }

    public void Face(EnemyFacingDirection direction)
    {
        if (visualRoot == null) return;

        CurrentDirection = direction;

        Vector3 scale = originalScale;
        scale.x = Mathf.Abs(originalScale.x);

        if (direction == EnemyFacingDirection.Left)
            scale.x *= -1f;

        visualRoot.localScale = scale;
    }

    public void FaceTarget(Vector2 targetPosition, Vector2 currentPosition)
    {
        float direction = targetPosition.x - currentPosition.x;

        if (Mathf.Approximately(direction, 0f))
            return;

        Face(direction < 0f
            ? EnemyFacingDirection.Left
            : EnemyFacingDirection.Right);
    }

    public bool IsFacing(EnemyFacingDirection direction)
    {
        return CurrentDirection == direction;
    }
}
