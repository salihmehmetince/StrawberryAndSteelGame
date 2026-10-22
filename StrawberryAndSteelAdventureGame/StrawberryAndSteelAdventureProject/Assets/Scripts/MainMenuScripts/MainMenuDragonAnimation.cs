using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Moves one Dragon across the menu from one edge to the other. It follows a positive
/// sine arch for five seconds, then flies straight for five seconds, repeating the pattern.
/// </summary>
public sealed class MainMenuDragonAnimation : MonoBehaviour
{
    [SerializeField, Min(0.1f)] private int sortingOrder = 7;

    private DragonController controller;
    private float startX;
    private float endX;
    private float laneY;
    private float movementSpeed;
    private float sineSegmentDuration;
    private float straightSegmentDuration;
    private float sineAmplitude;
    private float elapsed;
    private bool initialized;

    public void Initialize(
        float startX,
        float endX,
        float laneY,
        float movementSpeed,
        float sineSegmentDuration,
        float straightSegmentDuration,
        float sineAmplitude)
    {
        Animator animator = GetComponent<Animator>();
        if (animator == null || Mathf.Abs(endX - startX) <= 0.001f)
        {
            Debug.LogError("A Dragon needs an Animator and distinct start/end positions.", this);
            Destroy(gameObject);
            return;
        }

        this.startX = startX;
        this.endX = endX;
        this.laneY = laneY;
        this.movementSpeed = Mathf.Max(0.01f, movementSpeed);
        this.sineSegmentDuration = Mathf.Max(0.1f, sineSegmentDuration);
        this.straightSegmentDuration = Mathf.Max(0.1f, straightSegmentDuration);
        this.sineAmplitude = Mathf.Max(0f, sineAmplitude);
        elapsed = 0f;

        SortingGroup sortingGroup = GetComponent<SortingGroup>();
        if (sortingGroup == null)
            sortingGroup = gameObject.AddComponent<SortingGroup>();
        sortingGroup.sortingLayerName = "Default";
        sortingGroup.sortingOrder = sortingOrder;

        controller = new DragonController(transform, animator);
        controller.PlayFly();
        controller.SetFlightPose(startX, laneY, endX > startX, 0f);
        initialized = true;
    }

    private void Update()
    {
        if (!initialized)
            return;

        elapsed += Time.deltaTime;
        float direction = Mathf.Sign(endX - startX);
        float distance = Mathf.Abs(endX - startX);
        float travelled = movementSpeed * elapsed;

        if (travelled >= distance)
        {
            controller.SetFlightPose(endX, laneY, endX > startX, 0f);
            Destroy(gameObject);
            return;
        }

        float x = startX + direction * travelled;
        float patternDuration = sineSegmentDuration + straightSegmentDuration;
        float patternTime = elapsed % patternDuration;
        float y = laneY;
        float slope = 0f;

        if (patternTime < sineSegmentDuration)
        {
            float progress = patternTime / sineSegmentDuration;
            float sine = Mathf.Sin(Mathf.PI * progress);

            // sin² creates one positive arch and returns to the lane with a flat tangent.
            y += sineAmplitude * sine * sine;

            float verticalSpeed = sineAmplitude * Mathf.PI / sineSegmentDuration
                * Mathf.Sin(2f * Mathf.PI * progress);
            slope = verticalSpeed / (direction * movementSpeed);
        }

        float rotationZ = Mathf.Atan(slope) * Mathf.Rad2Deg;
        controller.SetFlightPose(x, y, direction > 0f, rotationZ);
    }
}
