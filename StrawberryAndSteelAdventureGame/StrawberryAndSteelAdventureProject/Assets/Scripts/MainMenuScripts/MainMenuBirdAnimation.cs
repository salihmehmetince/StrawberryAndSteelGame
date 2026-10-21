
using UnityEngine;
using System.Collections;

public class MainMenuBirdAnimation : MonoBehaviour
{
    private BirdControl birdControl;

    private const float MovementSpeed = 5f;
    private const float StartX = -5f;
    private const float EndX = 5f;
    private const float MinY = 1f;
    private const float MaxY = 2f;

    private void Start()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sortingOrder = 5;

        birdControl = new BirdControl(transform, spriteRenderer)
        {
            MovementSpeed = MovementSpeed,
            StartX = StartX,
            EndX = EndX,
            MinY = MinY,
            MaxY = MaxY,
            SineWaveAmplitude = 1f / 3f,
            SineWaveFrequency = 2f
        };

        StartCoroutine(FlyBird());
    }

    private void Update()
    {
        birdControl?.Tick(Time.deltaTime);
    }

    private IEnumerator FlyBird()
    {
        yield return new WaitForSeconds(Random.Range(1f, 2f));

        bool movingRight = Random.value < 0.5f;

        float startX = movingRight ? StartX : EndX;
        float destinationX = movingRight ? EndX : StartX;

        float startY = Random.Range(MinY, MaxY);
        float destinationY = Random.Range(MinY, MaxY);

        transform.localPosition = new Vector3(
            startX,
            startY,
            transform.localPosition.z
        );

        Vector2 destination = new Vector2(
            destinationX,
            destinationY
        );

        int movementType = Random.Range(0, 3);

        switch (movementType)
        {
            case 0:
                birdControl.FlyTo(destination);
                break;

            case 1:
                birdControl.FlyWithCircleTo(destination);
                break;

            case 2:
                birdControl.FlyWithSineWaveTo(destination);
                break;
        }

        yield return new WaitUntil(() => !birdControl.IsFlying);
    }
}
