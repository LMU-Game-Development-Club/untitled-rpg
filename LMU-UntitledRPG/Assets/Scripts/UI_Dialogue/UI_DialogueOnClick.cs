using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class UI_DialogueOnClick : MonoBehaviour, IPointerClickHandler
{
    public UnityEvent onClick;

    public void OnPointerClick(PointerEventData eventData) {
        onClick.Invoke();
    }

    void Awake() {
        if (onClick == null) {
            onClick = new UnityEvent();
        }
    }
}
