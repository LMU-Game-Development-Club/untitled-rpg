using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Quest : MonoBehaviour
{
    public string title;
    [TextArea] public string description;
    public bool isCompleted;
    public bool isActive;

    public void Activate()
    {
        isActive = true;
        Debug.Log($"Quest Activated: {title}");
    }

    public void Complete()
    {
        isCompleted = true;
        isActive = false;
        Debug.Log($"Quest Completed: {title}");
    }
}
