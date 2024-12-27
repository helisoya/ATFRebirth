using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// Handles the ennemies
/// </summary>
public class EnnemyManager : NetworkBehaviour
{
    private List<Ennemy> ennemies;

    public static EnnemyManager instance;


    void Awake()
    {
        instance = this;
        ennemies = new List<Ennemy>();
    }

    /// <summary>
    /// Registers an ennemy
    /// </summary>
    /// <param name="ennemy">The ennemy to register</param>
    public void RegisterEnnemy(Ennemy ennemy)
    {
        if (isServer)
        {
            ennemies.Add(ennemy);
        }
    }

    /// <summary>
    /// Unregister an ennemy, and ends the game if needed
    /// </summary>
    /// <param name="ennemy">The ennemy to unregister</param>
    public void UnRegisterEnnemy(Ennemy ennemy)
    {
        if (isServer && ennemies.Count > 0)
        {
            ennemies.Remove(ennemy);

            if (ennemies.Count == 0)
            {
                // End
                RpcOpenEndScreen();
            }
        }
    }

    /// <summary>
    /// Opens the end screen (clients)
    /// </summary>
    [ClientRpc]
    void RpcOpenEndScreen()
    {
        PlayerNetwork.localPlayer.CanMove = false;
        GameGUI.instance.OpenEndMenu();
    }

}
