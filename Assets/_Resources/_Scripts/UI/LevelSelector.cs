using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    private int levelToLoadIndex; // Private variable to store the level index

    public void SelectLevel(int levelIndex)
    {
        levelToLoadIndex = levelIndex; // Store the level index when a level is selected
        SceneManager.LoadSceneAsync("MainGameScene", LoadSceneMode.Single);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ensure the scene loaded is the game scene
        if (scene.name == "MainGameScene")
        {
            // Use the stored level index
            GameManager.Instance.StartGameAtLevel(levelToLoadIndex);
            SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe to avoid this being called again unnecessarily
        }
    }
}
