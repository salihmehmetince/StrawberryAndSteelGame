
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerXPBarController : MonoBehaviour
{
    [Header("XP Slider")]
    [SerializeField] private Slider xpSlider;

    [Header("XP Texts")]
    [SerializeField] private TextMeshProUGUI XPCurrentText;
    [SerializeField] private TextMeshProUGUI XPDestinyText;

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

        characterSession.OnStatsChanged += RefreshXPBar;

        RefreshXPBar();
    }

    private void OnDisable()
    {
        if (characterSession == null)
            return;

        characterSession.OnStatsChanged -= RefreshXPBar;
    }

    private void RefreshXPBar()
    {
        if (characterSession == null)
            return;

        long currentXP = characterSession.CurrentXP;
        long levelStartXP = characterSession.CurrentLevelStartXP;
        long nextLevelXP = characterSession.NextLevelRequiredXP;

        // Maximum level
        if (nextLevelXP <= 0)
        {
            if (xpSlider != null)
                xpSlider.value = 1f;

            if (XPCurrentText != null)
                XPCurrentText.text = currentXP.ToString();

            if (XPDestinyText != null)
                XPDestinyText.text = "MAX";

            return;
        }

        long currentProgressXP = currentXP - levelStartXP;
        long requiredProgressXP = nextLevelXP - levelStartXP;

        float progress = 0f;

        if (requiredProgressXP > 0)
        {
            progress = Mathf.Clamp01(
                (float)currentProgressXP / requiredProgressXP
            );
        }

        if (xpSlider != null)
            xpSlider.value = progress;

        if (XPCurrentText != null)
            XPCurrentText.text = currentProgressXP.ToString();

        if (XPDestinyText != null)
            XPDestinyText.text = requiredProgressXP.ToString();
    }
}
