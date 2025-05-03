using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    private Dictionary<string, Quest> quests = new Dictionary<string, Quest>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddQuest(string questID, string title, string description)
    {
        if (!quests.ContainsKey(questID))
        {
            quests[questID] = new Quest(questID, title, description);
            Debug.Log($"Quest added: {title}");
        }
    }

    public void CompleteQuest(string questID)
    {
        if (quests.ContainsKey(questID))
        {
            quests[questID].Complete();
            Debug.Log($"Quest completed: {quests[questID].Title}");
        }
    }

    public Quest GetQuest(string questID)
    {
        quests.TryGetValue(questID, out Quest quest);
        return quest;
    }

    public List<Quest> GetAllQuests()
    {
        return new List<Quest>(quests.Values);
    }
}
