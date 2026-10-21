using System.Collections;
using UnityEngine;

public class MainMenuSlimeAnimation : MonoBehaviour
{
    private const float MovementSpeed = 5f;
    private const float StartX = -22f;
    private const float EndX = 22f;
    private const float LaneY = -7f;

    private SlimeController slimeController;

    private void Start()
    {
        Animator animator = GetComponent<Animator>();
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator == null || spriteRenderer == null)
        {
            Debug.LogError("MainMenuSlimeAnimation requires an Animator and SpriteRenderer.", this);
            enabled = false;
            return;
        }

        spriteRenderer.sortingOrder = 5;
        slimeController = new SlimeController(
            transform,
            animator,
            spriteRenderer,
            MovementSpeed
        );

        StartCoroutine(WalkAcrossScreen());
    }

    private void Update()
    {
        slimeController?.Tick(Time.deltaTime);
    }

    private IEnumerator WalkAcrossScreen()
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

        slimeController.WalkTo(new Vector2(destinationX, LaneY));

        yield return new WaitUntil(() => !slimeController.IsMoving);
    }
}
