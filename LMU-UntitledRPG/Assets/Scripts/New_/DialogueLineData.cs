using UnityEngine;

[System.Serializable]
public class DialogueLineData {
    public string LineID;
    public string CharacterName;
    [TextArea(2, 5)] public string LineText;
    public bool IsPlayerLine;
    public string[] NextLineIDs;
}
