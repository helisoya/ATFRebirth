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
    [SerializeField] private GameObject userNameRoot;
    private int health;

    public bool Alive { get; private set; } = true;

    void Start()
    {
        health = maxHealth;
    }

    /// <summary>
    /// Adds health to the player
    /// </summary>
    /// <param name="amount">The amount of health to add</param>
    public void AddHealth(int amount)
    {
        if (!Alive) return;

        health = Mathf.Clamp(health + amount, 0, maxHealth);
        GameGUI.instance.SetHealthFillAmount(health / ((float)maxHealth));

        if (health == 0)
        {
            Alive = false;
            PlayerNetwork.localPlayer.StartDeadMode();
            CmdActivateRagdoll();
        }
    }


    /// <summary>
    /// Inflict damage on the player
    /// </summary>
    /// <param name="amount">The amount of damage</param>
    [ClientRpc(includeOwner = true)]
    public void TakeDamage(int amount)
    {
        if (isLocalPlayer) AddHealth(-amount);
    }


    /// <summary>
    /// Activates the ragdoll (Server)
    /// </summary>
    [Command(requiresAuthority = false)]
    void CmdActivateRagdoll()
    {
        Alive = false;
        CheckDeaths();
        RpcActivateRagdoll();
    }

    /// <summary>
    /// Checks if the players are all dead
    /// Ends the game if so
    /// </summary>
    [ServerCallback]
    private void CheckDeaths()
    {
        foreach (PlayerNetwork player in GameManager.instance.players)
        {
            if (player.health.Alive) return;
        }
        RpcOpenEndScreen();
    }

    /// <summary>
    /// Opens the end menu (Clients)
    /// </summary>
    [ClientRpc(includeOwner = true)]
    void RpcOpenEndScreen()
    {
        GameGUI.instance.OpenEndMenu();
    }

    /// <summary>
    /// Activates the ragdoll (Client)
    /// </summary>
    [ClientRpc(includeOwner = true)]
    void RpcActivateRagdoll()
    {
        ragdollManager.ActivateRagdoll();
        Alive = false;
        userNameRoot.SetActive(false);
    }
}
