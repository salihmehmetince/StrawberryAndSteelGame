using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuWitchesAnimation : MonoBehaviour
{
    [SerializeField] private RuntimeAnimatorController witch1Animator;
    [SerializeField] private RuntimeAnimatorController witch2Animator;

    private const float MinWaitTime = 10f;
    private const float MaxWaitTime = 16f;
    private const float StartX = -22f;
    private const float EndX = 22f;
    private const float MinY = 4f;
    private const float MaxY = 5f;
    private const int MaxWitchCount = 5;

    private readonly Queue<GameObject> witches = new Queue<GameObject>();

    private void Start()
    {
        StartCoroutine(SpawnWitches());
    }

    private IEnumerator SpawnWitches()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(MinWaitTime, MaxWaitTime));
            RuntimeAnimatorController selectedAnimator = Random.value < 0.5f
                ? witch1Animator
                : witch2Animator;
            if (selectedAnimator == null) continue;

            if (witches.Count >= MaxWitchCount)
            {
                GameObject oldestWitch = witches.Dequeue();
                if (oldestWitch != null) Destroy(oldestWitch);
            }

            bool movingRight = Random.value < 0.5f;
            float startY = Random.Range(MinY, MaxY);
            float destinationY = Random.Range(MinY, MaxY);
            GameObject witchObject = new GameObject(
                selectedAnimator == witch1Animator ? "Witch1" : "Witch2");
            witchObject.transform.SetParent(transform, false);
            witchObject.transform.localPosition = new Vector3(
                movingRight ? StartX : EndX, startY, 0f);
            witchObject.transform.localScale = new Vector3(2f, 2f, 1f);

            SpriteRenderer spriteRenderer = witchObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sortingOrder = 5;
            Animator animator = witchObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = selectedAnimator;
            MainMenuWitchAnimation witchAnimation =
                witchObject.AddComponent<MainMenuWitchAnimation>();
            witchAnimation.Initialize(movingRight, startY, destinationY);
            witches.Enqueue(witchObject);
        }
    }
}
