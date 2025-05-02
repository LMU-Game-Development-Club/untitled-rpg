using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Use to load exhibits
    public void LoadExhibit(Exhibit exhibit)
    {
        string sceneName = GetExhibitName(exhibit);
        if (!string.IsNullOrEmpty(sceneName))
        {
            ChangeScene(sceneName);
        }
        else
        {
            Debug.LogError("Invalid exhibit name.");
        }
    }

    public void LoadCombat()
    {
        ChangeScene("Combat");
    }

    public void LoadMainMenu()
    {
        ChangeScene("MainMenu");
    }

    public void LoadEx(){
        ChangeScene("Exhibit1");
    }

    private void ChangeScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    private string GetExhibitName(Exhibit exhibit)
    {
        return exhibit.ToString(); // Assuming the scene names match the enum names
    }
}
