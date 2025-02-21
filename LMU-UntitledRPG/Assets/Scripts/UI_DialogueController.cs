using System.Collections.Generic;
using UnityEngine;

public class UI_DialogueController : MonoBehaviour
{
    public string[] DialogueName;
    public string[] DialogueData;
    public int[] DialogueLevels;

    private Dialogue[] dialogues;

    public struct Dialogue {
        public Dialogue(string dialogueData) {

            DialogueText = new Dictionary<string, string>();
            DialogueOptions = new Dictionary<string, string>();
        }
        public Dictionary<string, string> DialogueText { get; }
        public Dictionary<string, string> DialogueOptions { get; }
    }

    void Awake() {
        dialogues = new Dialogue[DialogueData.Length];
        for (int i = 0; i < DialogueData.Length; i++) {
            Dialogue dialogue = new Dialogue(DialogueData[i]);
            dialogues[i] = dialogue;
        }
    }

    void Start() {
        StartDialogue("Phill Introduction");
    }

    public void StartDialogue(string characterName) {
        for (int i = 0; i < DialogueName.Length; i++) {
            if (characterName == DialogueName[i]) {
                StartDialogue(i);
                return;
            }
        }
        Debug.Log($"\"{characterName}\" not found in CharacterNames");
    }
    public void StartDialogue(int characterIndex) {
        
    }
}