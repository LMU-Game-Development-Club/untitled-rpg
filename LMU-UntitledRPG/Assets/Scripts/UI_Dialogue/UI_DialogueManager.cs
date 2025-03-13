using UnityEngine;

public class UI_DialogueManager : MonoBehaviour
{
    public static UI_DialogueManager Instance { get; private set; }
    private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
        }
    }
}
