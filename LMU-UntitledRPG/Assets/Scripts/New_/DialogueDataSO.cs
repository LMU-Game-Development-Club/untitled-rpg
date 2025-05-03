using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueData", menuName = "Dialogue/Dialogue Data")]
public class DialogueDataSO : ScriptableObject {
    public List<DialogueLineData> Lines = new();
}
