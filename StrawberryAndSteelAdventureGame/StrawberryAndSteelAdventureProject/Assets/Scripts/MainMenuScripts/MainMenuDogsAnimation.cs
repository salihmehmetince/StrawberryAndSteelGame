using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuDogsAnimation : MonoBehaviour
{
    [SerializeField] private RuntimeAnimatorController[] dogAnimators = new RuntimeAnimatorController[6];

    private const float MinWaitTime = 10f;
    private const float MaxWaitTime = 16f;
    private const int MaxDogCount = 10;

    private readonly Queue<GameObject> dogs = new Queue<GameObject>();

    private void Start()
    {
        StartCoroutine(SpawnDogs());
    }

    private IEnumerator SpawnDogs()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(MinWaitTime, MaxWaitTime));

            if (dogAnimators == null || dogAnimators.Length == 0)
                continue;

            RuntimeAnimatorController selectedAnimator =
                dogAnimators[Random.Range(0, dogAnimators.Length)];

            if (selectedAnimator == null)
                continue;

            if (dogs.Count >= MaxDogCount)
            {
                GameObject oldestDog = dogs.Dequeue();
                if (oldestDog != null)
                    Destroy(oldestDog);
            }

            GameObject dogObject = new GameObject("Dog");
            dogObject.transform.SetParent(transform, false);
            dogObject.transform.localPosition = new Vector3(-2.75f, 0f, 0f);

            dogObject.AddComponent<SpriteRenderer>();

            Animator animator = dogObject.AddComponent<Animator>();
            animator.runtimeAnimatorController = selectedAnimator;

            dogObject.AddComponent<MainMenuDogAnimation>();
            dogs.Enqueue(dogObject);
        }
    }
}
