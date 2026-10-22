using System.Collections;
using UnityEngine;

public sealed class MainMenuPenguinsAnimation : MonoBehaviour
{
    [SerializeField] private RuntimeAnimatorController penguinAnimator;
    [SerializeField, Min(0.1f)] private float minimumSpawnDelay = 10f;
    [SerializeField, Min(0.1f)] private float maximumSpawnDelay = 15f;
    [SerializeField, Min(1)] private int maximumPenguinCount = 8;
    [SerializeField, Min(0.1f)] private float movementSpeed = 5f;
    [SerializeField, Min(0.1f)] private float jumpDuration = 0.7f;
    [SerializeField, Min(0f)] private float jumpHeight = 2f;

    private const float LeftEdge = -22f;
    private const float RightEdge = 22f;
    private const float LaneY = -6.3f;
    private const float SlideLaneY = -6.5f;

    private void Start()
    {
        if (penguinAnimator == null)
        {
            Debug.LogError("Assign PenguinMovementAnimator to the Penguins object.", this);
            enabled = false;
            return;
        }

        StartCoroutine(SpawnPenguins());
    }

    private IEnumerator SpawnPenguins()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minimumSpawnDelay, maximumSpawnDelay));
            if (transform.childCount >= maximumPenguinCount)
                continue;

            bool movingRight = Random.value < 0.5f;
            float startX = movingRight ? LeftEdge : RightEdge;
            float endX = movingRight ? RightEdge : LeftEdge;
            var action = (PenguinController.Action)Random.Range(0, 3);
            float laneY = action == PenguinController.Action.Slide ? SlideLaneY : LaneY;

            GameObject penguin = new GameObject("Penguin");
            penguin.transform.SetParent(transform, false);
            penguin.transform.localPosition = new Vector3(startX, laneY, 0f);
            penguin.AddComponent<SpriteRenderer>();
            Animator animator = penguin.AddComponent<Animator>();
            animator.runtimeAnimatorController = penguinAnimator;

            MainMenuPenguinAnimation animation = penguin.AddComponent<MainMenuPenguinAnimation>();
            animation.Initialize(startX, endX, laneY, movementSpeed, action, jumpDuration, jumpHeight);
        }
    }
}
