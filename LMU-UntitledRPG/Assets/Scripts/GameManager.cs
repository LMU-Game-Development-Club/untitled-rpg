using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] public RectTransform fader;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (fader != null)
        {
            fader.gameObject.SetActive(true);
            LeanTween.scale(fader, new Vector3(1, 1, 1), 0);
            LeanTween.scale(fader, Vector3.zero, 0.5f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
            {
                fader.gameObject.SetActive(false);
            });
        }
    }


    private void OnDestroy()
    {
        if (Instance == this)
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
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
        fader.gameObject.SetActive(true);
        LeanTween.scale(fader, Vector3.zero, 0f);
        LeanTween.scale(fader, new Vector3(1,1,1), 0.5f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
        });
    }

    private string GetExhibitName(Exhibit exhibit)
    {
        return exhibit.ToString(); // Assuming the scene names match the enum names
    }
}
