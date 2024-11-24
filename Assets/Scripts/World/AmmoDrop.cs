using Mirror;
using UnityEngine;

/// <summary>
/// Represents an ammo drop
/// </summary>
public class AmmoDrop : NetworkBehaviour
{
    [Header("Infos")]
    [SerializeField] private int ammoClipsAmount = 2;
    private bool activated = false;


    void OnTriggerEnter(Collider collider)
    {
        if (collider.attachedRigidbody == null) return;

        if (!activated && collider.attachedRigidbody.gameObject == PlayerNetwork.localPlayer.gameObject &&
            PlayerNetwork.localPlayer.weapon.AddAmmoToCurrentWeapon(ammoClipsAmount))
        {
            activated = true;
            DestroyDrop();
        }
    }

    /// <summary>
    /// Destroys the ammo drop on all clients
    /// </summary>
    [Command(requiresAuthority = false)]
    void DestroyDrop()
    {
        NetworkServer.Destroy(gameObject);
    }

}
