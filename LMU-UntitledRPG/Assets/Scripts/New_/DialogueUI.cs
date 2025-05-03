using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    [Header("Dialogue Elements")]
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("UI Containers")]
    [SerializeField] private Transform responseContainer;

    [Header("Styling")]
    [SerializeField] private TMP_FontAsset fontAsset;
    [SerializeField] private Sprite buttonBackgroundSprite;
    [SerializeField] private Color buttonBackgroundColor = new Color(1f, 1f, 1f, 0.95f);
    [SerializeField] private Color textColor = Color.black;

    private readonly List<GameObject> activeButtons = new();

    public void ShowLine(DialogueLineData line)
    {
        ClearResponses();
        dialogueText.text = $"{line.CharacterName}: {line.LineText}";
    }

    public void ShowResponses(DialogueLineData[] responses, Action<string> onClick)
    {
        ClearResponses();

        if (responses == null || responses.Length == 0)
        {
            Debug.LogWarning("DialogueUI: No responses provided.");
            return;
        }

        foreach (var line in responses)
        {
            if (line == null || string.IsNullOrWhiteSpace(line.LineID)) continue;

            GameObject button = CreateResponseButton(line, onClick);
            button.transform.SetParent(responseContainer, false);
            activeButtons.Add(button);
        }
    }

    public void ClearResponses()
    {
        foreach (GameObject button in activeButtons)
        {
            if (button != null) Destroy(button);
        }
        activeButtons.Clear();
    }

    private GameObject CreateResponseButton(DialogueLineData line, Action<string> onClick)
    {
        GameObject buttonGO = new GameObject("ResponseButton", typeof(RectTransform), typeof(Image), typeof(Button), typeof(LayoutElement));


        var image = buttonGO.GetComponent<Image>();
        image.sprite = buttonBackgroundSprite;
        image.type = Image.Type.Sliced;
        image.color = buttonBackgroundColor;

        var layout = buttonGO.GetComponent<LayoutElement>();
        layout.minHeight = 60;
        layout.preferredHeight = 80;
        layout.flexibleHeight = 0;
        layout.flexibleWidth = 1;

        var rect = buttonGO.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(0.5f, 1);

     
        GameObject textGO = new GameObject("Text", typeof(RectTransform));
        textGO.transform.SetParent(buttonGO.transform, false);

        var tmp = textGO.AddComponent<TextMeshProUGUI>();
        tmp.text = line.LineText;
        tmp.font = fontAsset;
        tmp.color = textColor;
        tmp.alignment = TextAlignmentOptions.Midline;
        tmp.enableAutoSizing = true;
        tmp.fontSizeMin = 18;
        tmp.fontSizeMax = 32;
        tmp.raycastTarget = false;

        var textRect = tmp.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.1f, 0.2f);  
        textRect.anchorMax = new Vector2(0.9f, 0.8f);
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;

        // Hook up button
        var btn = buttonGO.GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() =>
        {
            string next = line.NextLineIDs.Length > 0 && !string.IsNullOrWhiteSpace(line.NextLineIDs[0])
                ? line.NextLineIDs[0]
                : "end";
            onClick?.Invoke(next);
        });

        return buttonGO;
    }
}
