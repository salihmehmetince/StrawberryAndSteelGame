
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealthBarController : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] private Image healthFillImage;

    [Header("Health Percentage")]
    [SerializeField] private TextMeshProUGUI healthPercentageText;

    private PlayerCharacterSession characterSession;

    private void OnEnable()
    {
        GameBootstrapper bootstrapper = GameBootstrapper.Instance;

        if (bootstrapper == null ||
            bootstrapper.PlayerCharacterSession == null)
        {
            Debug.LogError("Player character session not found.");
            return;
        }

        characterSession = bootstrapper.PlayerCharacterSession;

        characterSession.OnStatsChanged += RefreshHealthBar;

        RefreshHealthBar();
    }

    private void OnDisable()
    {
        if (characterSession == null)
            return;

        characterSession.OnStatsChanged -= RefreshHealthBar;
    }

    private void RefreshHealthBar()
    {
        if (characterSession == null)
            return;

        float healthPercentage = 0f;

        if (characterSession.MaxHealth > 0)
        {
            healthPercentage =
                Mathf.Clamp01(
                    (float)characterSession.CurrentHealth /
                    characterSession.MaxHealth
                );
        }

        if (healthFillImage != null)
            healthFillImage.fillAmount = healthPercentage;

        if (healthPercentageText != null)
        {
            int percentage = Mathf.RoundToInt(
                healthPercentage * 100f
            );

            healthPercentageText.text =
                percentage + "% = " + characterSession.CurrentHealth;
        }
    }
}
