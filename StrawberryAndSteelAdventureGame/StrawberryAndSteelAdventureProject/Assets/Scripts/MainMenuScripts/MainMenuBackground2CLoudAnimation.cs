using System.Collections;
using UnityEngine;

public class MainMenuBackground2CLoudAnimation : MonoBehaviour
{
    private const float HorizontalRange = 1f;
    private const float MinWait = 2f;
    private const float MaxWait = 3f;
    private const float MoveDuration = 1f;

    private float startX;

    private void Start()
    {
        startX = transform.localPosition.x;
        StartCoroutine(MoveCloud());
    }

    private IEnumerator MoveCloud()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(MinWait, MaxWait));

            Vector3 startPosition = transform.localPosition;
            float targetX = startX + Random.Range(-HorizontalRange, HorizontalRange);
            Vector3 targetPosition = new Vector3(targetX, startPosition.y, startPosition.z);

            float elapsed = 0f;
            while (elapsed < MoveDuration)
            {
                elapsed += Time.deltaTime;
                transform.localPosition = Vector3.Lerp(startPosition, targetPosition, elapsed / MoveDuration);
                yield return null;
            }

            transform.localPosition = targetPosition;
        }
    }
}