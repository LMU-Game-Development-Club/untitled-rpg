using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UI_DialogueTextClick : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent onTextboxClick;

    public void OnPointerClick(PointerEventData eventData) {
        onTextboxClick.Invoke();
    }

    void Awake() {
        if (onTextboxClick == null) {
            onTextboxClick = new UnityEvent();
        }
    }
}
