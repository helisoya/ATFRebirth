using Mirror;
using UnityEngine;

/// <summary>
/// Represents a door that opens when a player comes near it
/// </summary>
public class SasDoor : NetworkBehaviour
{
    [SerializeField] private NetworkedAnimator animator;
    private bool alreadyOpen;

    void Start()
    {
        alreadyOpen = false;
    }

    void OnTriggerEnter(Collider collider)
    {
        if (isServer && !alreadyOpen && collider.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            alreadyOpen = true;
            animator.SetTrigger("Open");
        }
    }
}
