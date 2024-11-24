using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// Handles the player's interractions
/// </summary>
public class PlayerInterraction : NetworkBehaviour
{
    [SerializeField] private float minimumDistance;
    [SerializeField] private LayerMask exitMask;
    private InteractableObject currentInteractable;
    private Transform cam;


    void Start()
    {
        cam = Camera.main.transform;
    }

    /// <summary>
    /// Changes the currently focused on object if possible
    /// </summary>
    /// <param name="current">The new object</param>
    public void SetCurrentObject(InteractableObject current)
    {
        if (GameGUI.instance.inMenu) return;

        currentInteractable = current;
    }

    /// <summary>
    /// Remove the current object
    /// </summary>
    /// <param name="old">The current object</param>
    public void RemoveCurrentObject(InteractableObject old)
    {
        if (currentInteractable == old)
        {
            currentInteractable = null;
        }
    }

    void Update()
    {
        if (GameGUI.instance.inMenu) return;

        RaycastHit hit;
        if (Physics.Raycast(new Ray(cam.position, cam.forward), out hit,
            minimumDistance, exitMask))
        {
            InteractableObject obj = hit.transform.GetComponent<InteractableObject>();

            if ((!obj || obj != currentInteractable) && currentInteractable)
            {
                currentInteractable.OnExit();
            }

            if (obj)
            {
                obj.OnEnter();
            }
        }
        else if (currentInteractable)
        {
            currentInteractable.OnExit();
        }
        else
        {
            GameGUI.instance.SetInteractionText("");
        }


        if (currentInteractable != null && Input.GetKeyDown(KeyCode.E))
        {
            currentInteractable.OnInteraction();
            RemoveCurrentObject(currentInteractable);
        }
    }
}
