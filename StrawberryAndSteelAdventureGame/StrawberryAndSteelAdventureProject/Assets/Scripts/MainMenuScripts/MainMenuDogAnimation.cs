using System.Collections;
using UnityEngine;

public class MainMenuDogAnimation : MonoBehaviour
{
    private const float MovementSpeed = 0.625f;
    private const float StartX = -2.75f;
    private const float EndX = 2.75f;
    private const float LaneY = 0f;

    private DogController dogController;

    private void Start()
    {
        Animator animator = GetComponent<Animator>();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null || spriteRenderer == null)
        {
            Debug.LogError("MainMenuDogAnimation requires an Animator and SpriteRenderer.", this);
            enabled = false;
            return;
        }

        spriteRenderer.sortingOrder = 5;
        dogController = new DogController(
            transform,
            animator,
            spriteRenderer,
            MovementSpeed
        );

        StartCoroutine(RunAcrossScreen());
    }

    private void Update()
    {
        dogController?.Tick(Time.deltaTime);
    }

    private IEnumerator RunAcrossScreen()
    {
        yield return new WaitForSeconds(Random.Range(1f, 2f));

        bool movingRight = Random.value < 0.5f;
        float startX = movingRight ? StartX : EndX;
        float destinationX = movingRight ? EndX : StartX;

        transform.localPosition = new Vector3(
            startX,
            LaneY,
            transform.localPosition.z
        );

        dogController.RunTo(new Vector2(destinationX, LaneY));

        yield return new WaitUntil(() => !dogController.IsMoving);
    }
}
