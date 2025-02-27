/**
    Name: Wesley Ng (Undalevein)
    Date of Creation: 2/14/2025
    Description: A simple main menu script within the TitleScreen scene.
*/

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuButtons : MonoBehaviour
{
    public Button PlayButton, SettingsButton, QuitButton;

    void Start()
    {
        PlayButton.onClick.AddListener(PlayGame);
        SettingsButton.onClick.AddListener(OpenSettings);
        QuitButton.onClick.AddListener(QuitGame);
    }

    void PlayGame()
    {
        Debug.Log("Playing the Game");
        SceneManager.LoadScene(sceneName:"Unknown");
    }

    void OpenSettings() 
    {
        Debug.Log("Settings Opened");
    }

    void QuitGame()
    {
        Debug.Log("Quitting Game");
        PlayButton.onClick.RemoveListener(PlayGame);
        SettingsButton.onClick.RemoveListener(OpenSettings);
        QuitButton.onClick.RemoveListener(QuitGame);
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}