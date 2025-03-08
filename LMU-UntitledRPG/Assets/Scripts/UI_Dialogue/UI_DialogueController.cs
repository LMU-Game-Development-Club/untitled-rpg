using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NUnit.Framework;
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
<<<<<<< HEAD
    public GameObject DialogueResponsePrefab;
=======
>>>>>>> 19c23e88baf9325a7362b70299ea3e8c0a08beb3

    private Image dialoguePortrait;
    private Image dialogueFrame;
    private TextMeshProUGUI dialogueText;
    private GameObject dialogueTextBackground;
    private UI_DialogueOnClick[] textDialogueClick;
    private GameObject dialogueInputField;
    private GameObject dialogueStartButton; 

    [Header("Dialogue Data")]
    public string[] DialogueNames;
    public string[] DialogueData;
    public string[] DialogueFormatData;
    public string[] DialogueStages;
    private Dialogue[] dialogues;
    private List<Dictionary<string, DialogueFormat>> allDialogueFormats;

<<<<<<< HEAD

    private string mcDialogueFormatData = "1:CharacterName:CharacterPortrait:CharacterFrame:CharacterSpeechSound{n} 2:CharacterName:CharacterPortrait:CharacterFrame:CharacterSpeechSound{n} 3:CharacterName:CharacterPortrait:CharacterFrame:CharacterSpeechSound{n} ";
    private DialogueFormat mcFormatData;

=======
    [Header("Character Data")]
    public string MCDialogueFormatData;
    private DialogueFormat mcFormatData;


>>>>>>> 19c23e88baf9325a7362b70299ea3e8c0a08beb3
    private bool nextDialogue;
    private Dialogue currentDialogue;
    private string currentDialogueLineKey;
    private int currentDialogueIndex;
    private DialogueFormat currentDialogueLineFormat;

<<<<<<< HEAD
    private List<GameObject> activeDialogueResponseButtons;
    private bool responseClicked;
    private bool responsesSet;

=======
>>>>>>> 19c23e88baf9325a7362b70299ea3e8c0a08beb3
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
            DialogueText = new Dictionary<string, string>();
            DialogueTransitions = new Dictionary<string, List<string>>();
            DialogueStages = new Dictionary<string, List<string>>();
            DialogueFormats = new Dictionary<string, DialogueFormat> {};
            DialogueStarts = new List<string>();
            DialogueResponses = new HashSet<string>();

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
                    DialogueResponses.Add(idString);
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
        public HashSet<string> DialogueResponses { get; }
        //Simply to keep track of which keys are responses
        
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
<<<<<<< HEAD
        if (DebugMode) { Debug.Log($"Set dialogueLine to {dialogueKey}"); }
=======
        if (DebugMode) {
            Debug.Log($"Set dialogueLine to {dialogueKey}");
        }
>>>>>>> 19c23e88baf9325a7362b70299ea3e8c0a08beb3
        int dialoguePortraitIndex = Array.IndexOf(DialogueFormatAssets.CharacterPortraitIds, dialogueFormat.CharacterPortrait);
        dialoguePortrait.sprite = DialogueFormatAssets.CharacterPortraits[dialoguePortraitIndex];
        int dialogueFrameIndex = Array.IndexOf(DialogueFormatAssets.CharacterFrameIds, dialogueFormat.CharacterFrame);
        dialogueFrame.sprite = DialogueFormatAssets.CharacterFrames[dialogueFrameIndex];
        dialogueText.text = dialogue.DialogueText[dialogueKey];
    }

    private void setResponses(Dialogue dialogue, List<string> dialogueResponseKeys) {
<<<<<<< HEAD
        if (DebugMode) { Debug.Log($"Setting responses for {string.Join(", ", dialogueResponseKeys)}"); }
        Vector3 buttonPosition = new Vector3(556.996f, 18f, 0f);
        float yOffsetIncrement = 120f;
        foreach(string responseKey in dialogueResponseKeys) {
            GameObject newDialogueResponseButton = Instantiate(DialogueResponsePrefab);
            newDialogueResponseButton.transform.parent = DialogueCanvas.transform;
            newDialogueResponseButton.transform.localPosition = buttonPosition;
            newDialogueResponseButton.transform.Find("Text").gameObject.GetComponent<TextMeshProUGUI>().text = dialogue.DialogueText[responseKey];
            newDialogueResponseButton.GetComponent<UI_DialogueOnClick>().onClick.AddListener(() => { currentDialogueLineKey = responseKey; responseClicked = true; });
            activeDialogueResponseButtons.Add(newDialogueResponseButton);

            buttonPosition.y += yOffsetIncrement;
        }
        responsesSet = true;
=======
        
>>>>>>> 19c23e88baf9325a7362b70299ea3e8c0a08beb3
    }

    private List<string> filterKeyByDialogueLevel(Dialogue dialogue, List<string> transitions, int dialogueIndex) {
        string currentStage = DialogueStages[dialogueIndex];
        List<string> filteredResponses = new List<string>();

        foreach (string transitionKey in transitions) {
            if (transitionKey == "end") { return new List<string>(){"end"}; }

            List<string> transitionStages = dialogue.DialogueStages[transitionKey];
            if (transitionStages.Contains(currentStage)) {
                filteredResponses.Add(transitionKey);
            }
        }

        return filteredResponses;
    }

    public void StartDialogue(string dialogueName) {
        for (int i = 0; i < DialogueNames.Length; i++) {
<<<<<<< HEAD
=======
            if (DebugMode) { Debug.Log($"Comparing \"{dialogueName}\" and \"{DialogueNames[i]}\""); }
            
>>>>>>> 19c23e88baf9325a7362b70299ea3e8c0a08beb3
            string a = regex.Replace(dialogueName, "");
            string b = regex.Replace(DialogueNames[i], "");

            if (a == b) {
                StartDialogue(i);
                return;
            }
        }
        if (DebugMode) { Debug.Log($"\"{dialogueName}\" not found in DialogueNames"); }
    }
    public void StartDialogue(int dialogueIndex) {
        
        if (DebugMode) {
            Debug.Log($"Playing Dialogue {DialogueNames[dialogueIndex]}");
            Debug.Log(dialogues[dialogueIndex]);

            setAllInactive(dialogueInputField, dialogueStartButton);
        }
        setAllActive(dialoguePortrait.gameObject, dialogueFrame.gameObject, dialogueText.gameObject, dialogueTextBackground);

        nextDialogue = false;
        currentDialogue = dialogues[dialogueIndex];
        currentDialogueIndex = dialogueIndex;
        List<string> startingDialogues = filterKeyByDialogueLevel(currentDialogue, currentDialogue.DialogueStarts, dialogueIndex);
        if (startingDialogues.Count == 0) {
            throw new Exception($"No starting lines found in {DialogueNames[dialogueIndex]} current dialogue stage {DialogueStages[dialogueIndex]}");
        }
        currentDialogueLineKey = startingDialogues[0];
        currentDialogueLineFormat = currentDialogue.DialogueFormats[currentDialogueLineKey];

        setNewDialogueLine(currentDialogue, currentDialogueLineFormat, currentDialogueLineKey);
    }

<<<<<<< HEAD
    private void onNextDialogue() {
        if (DebugMode) { Debug.Log($"Moving onto next dialogue from key {currentDialogueLineKey}"); }
        nextDialogue = false;
        List<string> filteredTransitions = filterKeyByDialogueLevel(currentDialogue, currentDialogue.DialogueTransitions[currentDialogueLineKey], currentDialogueIndex);

        if (filteredTransitions.Count == 0) { throw new Exception($"No valid transitions found for Dialogue {DialogueNames[currentDialogueIndex]} line {currentDialogueLineKey}"); }
        if (filteredTransitions[0] == "end") { 
            if (DebugMode) {
                Debug.Log("Dialogue ended");
                setAllActive(dialogueInputField, dialogueStartButton);
                setAllInactive(dialoguePortrait.gameObject, dialogueFrame.gameObject, dialogueText.gameObject, dialogueTextBackground);
            } else { 
                setAllInactive(DialogueCanvas);
            }    
            return;
        }

        bool isResponse = true;
        foreach (string key in filteredTransitions) {
            isResponse = isResponse && currentDialogue.DialogueResponses.Contains(key);
        }

        if (!isResponse) {
            currentDialogueLineKey = filteredTransitions[0];
            currentDialogueLineFormat = currentDialogue.DialogueFormats[currentDialogueLineKey];
            setNewDialogueLine(currentDialogue, currentDialogueLineFormat, currentDialogueLineKey);
        } else {
            setResponses(currentDialogue, filteredTransitions);
        }
    }

    private void onResponseClick() {
        if (DebugMode) { Debug.Log($"Response Clicked: {currentDialogueLineKey}"); }
        responseClicked = false;
        if (activeDialogueResponseButtons.Count != 0) {
            foreach (GameObject responseButton in activeDialogueResponseButtons) {
                responseButton.GetComponent<UI_DialogueOnClick>().onClick.RemoveAllListeners();
                Destroy(responseButton);
            }
        }
        activeDialogueResponseButtons.Clear();
        responsesSet = false;

        onNextDialogue();
    }

    void Awake() {
        regex = new Regex(@"\s|[:;,'""\\?]|\p{C}");

        activeDialogueResponseButtons = new List<GameObject>();

=======
    void Awake() {
        regex = new Regex(@"\s|[:;,'""\\?]|\p{C}");

>>>>>>> 19c23e88baf9325a7362b70299ea3e8c0a08beb3
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
<<<<<<< HEAD
        mcFormatData = new DialogueFormat(mcDialogueFormatData);
=======
        mcFormatData = new DialogueFormat(MCDialogueFormatData);
>>>>>>> 19c23e88baf9325a7362b70299ea3e8c0a08beb3

        dialogues = new Dialogue[DialogueData.Length];
        for (int i = 0; i < DialogueData.Length; i++) {
            Dialogue dialogue = new Dialogue(allDialogueFormats[i], mcFormatData, DialogueData[i]);
            dialogues[i] = dialogue;
        }

        nextDialogue = false;
<<<<<<< HEAD
        responseClicked = false;
        responsesSet = false;
=======
>>>>>>> 19c23e88baf9325a7362b70299ea3e8c0a08beb3

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
<<<<<<< HEAD
        if (!responsesSet && nextDialogue) { onNextDialogue(); }
        else if (responseClicked) { onResponseClick(); }
=======
        if (nextDialogue) {
            nextDialogue = false;
            List<string> filteredTransitions = filterKeyByDialogueLevel(currentDialogue, currentDialogue.DialogueTransitions[currentDialogueLineKey], currentDialogueIndex);

            if (filteredTransitions.Count == 0) { throw new Exception($"No valid transitions found for Dialogue {DialogueNames[currentDialogueIndex]} line {currentDialogueLineKey}"); }
            if (filteredTransitions[0] == "end") { 
                if (DebugMode) {
                    Debug.Log("Dialogue ended");
                    setAllActive(dialogueInputField, dialogueStartButton);
                    setAllInactive(dialoguePortrait.gameObject, dialogueFrame.gameObject, dialogueText.gameObject, dialogueTextBackground);
                } else { 
                    setAllInactive(DialogueCanvas);
                }    
                return;
            }

            bool isResponse = true;
            foreach (string key in filteredTransitions) {
                isResponse = isResponse && currentDialogue.DialogueResponses.Contains(key);
            }

            if (!isResponse) {
                currentDialogueLineKey = filteredTransitions[0];
                currentDialogueLineFormat = currentDialogue.DialogueFormats[currentDialogueLineKey];
                setNewDialogueLine(currentDialogue, currentDialogueLineFormat, currentDialogueLineKey);
            } else {
                setResponses(currentDialogue, filteredTransitions);
            }
        }
>>>>>>> 19c23e88baf9325a7362b70299ea3e8c0a08beb3
    }
}

/*
TODO
<<<<<<< HEAD
=======
- dialogue responses
>>>>>>> 19c23e88baf9325a7362b70299ea3e8c0a08beb3
- addSound
- textEffects by making sure each character if its own Textmeshpro thingy
- character pooling maybe if it is too laggy
- check at the start to make sure dialoguedata/dialogueformatdata is valid
- tool to make dialogueData and dialogueFormats
- custom talking animations!??!?
<<<<<<< HEAD
=======

Um do this
- why tf did u add mcFormatData the player isnt supposed to fucking be shown in the dialogue
>>>>>>> 19c23e88baf9325a7362b70299ea3e8c0a08beb3
*/