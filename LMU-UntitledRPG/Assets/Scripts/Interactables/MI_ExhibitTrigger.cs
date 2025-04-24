using UnityEngine;
using UnityEngine.Events;

public enum Exhibit
{
    None,
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
    public UnityEvent EnterExhibit;
    public Exhibit ExhibitToEnter; // Set this in inspector
    private void OnTriggerEnter(Collider other)
    {
        EnterExhibit.Invoke();
    }
}
