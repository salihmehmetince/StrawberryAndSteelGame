
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Act1VideoManager : MonoBehaviour
{
    [SerializeField] private VideoPlayer videoPlayer;
    [SerializeField] private RawImage videoDisplay;
    [SerializeField] private TextMeshProUGUI storyText;
    [SerializeField] private Button continueButton;
    [SerializeField] private ActStoryData actStoryData;
    [SerializeField] private AudioSource backgroundMusic;

    [SerializeField] private string firstLevelSceneName;

    private int currentVideoIndex;

    private void Start()
    {
        videoPlayer.targetTexture = new RenderTexture(
            (int)videoDisplay.rectTransform.rect.width,
            (int)videoDisplay.rectTransform.rect.height,
            0
        );

        videoDisplay.texture = videoPlayer.targetTexture;

        currentVideoIndex = 0;

        ShowCurrentStoryPart();

        continueButton.onClick.AddListener(NextVideo);
    }

    private void ShowCurrentStoryPart()
    {
        if (currentVideoIndex >= actStoryData.storyParts.Length)
        {
            backgroundMusic.Stop();
            SceneManager.LoadScene(firstLevelSceneName);
            return;
        }

        ActStoryData.StoryPart storyPart =
            actStoryData.storyParts[currentVideoIndex];

        storyText.text = storyPart.storyText;

        videoPlayer.Stop();
        videoPlayer.clip = storyPart.video;
        videoPlayer.Play();

        backgroundMusic.Stop();
        backgroundMusic.clip = storyPart.backgroundMusic;
        backgroundMusic.Play();
    }

    private void NextVideo()
    {
        currentVideoIndex++;
        ShowCurrentStoryPart();
    }

    private void OnDestroy()
    {
        continueButton.onClick.RemoveListener(NextVideo);
    }
}
