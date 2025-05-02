using UnityEngine;
using UnityEngine.Events;

public enum Exhibit
{
    Exhibit1,
    Exhibit2,
    Exhibit3,
    Exhibit4,
    Exhibit5,
    Exhibit6,
    Exhibit7,
    Exhibit8,
    Exhibit9,
    Exhibit10
}

public class UI_ExhibitTrigger : MonoBehaviour
{
    public Exhibit ExhibitToEnter; // Set this in inspector
    private UnityEvent EnterExhibit;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            EnterExhibit.Invoke();
        }
    }

    private void Awake()
    {
        if (EnterExhibit == null)
        {
            EnterExhibit = new UnityEvent();
        }
    }

    private void Start()
    {
        EnterExhibit.AddListener(() => {
            GameManager.Instance.LoadExhibit(ExhibitToEnter);
        });
    }
}
