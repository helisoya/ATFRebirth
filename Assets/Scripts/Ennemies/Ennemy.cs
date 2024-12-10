using Mirror;
using UnityEngine;

/// <summary>
/// Represents an ennemy
/// </summary>
public class Ennemy : NetworkBehaviour
{
    [Header("Infos")]
    [SerializeField] private int maxHealth = 20;

    [Header("Components")]
    [SerializeField] private RagdollManager ragdollManager;
    [SerializeField] private EnnemyLogic logic;
    [SerializeField] private GameObject prefabAmmoBox;

    private int health;

    void Start()
    {
        health = maxHealth;
    }

    /// <summary>
    /// Activates 
    /// </summary>
    /// <param name="amount"></param>
    public void TakeDamage(int amount)
    {
        if (health == 0 || !isServer) return;
        print("Taking : " + amount);

        health = Mathf.Clamp(health - amount, 0, maxHealth);
        if (health == 0)
        {
            // Die
            logic.Activated = false;
            RpcActivateRagdoll();

            if (prefabAmmoBox)
            {
                NetworkServer.Spawn(Instantiate(prefabAmmoBox, transform.position, Quaternion.identity));
            }
        }
    }

    /// <summary>
    /// Activates the ragdoll on the clients
    /// </summary>
    [ClientRpc]
    void RpcActivateRagdoll()
    {
        ragdollManager.ActivateRagdoll();
    }


}
