using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] public RectTransform fader;
    [SerializeField] public UI_Combat combatSystem;
    public GameObject combat_ui;

    public string combatEnemyName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(combatSystem.transform.parent.gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }

        combatSystem.PlayerLoseCombat.AddListener(() => CombatExitRoutine());
        combatSystem.PlayerWinCombat.AddListener(() => CombatExitRoutine());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (fader != null)
        {
            fader.localScale = Vector3.one;
            fader.gameObject.SetActive(true);

            LeanTween.scale(fader, Vector3.zero, 0.5f)
                .setEase(LeanTweenType.easeInOutQuad)
                .setOnComplete(() => fader.gameObject.SetActive(false));
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

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

    public void LoadCombat(string enemyName)
    {
        combatEnemyName = enemyName;
        StartCoroutine(CombatEnterRoutine());
    }

    public void LoadMainMenu()
    {
        ChangeScene("MainMenu");
    }

    public void LoadExhibit1()
    {
        StartCoroutine(CombatExitRoutine());
    }

    private void ChangeScene(string sceneName)
    {
        StartCoroutine(SceneTransitionRoutine(sceneName));
    }

    private string GetExhibitName(Exhibit exhibit)
    {
        return exhibit.ToString(); // Assuming scene names match enum names
    }

    private IEnumerator SceneTransitionRoutine(string sceneName)
    {
        yield return StartCoroutine(FadeToBlack());
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator CombatEnterRoutine()
    {
        Debug.Log("yipeeeee");
        yield return StartCoroutine(FadeToBlack());

        combat_ui.SetActive(true);
        combatSystem.gameObject.SetActive(true);


        //Here put which combat you wanna start
        // rn there are the following options:
        // Archer
        // Knight
        // Priest
        // Soldier

        yield return null;

        combatSystem.Init();

        combatSystem.StartCombat(combatEnemyName);
        
        yield return new WaitForSeconds(0.3f); // Optional pause
        yield return StartCoroutine(FadeFromBlack());
    }

    private IEnumerator CombatExitRoutine()
    {
        yield return StartCoroutine(FadeToBlack());

        combatSystem.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.3f); // Optional pause
        yield return StartCoroutine(FadeFromBlack());
    }

    private IEnumerator FadeToBlack()
    {
        fader.localScale = Vector3.zero;
        fader.gameObject.SetActive(true);

        bool complete = false;
        LeanTween.scale(fader, Vector3.one, 0.5f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() => complete = true);

        yield return new WaitUntil(() => complete);
    }

    private IEnumerator FadeFromBlack()
    {
        bool complete = false;
        LeanTween.scale(fader, Vector3.zero, 0.5f)
            .setEase(LeanTweenType.easeInOutQuad)
            .setOnComplete(() => {
                fader.gameObject.SetActive(false);
                complete = true;
            });

        yield return new WaitUntil(() => complete);
    }
}
