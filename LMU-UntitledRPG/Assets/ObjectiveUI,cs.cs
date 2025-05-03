using TMPro;
using UnityEngine;

public class ObjectiveUI : MonoBehaviour
{
    public static ObjectiveUI Instance;

    [SerializeField] private TextMeshProUGUI objectiveText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    public void SetObjective(string newObjective)
    {
        objectiveText.text = $"Objective: {newObjective}";
    }
}
