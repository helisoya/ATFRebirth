using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// Represents a dropped weapon
/// </summary>
public class WeaponDrop : InteractableObject
{
    [SerializeField] private WeaponType type;
    [SerializeField] private Rigidbody rb;


    /// <summary>
    /// Initialize the weapon
    /// </summary>
    /// <param name="position">The weapon's position</param>
    /// <param name="impulseDirection">The weapon's impulse direction</param>
    public void Init(Vector3 position, Vector3 impulseDirection)
    {
        transform.position = position;
        rb.AddForce(impulseDirection * 2000);
    }

    /// <summary>
    /// Deletes the object from the network
    /// </summary>
    [Command(requiresAuthority = false)]
    public void NetworkDeleteRpc()
    {
        NetworkServer.Destroy(gameObject);
    }

    public override void OnInteraction()
    {
        if (PlayerNetwork.localPlayer.weapon.InCooldown) return;

        OnExit();
        PlayerNetwork.localPlayer.weapon.PickupWeapon(type);
        NetworkDeleteRpc();
        //Destroy(gameObject);
    }
}
