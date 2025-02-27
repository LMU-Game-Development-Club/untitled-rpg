using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DialogueController : MonoBehaviour
{
    [Header("Gameobject Refs")]
    public UI_DialogueAssets DialogueFormatAssets;
    public GameObject DialogueCanvas;
    public Image DialoguePortrait;
    public Image DialogueFrame;
    public TextMeshProUGUI DialogueText;
    public UI_DialogueTextClick TextDialogueClick;

    [Header("Dialogue Data")]
    public string[] DialogueName;
    public string[] DialogueData;
    public string[] DialogueFormatData;
    public string[] DialogueLevels;
    private Dialogue[] dialogues;
    private List<Dictionary<string, DialogueFormat>> allDialogueFormats;

    [Header("Character Data")]
    public string MCDialogueFormatData;
    private DialogueFormat mcFormatData;

    private bool nextDialogue;
    private string currentDialogueKey;
    private Dialogue currentDialogue;
    private DialogueFormat currentDialogueFormat;

    public struct DialogueFormat {
        public DialogueFormat(string dialogueFormatData) {
            string[] data = dialogueFormatData.Split(new string[] { ":" }, StringSplitOptions.None);
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
        public override string ToString()
        {
            return $"Name: {CharacterName}, Portrait: {CharacterPortrait}, Frame: {CharacterFrame}, SpeechSound: {CharacterSpeechSound}";
        }
    }

    public struct Dialogue {
        public Dialogue(Dictionary<string, DialogueFormat> dialogueFormat, DialogueFormat mcFormat, string dialogueData) {
            DialogueText = new Dictionary<string, string>();
            DialogueTransitions = new Dictionary<string, List<string>>();
            DialogueStages = new Dictionary<string, List<string>>();
            DialogueFormats = new Dictionary<string, DialogueFormat> {};

            string[] lineData = dialogueData.Split(new string[] { "{n}" }, StringSplitOptions.None);
            for (int line = 0; line < lineData.Length; line++) {
                string data = lineData[line];
                string[] idSplit = null;
                string idString = "none";

                if (data.Contains("{:}")) {
                    idSplit = data.Split(new string[] { "{:}" }, StringSplitOptions.None);
                    idString = idSplit[0].Trim();
                    DialogueFormats[idString] = dialogueFormat[idSplit[0]];
                } else if (data.Contains("{.}")) {
                    idSplit = data.Split(new string[] { "{.}" }, StringSplitOptions.None);
                    idString = idSplit[0].Trim();
                    DialogueFormats[idString] = mcFormat;
                }

                if (idSplit == null || idSplit.Length <= 1) { break; }

                string[] transitionSplit = idSplit[1].Split(new string[] { "{>}" }, StringSplitOptions.None);
                
                // string transitionsAvaliable = "";
                DialogueTransitions[idString] = new List<string>();
                for (int i = 1; i < transitionSplit.Length; i++) {
                    DialogueTransitions[idString].Add(transitionSplit[i]);
                    // transitionsAvaliable += $"Transition {transitionSplit[i]} ";
                }
                // Debug.Log($"{idString}: {transitionSplit[0]} -> {transitionsAvaliable}");

                string[] stageSplit = transitionSplit[0].Split(new string[] { "~" }, StringSplitOptions.None);

                // string stagesRequired = "";
                DialogueStages[idString] = new List<string>();
                for (int i = 0; i < stageSplit.Length -1; i++) {
                    DialogueStages[idString].Add(stageSplit[i]);
                    // stagesRequired += $"Stage {stageSplit[i]},";
                }
                // Debug.Log($"{idString}: {stagesRequired} required for {stageSplit[1]}");

                DialogueText[idString] = stageSplit[stageSplit.Length -1];
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
        public override string ToString() {
            string dialogueTextString = "";
            string dialogueTransitionString = "";
            string dialogueStageString = "";
            string dialogueFormatString = "";

            foreach (string key in DialogueText.Keys) {
                dialogueTextString += $"{key}:  {DialogueText[key]}\n";
            }
            foreach (string key in DialogueTransitions.Keys) {
                string allTransitions = "";
                foreach (string transition in DialogueTransitions[key]) {
                    allTransitions += $"{transition}    ";
                }
                dialogueTransitionString += $"{key}:    {allTransitions}\n";
            }
            foreach (string key in DialogueStages.Keys) {
                string allStages = "";
                foreach (string stage in DialogueStages[key]) {
                    allStages += $"{stage}    ";
                }
                dialogueStageString += $"{key}:    {allStages}\n";
            }
            foreach (string key in DialogueFormats.Keys) {
                dialogueFormatString += $"{key}:    {DialogueFormats[key]}\n";
            }

            return $"----------Dialogue Data----------\n\nDialogues:\n {dialogueTextString}\nTransitions:\n {dialogueTransitionString}\nStages:\n {dialogueStageString}\nFormatting:\n {dialogueFormatString}";
        }
    }

    void Awake() {
        allDialogueFormats = new List<Dictionary<string, DialogueFormat>>();
        for (int i = 0; i < DialogueFormatData.Length; i++) {
            string[] dialogeFormatSplit = DialogueFormatData[i].Split(new string[] { "{n}" }, StringSplitOptions.None);
            Dictionary<string, DialogueFormat> dialogeFormatDictionary = new Dictionary<string, DialogueFormat>();
            for (int j = 0; j < dialogeFormatSplit.Length -1; j++) {
                DialogueFormat dialogueFormat = new DialogueFormat(dialogeFormatSplit[j]);
                dialogeFormatDictionary[dialogueFormat.DialogueString] = dialogueFormat;
            }
            allDialogueFormats.Add(dialogeFormatDictionary);
        }
        mcFormatData = new DialogueFormat(MCDialogueFormatData);

        dialogues = new Dialogue[DialogueData.Length];
        for (int i = 0; i < DialogueData.Length; i++) {
            Dialogue dialogue = new Dialogue(allDialogueFormats[i], mcFormatData, DialogueData[i]);
            dialogues[i] = dialogue;
        }

        nextDialogue = false;
    }

    void Start() {
        TextDialogueClick.onTextboxClick.AddListener(OnTextBoxClick);
        StartDialogue("Phill Introduction");
    }

    private void switchToDialogueKey(Dialogue dialogue, DialogueFormat dialogueFormat, string dialogueKey) {
        int dialoguePortraitIndex = Array.IndexOf(DialogueFormatAssets.CharacterPortraitIds, dialogueFormat.CharacterPortrait);
        DialoguePortrait.sprite = DialogueFormatAssets.CharacterPortraits[dialoguePortraitIndex];
        int dialogueFrameIndex = Array.IndexOf(DialogueFormatAssets.CharacterFrameIds, dialogueFormat.CharacterFrame);
        DialogueFrame.sprite = DialogueFormatAssets.CharacterFrames[dialogueFrameIndex];
        DialogueText.text = dialogue.DialogueText[dialogueKey];
    }

    public void StartDialogue(string dialogueName) {
        for (int i = 0; i < DialogueName.Length; i++) {
            if (dialogueName == DialogueName[i]) {
                StartDialogue(i);
                return;
            }
        }
        Debug.Log($"\"{dialogueName}\" not found in DialogueNames");
    }
    public void StartDialogue(int dialogueIndex) {
        Debug.Log($"Playing Dialogue {DialogueName[dialogueIndex]}");
        Debug.Log(dialogues[dialogueIndex]);

        currentDialogueKey = "start";
        currentDialogue = dialogues[dialogueIndex];
        currentDialogueFormat = dialogues[dialogueIndex].DialogueFormats[currentDialogueKey];

        switchToDialogueKey(currentDialogue, currentDialogueFormat, currentDialogueKey);
    }

    private void OnTextBoxClick() {
        nextDialogue = true;
        Debug.Log("On CLicked");
    }

    void Update() {
        if (nextDialogue) {
            nextDialogue = false;
            currentDialogueKey = currentDialogue.DialogueTransitions[currentDialogueKey][0];
            if (currentDialogueKey == "end") {
                Debug.Log("Dialogue ended");
                DialogueCanvas.SetActive(false);
                return;
            }
            switchToDialogueKey(currentDialogue, currentDialogueFormat, currentDialogueKey);
        }
    }
}