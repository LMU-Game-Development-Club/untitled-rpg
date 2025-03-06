using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_DialogueController : MonoBehaviour
{
    [Header("Options")]
    public bool DebugMode;

    [Header("Gameobject Refs")]
    public UI_DialogueAssets DialogueFormatAssets;
    public GameObject DialogueCanvas;

    private Image dialoguePortrait;
    private Image dialogueFrame;
    private TextMeshProUGUI dialogueText;
    private GameObject dialogueTextBackground;
    private UI_DialogueOnClick[] textDialogueClick;
    private GameObject dialogueInputField;
    private GameObject dialogueStartButton; 

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
    private string currentDialogueLineKey;
    private int currentDialogueIndex;
    private Dialogue currentDialogue;
    private DialogueFormat currentDialogueFormat;

    private Regex regex;

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
            DialogueStarts = new List<string>();
            DialogueText = new Dictionary<string, string>();
            DialogueTransitions = new Dictionary<string, List<string>>();
            DialogueStages = new Dictionary<string, List<string>>();
            DialogueFormats = new Dictionary<string, DialogueFormat> {};

            string[] lineData = dialogueData.Split(new string[] { "{n}" }, StringSplitOptions.None);
            for (int line = 0; line < lineData.Length; line++) {
                string data = lineData[line];
                string[] idSplit = null;
                string idString = "none";

                if (data.Contains("{;}")) {
                    idSplit = data.Split(new string[] { "{;}" }, StringSplitOptions.None);
                    idString = idSplit[0].Trim();
                    DialogueFormats[idString] = dialogueFormat[idSplit[0]];
                    DialogueStarts.Add(idString);
                } else if (data.Contains("{:}")) {
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
        public List<string> DialogueStarts { get; }
        //The starting points for dialogue
        
        public override string ToString() {
            string dialogueStartString = "";
            string dialogueTextString = "";
            string dialogueTransitionString = "";
            string dialogueStageString = "";
            string dialogueFormatString = "";

            foreach (string key in DialogueStarts) {
                dialogueStartString += $"{key}\n";
            }
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

            return $"----------Dialogue Data----------\n\nDialogue Starts:\n{dialogueStartString}\nDialogues:\n{dialogueTextString}\nTransitions:\n{dialogueTransitionString}\nStages:\n{dialogueStageString}\nFormatting:\n{dialogueFormatString}";
        }
    }

    private void setAllInactive(params GameObject[] objs) {
        foreach (GameObject obj in objs) {
            obj.SetActive(false);
        }
    }

    private void setAllActive(params GameObject[] objs) {
        foreach (GameObject obj in objs) {
            obj.SetActive(true);
        }
    }

    private void setNewDialogueLine(Dialogue dialogue, DialogueFormat dialogueFormat, string dialogueKey) {
        if (DebugMode) {
            Debug.Log($"Set dialogueLine to {dialogueKey}");
        }
        int dialoguePortraitIndex = Array.IndexOf(DialogueFormatAssets.CharacterPortraitIds, dialogueFormat.CharacterPortrait);
        dialoguePortrait.sprite = DialogueFormatAssets.CharacterPortraits[dialoguePortraitIndex];
        int dialogueFrameIndex = Array.IndexOf(DialogueFormatAssets.CharacterFrameIds, dialogueFormat.CharacterFrame);
        dialogueFrame.sprite = DialogueFormatAssets.CharacterFrames[dialogueFrameIndex];
        dialogueText.text = dialogue.DialogueText[dialogueKey];
    }

    private string filterKeyByDialogueLevel(Dialogue dialogue, List<string> transitions, int dialogueIndex) {
        string currentStage = DialogueLevels[dialogueIndex];

        foreach (string transitionKey in transitions) {
            if (transitionKey == "end") { return transitionKey; }
            List<string> transitionStages = dialogue.DialogueStages[transitionKey];
            if (transitionStages.Contains(currentStage)) {
                return transitionKey;
            }
        }

        if (DebugMode) {
            Debug.Log("Key could not be found");
        }
        return null;
    }

    public void StartDialogue(string dialogueName) {
        for (int i = 0; i < DialogueName.Length; i++) {
            if (DebugMode) { Debug.Log($"Comparing \"{dialogueName}\" and \"{DialogueName[i]}\""); }
            
            string a = regex.Replace(dialogueName, "");
            string b = regex.Replace(DialogueName[i], "");

            if (a == b) {
                StartDialogue(i);
                return;
            }
        }
        if (DebugMode) { Debug.Log($"\"{dialogueName}\" not found in DialogueNames"); }
    }
    public void StartDialogue(int dialogueIndex) {
        Dialogue dialogue = dialogues[dialogueIndex];
        
        if (DebugMode) {
            Debug.Log($"Playing Dialogue {DialogueName[dialogueIndex]}");
            Debug.Log(dialogues[dialogueIndex]);

            setAllInactive(dialogueInputField, dialogueStartButton);
        }
        setAllActive(dialoguePortrait.gameObject, dialogueFrame.gameObject, dialogueText.gameObject, dialogueTextBackground);

        currentDialogueLineKey = filterKeyByDialogueLevel(dialogue, dialogue.DialogueStarts, dialogueIndex);
        currentDialogueIndex = dialogueIndex;
        currentDialogue = dialogue;
        currentDialogueFormat = dialogue.DialogueFormats[currentDialogueLineKey];

        setNewDialogueLine(currentDialogue, currentDialogueFormat, currentDialogueLineKey);
    }

    void Awake() {
        regex = new Regex(@"\s|[:;,'""\\?]|\p{C}");

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

        dialoguePortrait = DialogueCanvas.transform.Find("DialoguePortrait").GetComponent<Image>();
        dialogueFrame = DialogueCanvas.transform.Find("DialogueFrame").GetComponent<Image>();
        dialogueText = DialogueCanvas.transform.Find("DialogueText").GetComponent<TextMeshProUGUI>();
        dialogueTextBackground = DialogueCanvas.transform.Find("TextBackground").gameObject;
        textDialogueClick = DialogueCanvas.transform.GetComponentsInChildren<UI_DialogueOnClick>();
        dialogueInputField = DialogueCanvas.transform.Find("DialogueNameInputField").gameObject;
        dialogueStartButton = DialogueCanvas.transform.Find("PlayDialogueButton").gameObject;

        if (!DebugMode) {
            setAllInactive(dialogueInputField, dialogueStartButton, DialogueCanvas);
        } else {
            setAllActive(dialogueInputField, dialogueStartButton, DialogueCanvas);
            setAllInactive(dialoguePortrait.gameObject, dialogueFrame.gameObject, dialogueText.gameObject, dialogueTextBackground);
        }
    }

    void Start() {
        foreach (UI_DialogueOnClick clicker in textDialogueClick) {
            if (clicker.gameObject.name == "PlayDialogueButton") { continue; }
            clicker.onClick.AddListener(() => nextDialogue = true);
        }

        if (DebugMode) {
            dialogueStartButton.GetComponent<UI_DialogueOnClick>().onClick.AddListener(() => StartDialogue(dialogueInputField.transform.Find("Text Area").Find("Text").GetComponent<TextMeshProUGUI>().text));
        }
    }

    void Update() {
        if (nextDialogue) {
            nextDialogue = false;
            currentDialogueLineKey = filterKeyByDialogueLevel(currentDialogue, currentDialogue.DialogueTransitions[currentDialogueLineKey], currentDialogueIndex);
            if (currentDialogueLineKey == "end") {
                if (DebugMode) {
                    Debug.Log("Dialogue ended");
                    setAllActive(dialogueInputField, dialogueStartButton);
                    setAllInactive(dialoguePortrait.gameObject, dialogueFrame.gameObject, dialogueText.gameObject, dialogueTextBackground);
                } else { 
                    setAllInactive(DialogueCanvas);
                }
                return;
            }
            setNewDialogueLine(currentDialogue, currentDialogueFormat, currentDialogueLineKey);
        }
    }
}