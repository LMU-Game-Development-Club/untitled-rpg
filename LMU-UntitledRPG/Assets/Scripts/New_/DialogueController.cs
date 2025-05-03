using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class DialogueController : MonoBehaviour {
    [SerializeField] private DialogueDataSO dialogueData;
    [SerializeField] private DialogueUI ui;
    [SerializeField] private float autoAdvanceDelay = 0.5f;

    public UnityEvent OnDialogueStart;
    public UnityEvent OnDialogueEnd;

    private Dictionary<string, DialogueLineData> lineLookup;

    public bool hasCombat;
    public string enemyName;

    private void Start() {
        if (dialogueData == null || dialogueData.Lines.Count == 0) {
            Debug.LogError("DialogueController: Dialogue data not assigned or empty.");
            return;
        }

        lineLookup = dialogueData.Lines
            .Where(line => !string.IsNullOrWhiteSpace(line.LineID))
            .ToDictionary(line => line.LineID, line => line);

        ui.gameObject.SetActive(false);
    }

    public void Play(string startID) {
        if (!lineLookup.ContainsKey(startID)) {
            Debug.LogError($"DialogueController: Start line ID \"{startID}\" not found.");
            return;
        }

        ui.gameObject.SetActive(true);
        OnDialogueStart?.Invoke();
        ShowLine(startID);
    }

    private void ShowLine(string id) {
        if (!lineLookup.TryGetValue(id, out var line)) return;

        ui.ShowLine(line);

        if (line.NextLineIDs == null || line.NextLineIDs.Length == 0 || line.NextLineIDs[0] == "end") {
            StartCoroutine(CloseDialogue());
            return;
        }

        var nextLines = line.NextLineIDs
            .Where(n => lineLookup.ContainsKey(n))
            .Select(n => lineLookup[n])
            .ToArray();

        if (nextLines.All(l => l.IsPlayerLine)) {
            ui.ShowResponses(nextLines, ShowLine);
        } else {
            StartCoroutine(AutoAdvance(nextLines));
        }
    }

    private IEnumerator AutoAdvance(DialogueLineData[] lines) {
        yield return new WaitForSeconds(autoAdvanceDelay);
        ShowLine(lines[0].LineID);
    }

    private IEnumerator CloseDialogue() {
        yield return new WaitForSeconds(1f);
        ui.ClearResponses();
        ui.gameObject.SetActive(false);
        if(hasCombat){
            GameManager.Instance.LoadCombat(enemyName);
        }

        OnDialogueEnd?.Invoke();
    }
}
