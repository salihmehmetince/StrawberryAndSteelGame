
using UnityEngine;
using System.Collections;

public class ControlCloudAnimation : MonoBehaviour
{
    private float movementRange = 1f;
    private float scaleRange = 0.2f;
    private float movementSpeed = 0.02f;
    private float scaleSpeed = 0.02f;

    private Vector3 initialPosition;
    private Vector3 initialScale;

    private Vector3 targetPosition;
    private Vector3 targetScale;

    void Start()
    {
        initialPosition = transform.position;
        initialScale = transform.localScale;

        StartCoroutine(RandomMovement());
    }

    void Update()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            movementSpeed * Time.deltaTime
        );

        transform.localScale = Vector3.Lerp(
            transform.localScale,
            targetScale,
            scaleSpeed * Time.deltaTime
        );
    }

    IEnumerator RandomMovement()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 5f));

            // %50 ihtimalle hareket et ve boyutunu deðiþtir.
            if (Random.value < 0.5f)
            {
                ChooseNewTargets();
            }
        }
    }

    void ChooseNewTargets()
    {
        targetPosition = new Vector3(
            Random.Range(initialPosition.x - movementRange, initialPosition.x + movementRange),
            Random.Range(initialPosition.y - movementRange, initialPosition.y + movementRange),
            initialPosition.z
        );

        targetScale = new Vector3(
            Random.Range(initialScale.x - scaleRange, initialScale.x + scaleRange),
            Random.Range(initialScale.y - scaleRange, initialScale.y + scaleRange),
            initialScale.z
        );
    }
}
