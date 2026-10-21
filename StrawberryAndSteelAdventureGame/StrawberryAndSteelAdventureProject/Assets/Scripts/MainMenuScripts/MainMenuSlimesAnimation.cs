using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuSlimesAnimation : MonoBehaviour
{
    [SerializeField] private RuntimeAnimatorController slimeAnimator;

    private const float MinWaitTime = 10f;
    private const float MaxWaitTime = 16f;
    private const int MaxSlimeCount = 10;

    private readonly Queue<GameObject> slimes = new Queue<GameObject>();

    private void Start()
    {
        StartCoroutine(SpawnSlimes());
    }

    private IEnumerator SpawnSlimes()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(MinWaitTime, MaxWaitTime));

            if (slimeAnimator == null)
                continue;

            if (slimes.Count >= MaxSlimeCount)
            {
                GameObject oldestSlime = slimes.Dequeue();
                if (oldestSlime != null)
                    Destroy(oldestSlime);
            }

            GameObject slimeObject = new GameObject("Slime");
            slimeObject.transform.SetParent(transform, false);
            slimeObject.transform.localPosition = new Vector3(-22f, -7f, 0f);
            slimeObject.transform.localScale = new Vector3(5f, 5f, 1f);

            slimeObject.AddComponent<SpriteRenderer>();

            Animator animator = slimeObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = slimeAnimator;

            slimeObject.AddComponent<MainMenuSlimeAnimation>();
            slimes.Enqueue(slimeObject);
        }
    }
}
