using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Periodically spawns Dragon prefabs at either screen edge and gives each one its flight path.
/// </summary>
public sealed class MainMenuDragonsAnimation : MonoBehaviour
{
    [SerializeField] private GameObject dragonPrefab;
    [SerializeField] private RuntimeAnimatorController dragonAnimator;
    [SerializeField, Min(0.1f)] private float minimumSpawnDelay = 15f;
    [SerializeField, Min(0.1f)] private float maximumSpawnDelay = 20f;
    [SerializeField, Min(1)] private int maximumDragonCount = 4;
    [SerializeField] private float leftEdge = -22f;
    [SerializeField] private float rightEdge = 22f;
    [SerializeField] private float spawnY = 6f;
    [SerializeField, Min(0.1f)] private float movementSpeed = 6.6f;
    [SerializeField, Min(0.1f)] private float sineSegmentDuration = 1.6666667f;
    [SerializeField, Min(0.1f)] private float straightSegmentDuration = 1.6666667f;
    [SerializeField, Min(0f)] private float sineAmplitude = 1f;

    private readonly List<GameObject> activeDragons = new List<GameObject>();

    private void Start()
    {
        if (dragonPrefab == null || dragonAnimator == null)
        {
            Debug.LogError("Assign DragonRed and DragonMovementAnimator to the Dragons object.", this);
            enabled = false;
            return;
        }

        if (rightEdge <= leftEdge)
        {
            Debug.LogError("The Dragon right edge must be greater than its left edge.", this);
            enabled = false;
            return;
        }

        maximumSpawnDelay = Mathf.Max(minimumSpawnDelay, maximumSpawnDelay);
        StartCoroutine(SpawnDragons());
    }

    private IEnumerator SpawnDragons()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minimumSpawnDelay, maximumSpawnDelay));
            activeDragons.RemoveAll(dragon => dragon == null);

            if (activeDragons.Count >= maximumDragonCount)
                continue;

            bool movingRight = Random.value < 0.5f;
            float startX = movingRight ? leftEdge : rightEdge;
            float endX = movingRight ? rightEdge : leftEdge;

            GameObject dragon = Instantiate(dragonPrefab, transform, false);
            dragon.name = "Dragon";
            dragon.transform.localPosition = new Vector3(startX, spawnY, 0f);

            Animator animator = dragon.GetComponent<Animator>();
            if (animator == null)
                animator = dragon.AddComponent<Animator>();
            animator.runtimeAnimatorController = dragonAnimator;

            MainMenuDragonAnimation flight = dragon.AddComponent<MainMenuDragonAnimation>();
            flight.Initialize(
                startX,
                endX,
                spawnY,
                movementSpeed,
                sineSegmentDuration,
                straightSegmentDuration,
                sineAmplitude);

            activeDragons.Add(dragon);
        }
    }
}
