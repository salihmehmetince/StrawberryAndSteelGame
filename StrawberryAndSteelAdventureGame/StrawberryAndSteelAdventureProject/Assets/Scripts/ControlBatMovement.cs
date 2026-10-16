using UnityEngine;
using System.Collections;

public class ControlBatMovement : MonoBehaviour
{
    [SerializeField] private float minWaitTime = 3f;
    [SerializeField] private float maxWaitTime = 7f;
    [SerializeField] private float movementSpeed = 5f;

    private bool movingRight;
    private bool isFlying;
    private SpriteRenderer spriteRenderer;
    void Start()
    {

        spriteRenderer = GetComponent<SpriteRenderer>();
        transform.localPosition = new Vector3(
            -22.5f,
            transform.localPosition.y,
            transform.localPosition.z
        );

        StartCoroutine(FlyBat());
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
        }
    }

    IEnumerator FlyBat()
    {
        while (true)
        {
            yield return new WaitForSeconds(
                Random.Range(minWaitTime, maxWaitTime)
            );

            movingRight = !movingRight;
            spriteRenderer.flipX = !movingRight;
            Vector3 position = transform.localPosition;
            position.x = movingRight ? -22.5f : 22.5f;
            transform.localPosition = position;

            isFlying = true;

            yield return new WaitUntil(() => !isFlying);
        }
    }
}