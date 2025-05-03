using UnityEngine;
using System.Collections.Generic;

public class GemCollect : MonoBehaviour, IInteractable
{
    public string gem;

    public void Interact()
    {
        GameManager.Instance.gems.Add(gem);
        Destroy(this.gameObject);
    }
}