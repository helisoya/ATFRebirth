using Mirror;
using UnityEngine;

/// <summary>
/// Represents the player's health
/// </summary>
public class PlayerHealth : NetworkBehaviour
{
    [Header("Infos")]
    [SerializeField] private int maxHealth = 20;
    [SerializeField] private RagdollManager ragdollManager;
    private int health;

    public bool Alive { get; private set; }

    void Start()
    {
        if (isLocalPlayer)
        {
            Alive = true;
            health = maxHealth;
        }
    }

    /// <summary>
    /// Adds health to the player
    /// </summary>
    /// <param name="amount">The amount of health to add</param>
    public void AddHealth(int amount)
    {
        if (!Alive) return;

        health = Mathf.Clamp(health + amount, 0, maxHealth);

        if (health == 0)
        {
            Alive = false;
            CmdActivateRagdoll();
        }
    }


    /// <summary>
    /// Activates the ragdoll (Server)
    /// </summary>
    [Command(requiresAuthority = false)]
    void CmdActivateRagdoll()
    {
        ragdollManager.ActivateRagdoll();
        RpcActivateRagdoll();
    }

    /// <summary>
    /// Activates the ragdoll (Client)
    /// </summary>
    [ClientRpc(includeOwner = true)]
    void RpcActivateRagdoll()
    {
        ragdollManager.ActivateRagdoll();
    }
}
