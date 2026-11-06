
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Button))]
public class MainMenuButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Button button;
    private bool isPointerOver;

    private void Awake()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(PlayClickSound);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable || isPointerOver)
            return;

        isPointerOver = true;

        if (MainMenuAudioController.Instance == null)
            return;

        MainMenuAudioController.Instance.PlayButtonHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerOver = false;
    }

    private void PlayClickSound()
    {
        if (!button.interactable)
            return;

        if (MainMenuAudioController.Instance == null)
            return;

        MainMenuAudioController.Instance.PlayButtonClick();
    }

    private void OnDestroy()
    {
        if (button != null)
            button.onClick.RemoveListener(PlayClickSound);
    }
}
