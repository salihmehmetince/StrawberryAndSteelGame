
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerManaBarController : MonoBehaviour
{
    [Header("Mana Bar")]
    [SerializeField] private Image manaFillImage;

    [Header("Mana Percentage")]
    [SerializeField] private TextMeshProUGUI manaPercentageText;

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

        characterSession.OnStatsChanged += RefreshManaBar;

        RefreshManaBar();
    }

    private void OnDisable()
    {
        if (characterSession == null)
            return;

        characterSession.OnStatsChanged -= RefreshManaBar;
    }

    private void RefreshManaBar()
    {
        if (characterSession == null)
            return;

        float manaPercentage = 0f;

        if (characterSession.MaxMana > 0)
        {
            manaPercentage =
                Mathf.Clamp01(
                    (float)characterSession.CurrentMana /
                    characterSession.MaxMana
                );
        }

        if (manaFillImage != null)
            manaFillImage.fillAmount = manaPercentage;

        if (manaPercentageText != null)
        {
            int percentage = Mathf.RoundToInt(
                manaPercentage * 100f
            );

            manaPercentageText.text =
                percentage + "% = " + characterSession.CurrentMana;
        }
    }
}
