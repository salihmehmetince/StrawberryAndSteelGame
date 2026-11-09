
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class PlayModeStartSceneSetter
{
    private const string MainMenuSceneName = "MainMenuScene";

    static PlayModeStartSceneSetter()
    {
        SetMainMenuAsStartScene();
    }

    private static void SetMainMenuAsStartScene()
    {
        string[] sceneGuids =
            AssetDatabase.FindAssets(MainMenuSceneName + " t:Scene");

        foreach (string guid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(guid);

            SceneAsset sceneAsset =
                AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);

            if (sceneAsset != null &&
                sceneAsset.name == MainMenuSceneName)
            {
                EditorSceneManager.playModeStartScene = sceneAsset;

                Debug.Log("Play Mode baþlangýç sahnesi: " + MainMenuSceneName);
                return;
            }
        }

        Debug.LogError(
            MainMenuSceneName + " sahnesi bulunamadý!"
        );
    }
}
