using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] public RectTransform fader;
    [SerializeField] private GameObject combatPrefab; 
    private GameObject activeCombat;
    private CanvasGroup combatCanvasGroup;

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

    public void LoadExhibit(Exhibit exhibit)
    {
        string sceneName = GetExhibitName(exhibit);
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogError("Invalid exhibit name.");
        }
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void LoadEx()
    {
        SceneManager.LoadScene("Exhibit1");
    }

    private string GetExhibitName(Exhibit exhibit)
    {
        return exhibit.ToString(); // Assuming the scene names match enum names
    }

    // ------------------- Combat Management -------------------

    public void LoadCombat()
    {
        if (activeCombat == null && combatPrefab != null)
        {
            activeCombat = Instantiate(combatPrefab, transform);
            combatCanvasGroup = activeCombat.GetComponent<CanvasGroup>();
            if (combatCanvasGroup != null)
            {
                combatCanvasGroup.alpha = 0;
                combatCanvasGroup.interactable = false;
                combatCanvasGroup.blocksRaycasts = false;

                LeanTween.alphaCanvas(combatCanvasGroup, 1f, 0.5f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
                {
                    combatCanvasGroup.interactable = true;
                    combatCanvasGroup.blocksRaycasts = true;
                });
            }
        }
    }

    public void EndCombat()
    {
        if (activeCombat != null && combatCanvasGroup != null)
        {
            combatCanvasGroup.interactable = false;
            combatCanvasGroup.blocksRaycasts = false;

            LeanTween.alphaCanvas(combatCanvasGroup, 0f, 0.5f).setEase(LeanTweenType.easeInOutQuad).setOnComplete(() =>
            {
                Destroy(activeCombat);
                activeCombat = null;
            });
        }
    }
}
