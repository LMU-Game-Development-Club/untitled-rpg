using TMPro;
using UnityEngine;

public class UI_CombatCaller : MonoBehaviour
{
    public GameObject StartCombatButton;
    public GameObject StartCanvas;
    public TMP_InputField EnemyNameInputField;
    public UI_Combat CombatScript;
    void Start() {
        StartCombatButton.GetComponent<UI_DialogueOnClick>().onClick.AddListener(() => {
            CombatScript.StartCombat(EnemyNameInputField.text);
            Destroy(StartCanvas);
            Destroy(this);
        });
    }
}
