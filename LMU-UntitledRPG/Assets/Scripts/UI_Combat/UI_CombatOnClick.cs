using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_CombatOnClick : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public UnityEvent onClick;

    public Sprite NotPressed;
    public Sprite Pressed;

    private Image spriteRenderer;
    private Vector3 defaultScale;

    public void OnPointerEnter(PointerEventData eventData) {
        transform.localScale = defaultScale * 1.05f;
    }

    public void OnPointerExit(PointerEventData eventData) {
        transform.localScale = defaultScale;
    }

    public void OnPointerDown(PointerEventData eventData) {
        spriteRenderer.sprite = Pressed;
    }

    public void OnPointerUp(PointerEventData eventData) {
        spriteRenderer.sprite = NotPressed;
        onClick.Invoke();
    }

    void Awake() {
        if (onClick == null) {
            onClick = new UnityEvent();
        }

        spriteRenderer = GetComponent<Image>();
        spriteRenderer.sprite = NotPressed;

        defaultScale = transform.localScale;
    }
}
