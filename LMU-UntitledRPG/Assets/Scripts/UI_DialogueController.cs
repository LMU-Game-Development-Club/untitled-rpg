using System;
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
            DialogueOptions = new Dictionary<string, string[]>();
            DialogueTransitions = new Dictionary<string, string[]>();
            DialogueStages = new Dictionary<string, int[]>();

            string[] lineData = dialogueData.Split(new string[] { "{n}" }, StringSplitOptions.None);
            for (int line = 0; line < lineData.Length; line++) {
                string data = lineData[line];
                string[] idSplit = null;
                if (data.Contains("{:}")) {
                    idSplit = data.Split(new string[] { "{:}" }, StringSplitOptions.None);
                } else if (data.Contains("{.}")) {
                    idSplit = data.Split(new string[] { "{.}" }, StringSplitOptions.None);
                }

                if (idSplit == null || idSplit.Length <= 1) { break; }

                string[] transitionSplit = idSplit[1].Split(new string[] { "{>}" }, StringSplitOptions.None);
                
                // Debug.Log($"{idSplit[0]}: {transitionSplit[0]} -> {transitionSplit[1]}");
            }
        }
        public Dictionary<string, string> DialogueText { get; }
        //DialogueText is the formatted text in a dialogue (note this has to be parsed before being displayed)
        public Dictionary<string, string[]> DialogueOptions { get; }
        //DialogueOptions refers to the responses avaliable to a given dialogue
        public Dictionary<string, string[]> DialogueTransitions { get; }
        //DialogueTransitions refers to the next dialogue that should be played after a given dialogue ends
        public Dictionary<string, int[]> DialogueStages { get; }
        //DialogueStages refers to the stages that this dialogue is present in
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
        throw new NotImplementedException();
    }
}