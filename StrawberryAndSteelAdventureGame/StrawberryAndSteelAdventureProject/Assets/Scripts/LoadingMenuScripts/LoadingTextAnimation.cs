using System.Collections;
using TMPro;
using UnityEngine;

public class LoadingTextAnimation : MonoBehaviour
{
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private float animationInterval = 0.4f;

    private Coroutine animationCoroutine;

    private void Awake()
    {
        if (loadingText == null)
            loadingText = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        animationCoroutine = StartCoroutine(AnimateDots());
    }

    private void OnDisable()
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);
    }

    private IEnumerator AnimateDots()
    {
        int dotCount = 0;

        while (true)
        {
            loadingText.text = "Loading" + new string('.', dotCount);

            dotCount = (dotCount + 1) % 4;

            yield return new WaitForSecondsRealtime(animationInterval);
        }
    }
}