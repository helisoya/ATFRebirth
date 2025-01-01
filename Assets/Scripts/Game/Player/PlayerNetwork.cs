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
    [SerializeField] private GameObject[] disableWhenDead;
    [SerializeField] private AudioListener audioListener;

    [Header("Components")]
    [SerializeField] private PlayerWeapon _weapon;
    [SerializeField] private PlayerInterraction _interraction;
    [SerializeField] private PlayerHealth _health;
    [SerializeField] private PlayerNameText playerName;

    public PlayerWeapon weapon { get { return _weapon; } }
    public PlayerInterraction interraction { get { return _interraction; } }
    public PlayerHealth health { get { return _health; } }
    public Camera cam;

    public bool CanMove { get; set; }

    [SyncVar(hook = nameof(RefreshPlayerName))] public string username;

    public static PlayerNetwork localPlayer;

    private int currentPlayerIdx;

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

        cam.enabled = isLocalPlayer;

        if (isLocalPlayer)
        {
            localPlayer = this;
            CanMove = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            InitDataCmd(LocalPlayerData.Username);
        }
        else
        {
            audioListener.enabled = false;
            Destroy(audioListener);
        }

        GameManager.instance.players.Add(this);
        gameObject.name = "Player(" + GetComponent<NetworkIdentity>().netId + "-" + username + ")";
    }

    public override void OnStopClient()
    {
        GameManager.instance.players.Remove(this);
    }

    /// <summary>
    /// Starts the dead mode
    /// </summary>
    public void StartDeadMode()
    {
        GameGUI.instance.OpenDeadMenu();

        foreach (GameObject obj in disableWhenDead)
        {
            obj.SetActive(false);
        }

        cam.enabled = false;
        currentPlayerIdx = GameManager.instance.players.FindIndex(x => x == this);
        GameManager.instance.players[currentPlayerIdx].cam.enabled = true;
        FindFirstUserAlive(1);
    }

    /// <summary>
    /// Finds the first user alive and cache it
    /// </summary>
    private void FindFirstUserAlive(int direction)
    {
        List<PlayerNetwork> players = GameManager.instance.players;
        int delta = (currentPlayerIdx + direction + players.Count) % players.Count;
        print("Starting from " + delta);

        while (delta != currentPlayerIdx)
        {
            print(delta + " : " + players[delta].health.Alive);
            if (players[delta].health.Alive)
            {
                players[currentPlayerIdx].cam.enabled = false;
                currentPlayerIdx = delta;
                players[currentPlayerIdx].cam.enabled = true;
                GameGUI.instance.SetDeadUserName(players[currentPlayerIdx].username);
                return;
            }
            else
            {
                delta = (delta + direction + players.Count) % players.Count;
            }
        }
    }

    void Update()
    {
        if (isLocalPlayer && !health.Alive && !GameGUI.instance.inMenu)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                FindFirstUserAlive(-1);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                FindFirstUserAlive(1);
            }
        }
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
        playerName.SetText(newValue);
        gameObject.name = "Player(" + GetComponent<NetworkIdentity>().netId + "-" + newValue + ")";
    }
}
