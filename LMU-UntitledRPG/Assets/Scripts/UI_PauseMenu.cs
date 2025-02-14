/**
    Name: Wesley Ng (Undalevein)
    Date of Creation: 2/14/2025
    Description: A simple pause menu script within the Pause prefab.
*/


using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_PauseMenu : MonoBehaviour
{
    public GameObject PauseText;
    public Button ResumeButton, ReturnButton;

    private bool gamePaused = false;
    private UnityEvent eventListener = new UnityEvent();
    
    void Start()
    {
        eventListener.AddListener(MyAction);
        ResumeButton.onClick.AddListener(ResumeGame);
        ReturnButton.onClick.AddListener(ReturnToMenu);
        PauseText.gameObject.SetActive(false);
        ResumeButton.gameObject.SetActive(false);
        ReturnButton.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && eventListener != null) 
        {   
            TogglePause();
        }  
    }

    void ResumeGame()
    {
        TogglePause();
    }

    void ReturnToMenu()
    {
        Debug.Log("Returning to Title Screen");
        TogglePause();
        SceneManager.LoadScene(sceneName:"TitleScreen");
    }

    void TogglePause()
    {
        if (gamePaused)
        {
            Debug.Log("Game Resumed...");
            PauseText.gameObject.SetActive(false);
            ResumeButton.gameObject.SetActive(false);
            ReturnButton.gameObject.SetActive(false);
            gamePaused = false;
            Time.timeScale = 1;
        } 
        else 
        {
            Debug.Log("Game Paused...");
            PauseText.gameObject.SetActive(true);
            ResumeButton.gameObject.SetActive(true);
            ReturnButton.gameObject.SetActive(true);
            gamePaused = true;
            Time.timeScale = 0;
        }
    }

    void MyAction()
    {
        //Output message to the console
        Debug.Log("Do Stuff");
    }
}
