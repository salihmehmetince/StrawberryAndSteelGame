
using UnityEngine;
using System.Collections;

public class MainMenuBatAnimation : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float startX = -22f;
    [SerializeField] private float endX = 22f;
    [SerializeField] private float minY = 4f;
    [SerializeField] private float maxY = 5f;
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float minWaitTime = 10f;
    [SerializeField] private float maxWaitTime = 14f;

    [Header("References")]
    [SerializeField] private Animator batAnimator;
    [SerializeField] private SpriteRenderer batSpriteRenderer;

    private BatController batController;

    private void Awake()
    {
        if (batAnimator == null)
            batAnimator = GetComponent<Animator>();

        if (batSpriteRenderer == null)
            batSpriteRenderer = GetComponent<SpriteRenderer>();

        batController = new BatController(
            transform,
            batAnimator,
            batSpriteRenderer,
            movementSpeed
        );
    }

    private void Start()
    {
        StartCoroutine(AnimateBat());
    }

    private void Update()
    {
        batController.Tick(Time.deltaTime);
    }

    private IEnumerator AnimateBat()
    {
        while (true)
        {
            bool moveLeftToRight = Random.value < 0.5f;

            float startXPosition = moveLeftToRight ? startX : endX;
            float destinationX = moveLeftToRight ? endX : startX;

            float startY = Random.Range(minY, maxY);

            transform.localPosition = new Vector3(
                startXPosition,
                startY,
                transform.localPosition.z
            );

            float destinationY = Random.Range(minY, maxY);

            Vector2 destination = new Vector2(
                destinationX,
                destinationY
            );

            if (Random.value < 0f)
            {
                batController.FlyTo(destination);
            }
            else
            {
                batController.FlyWithSomersaultTo(destination);
            }

            yield return new WaitUntil(
                () => !batController.IsMoving
            );

            yield return new WaitForSeconds(
                Random.Range(minWaitTime, maxWaitTime)
            );
        }
    }
}
