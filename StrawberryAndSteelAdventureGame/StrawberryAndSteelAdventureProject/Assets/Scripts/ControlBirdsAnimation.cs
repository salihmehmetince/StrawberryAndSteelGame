using UnityEngine;
using System.Collections;

public class ControlBirdsAnimation : MonoBehaviour
{
    [SerializeField] private Sprite[] birdSprites = new Sprite[5];
    private float minWaitTime = 10f;
    private float maxWaitTime = 15f;
    private float movementSpeed = 18f;

    private SpriteRenderer spriteRenderer;
    private bool movingRight;
    private bool isFlying;

    [SerializeField] private ParticleSystem birdTrail;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 5;

        transform.localPosition = new Vector3(
            -22.5f,
            4.5f,
            transform.localPosition.z
        );

        spriteRenderer.enabled = false;

        StartCoroutine(FlyBird());

        birdTrail.Stop();
    }

    void Update()
    {
        if (!isFlying)
            return;

        float direction = movingRight ? 1f : -1f;

        transform.localPosition +=
            Vector3.right * direction * movementSpeed * Time.deltaTime;

        if ((movingRight && transform.localPosition.x >= 22.5f) ||
            (!movingRight && transform.localPosition.x <= -22.5f))
        {
            isFlying = false;
            spriteRenderer.enabled = false;
            birdTrail.Stop();
            birdTrail.Clear();
        }
    }

    IEnumerator FlyBird()
    {
        while (true)
        {
            yield return new WaitForSeconds(
                Random.Range(minWaitTime, maxWaitTime)
            );

            if (birdSprites == null || birdSprites.Length == 0)
                continue;

            Sprite selectedSprite =
                birdSprites[Random.Range(0, birdSprites.Length)];

            if (selectedSprite == null)
                continue;

            spriteRenderer.sprite = selectedSprite;

            movingRight = Random.value < 0.5f;
            spriteRenderer.flipX = !movingRight;

            float startX = movingRight ? -22.5f : 22.5f;
            float randomY = Random.Range(4f, 5f);

            transform.localPosition = new Vector3(
                startX,
                randomY,
                transform.localPosition.z
            );

            spriteRenderer.enabled = true;
            birdTrail.Play();
            isFlying = true;

            yield return new WaitUntil(() => !isFlying);
        }
    }
}