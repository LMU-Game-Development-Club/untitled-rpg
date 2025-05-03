using UnityEngine;
using System.Collections.Generic;

public class GemCollect : MonoBehaviour, IInteractable
{
    public string gem;

    public void Interact()
    {
        if(GameManager.Instance.gems.Count == 0)
        {
            GameManager.Instance.gems.Add(gem);
            Destroy(this.gameObject);
        }
        else
        {
            //Debug message
            Debug.Log("Already holding a gem");
        }
    }
}