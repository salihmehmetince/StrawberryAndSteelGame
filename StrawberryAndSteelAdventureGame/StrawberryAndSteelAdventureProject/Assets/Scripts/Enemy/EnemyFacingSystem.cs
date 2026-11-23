
using UnityEngine;

public class EnemyFacingSystem
{
    private Transform visualRoot;
    private Vector3 originalScale;

    public EnemyFacingDirection CurrentDirection { get; private set; }

    public bool IsInitialized => visualRoot != null;

    public void Initialize(Transform visualRoot)
    {
        this.visualRoot = null;
        originalScale = Vector3.one;

        if (visualRoot == null)
        {
            Debug.LogError("EnemyFacingSystem requires a Visual Root.");
            return;
        }

        Vector3 scale = visualRoot.localScale;
        if (!IsFinite(scale.x) || !IsFinite(scale.y) || !IsFinite(scale.z))
        {
            Debug.LogError("EnemyFacingSystem received a non-finite visual scale.");
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

    private static bool IsFinite(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }
}
