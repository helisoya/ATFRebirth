using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// Parent class for interactable objects
/// </summary>
public class InteractableObject : NetworkBehaviour
{
    [Header("General Infos")]
    [SerializeField] protected string hoverText;
    [SerializeField] protected NetworkIdentity network;

    /// <summary>
    /// Handles the OnMouseEnter event
    /// </summary>
    public virtual void OnEnter()
    {
        GameGUI.instance.SetInteractionText(hoverText);
        PlayerNetwork.localPlayer.interraction.SetCurrentObject(this);
    }

    /// <summary>
    /// Handles the OnMouseExit event
    /// </summary>
    public virtual void OnExit()
    {
        GameGUI.instance.SetInteractionText("");
        PlayerNetwork.localPlayer.interraction.RemoveCurrentObject(this);
    }

    /// <summary>
    /// Handles the interraction
    /// </summary>
    public virtual void OnInteraction()
    {
        print("Interacted with " + name);
    }
}
