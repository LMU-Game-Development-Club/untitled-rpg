using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    private Quest activeQuest;
    public TextMeshProUGUI questText;  
    [SerializeField] public List<Quest> quests;
    private int currentQuestIndex = 0;

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
    private void Start()
    {
        questText.text = quests[currentQuestIndex].description;
    }

    public void UpdateQuest(){
        currentQuestIndex++;
        questText.text = quests[currentQuestIndex].description;
    }
}