using UnityEngine;

public class PuzzleChecker: MonoBehaviour, IInteractable
{
    public string GemName;
    public Sprite GemSprite;

    public void Interact()
    {
        if (GameManager.Instance.gems.Contains(GemName))
        {
            GameManager.Instance.gems.Remove(GemName);
            //replace current sprite with gemsprite
            GetComponent<SpriteRenderer>().sprite = GemSprite;
        }
        else
        {
            //Debug message
            Debug.Log("You need to collect the gem first!");
        }
    }
}
