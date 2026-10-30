
using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "ActStoryData", menuName = "Story/Act Story Data")]
public class ActStoryData : ScriptableObject
{
    [System.Serializable]
    public class StoryPart
    {
        public VideoClip video;

        public AudioClip backgroundMusic;

        [TextArea(3, 8)]
        public string storyText;
    }

    public StoryPart[] storyParts;
}
