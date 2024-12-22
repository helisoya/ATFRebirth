using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// Represents the networked player
/// </summary>
public class PlayerNetwork : NetworkBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject[] objectsToDisableIfNotClient;
    [SerializeField] private MonoBehaviour[] behavioursToDisableIfNotClient;
    [SerializeField] private GameObject[] objectsToDisableIfClient;

    [Header("Components")]
    [SerializeField] private PlayerWeapon _weapon;
    [SerializeField] private PlayerInterraction _interraction;
    [SerializeField] private PlayerHealth _health;
    [SerializeField] private PlayerNameText playerName;

    public PlayerWeapon weapon { get { return _weapon; } }
    public PlayerInterraction interraction { get { return _interraction; } }
    public PlayerHealth health { get { return _health; } }

    [SyncVar(hook = nameof(RefreshPlayerName))] public string username;

    public static PlayerNetwork localPlayer;

    public override void OnStartClient()
    {
        foreach (GameObject obj in objectsToDisableIfNotClient)
        {
            obj.SetActive(isLocalPlayer);
        }
        foreach (MonoBehaviour obj in behavioursToDisableIfNotClient)
        {
            obj.enabled = isLocalPlayer;
        }

        foreach (GameObject obj in objectsToDisableIfClient)
        {
            obj.SetActive(!isLocalPlayer);
        }

        if (isLocalPlayer)
        {
            localPlayer = this;


            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            InitDataCmd(GetComponent<NetworkIdentity>().netId.ToString());
        }

        GameManager.instance.players.Add(this);
        gameObject.name = "Player(" + GetComponent<NetworkIdentity>().netId + ")";
    }

    /// <summary>
    /// Initialize the networked data (Server)
    /// </summary>
    /// <param name="playerName">The player's name</param>
    [Command(requiresAuthority = false)]
    private void InitDataCmd(string playerName)
    {
        username = playerName;
    }

    /// <summary>
    /// Hook for refreshing the username
    /// </summary>
    /// <param name="oldValue">The old username</param>
    /// <param name="newValue">The new username</param>
    void RefreshPlayerName(string oldValue, string newValue)
    {
        playerName.SetText(username);
    }
}
