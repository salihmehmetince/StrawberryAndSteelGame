using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MainMenuBirdsAnimation : MonoBehaviour
{
    [SerializeField] private Sprite[] birdSprites = new Sprite[5];

    private const float MinWaitTime = 10f;
    private const float MaxWaitTime = 16f;
    private const int MaxBirdCount = 10;

    private readonly Queue<GameObject> birds = new Queue<GameObject>();

    private void Start()
    {
        StartCoroutine(SpawnBirds());
    }

    private IEnumerator SpawnBirds()
    {
        while (true)
        {
            yield return new WaitForSeconds(
                Random.Range(MinWaitTime, MaxWaitTime)
            );

            if (birdSprites == null || birdSprites.Length == 0)
                continue;

            Sprite selectedSprite =
                birdSprites[Random.Range(0, birdSprites.Length)];

            if (selectedSprite == null)
                continue;

            if (birds.Count >= MaxBirdCount)
            {
                GameObject oldestBird = birds.Dequeue();

                if (oldestBird != null)
                    Destroy(oldestBird);
            }

            GameObject birdObject = new GameObject("Bird");

            birdObject.transform.SetParent(transform, false);
            birdObject.transform.localPosition = new Vector3(-5f, 1f, 0f);

            SpriteRenderer spriteRenderer =
                birdObject.AddComponent<SpriteRenderer>();

            spriteRenderer.sprite = selectedSprite;

            birdObject.AddComponent<MainMenuBirdAnimation>();

            birds.Enqueue(birdObject);
        }
    }
}
