using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_DialogueController : MonoBehaviour
{
    [Header("Dialogue Data")]
    public string[] DialogueName;
    public string[] DialogueData;
    public string[] DialogueFormatData;
    public string[] DialogueLevels;
    private Dialogue[] dialogues;
    private List<Dictionary<string, DialogueFormat>> allDialogueFormats;

    [Header("Character Data")]
    public string MCDialogueFormatData;
    private DialogueFormat mcFormat;

    public struct DialogueFormat {
        public DialogueFormat(string dialogueFormatData) {
            string[] data = dialogueFormatData.Split(new string[] { "{n}" }, StringSplitOptions.None);
            DialogueString = data[0];
            CharacterName = data[1];
            CharacterPortrait = data[2];
            CharacterFrame = data[3];
            CharacterSpeechSound = data[4];
        }
        public string DialogueString { get; }
        public string CharacterName { get; }
        public string CharacterPortrait { get; }
        public string CharacterFrame { get; }
        public string CharacterSpeechSound { get; }
    }

    public struct Dialogue {
        public Dialogue(Dictionary<string, DialogueFormat> dialogueFormat, string dialogueData) {
            DialogueText = new Dictionary<string, string>();
            DialogueTransitions = new Dictionary<string, List<string>>();
            DialogueStages = new Dictionary<string, List<string>>();
            DialogueFormats = new Dictionary<string, DialogueFormat> {};

            string[] lineData = dialogueData.Split(new string[] { "{n}" }, StringSplitOptions.None);
            for (int line = 0; line < lineData.Length; line++) {
                string data = lineData[line];
                string[] idSplit = null;

                if (data.Contains("{:}")) {
                    idSplit = data.Split(new string[] { "{:}" }, StringSplitOptions.None);
                    
                    DialogueFormats[idSplit[0]] = dialogueFormat[idSplit[0]];
                } else if (data.Contains("{.}")) {
                    idSplit = data.Split(new string[] { "{.}" }, StringSplitOptions.None);
                }

                if (idSplit == null || idSplit.Length <= 1) { break; }

                string[] transitionSplit = idSplit[1].Split(new string[] { "{>}" }, StringSplitOptions.None);
                
                // string transitionsAvaliable = "";
                DialogueTransitions[idSplit[0]] = new List<string>();
                for (int i = 1; i < transitionSplit.Length; i++) {
                    DialogueTransitions[idSplit[0]].Add(transitionSplit[i]);
                    // transitionsAvaliable += $"Transition {transitionSplit[i]} ";
                }
                // Debug.Log($"{idSplit[0]}: {transitionSplit[0]} -> {transitionsAvaliable}");

                string[] stageSplit = transitionSplit[0].Split(new string[] { "~" }, StringSplitOptions.None);

                // string stagesRequired = "";
                DialogueStages[idSplit[0]] = new List<string>();
                for (int i = 0; i < stageSplit.Length -1; i++) {
                    DialogueStages[idSplit[0]].Add(stageSplit[i]);
                    // stagesRequired += $"Stage {stageSplit[i]},";
                }
                // Debug.Log($"{idSplit[0]}: {stagesRequired} required for {stageSplit[1]}");

                DialogueText[idSplit[0]] = stageSplit[stageSplit.Length -1];
            }
        }
        public Dictionary<string, string> DialogueText { get; }
        //DialogueText is the formatted text in a dialogue (note this has to be parsed before being displayed)
        public Dictionary<string, List<string>> DialogueTransitions { get; }
        //DialogueTransitions refers to the next dialogue that should be played after a given dialogue ends
        public Dictionary<string, List<string>> DialogueStages { get; }
        //DialogueStages refers to the stages that this dialogue is present in
        public Dictionary<string, DialogueFormat> DialogueFormats { get; }
        //DialogueFormats refers to the formatData that each dialogue has (note this only contains formats for the enemy speech bubbles)
        //ALSO the string "0" should be and is reserved for the player character.
    }

    void Awake() {
        allDialogueFormats = new List<Dictionary<string, DialogueFormat>>();
        for (int i = 0; i < DialogueFormatData.Length; i++) {
            string[] dialogeFormatSplit = DialogueFormatData[i].Split(new string[] { "{n}" }, StringSplitOptions.None);
            Dictionary<string, DialogueFormat> dialogeFormatDictionary = new Dictionary<string, DialogueFormat>();
            for (int j = 0; j < dialogeFormatSplit.Length; j++) {
                DialogueFormat dialogueFormat = new DialogueFormat(dialogeFormatSplit[j]);
                dialogeFormatDictionary[dialogueFormat.DialogueString] = dialogueFormat;
            }
            allDialogueFormats.Add(dialogeFormatDictionary);
        }

        dialogues = new Dialogue[DialogueData.Length];
        for (int i = 0; i < DialogueData.Length; i++) {
            Dialogue dialogue = new Dialogue(allDialogueFormats[i], DialogueData[i]);
            dialogues[i] = dialogue;
        }

        mcFormat = new DialogueFormat(MCDialogueFormatData);
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