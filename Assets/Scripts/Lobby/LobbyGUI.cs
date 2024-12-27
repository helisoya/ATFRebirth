using Mirror;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents the Lobby's GUI
/// </summary>
public class LobbyGUI : MonoBehaviour
{
    [Header("Players")]
    [SerializeField] private Transform playersRoot;
    [SerializeField] private LobbyPlayerGUI prefabPlayer;
    [SerializeField] private Image readyUpButton;
    private LobbyPlayer localPlayer;

    public static LobbyGUI instance;

    void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Sets the local player
    /// </summary>
    /// <param name="lobbyPlayer">The local player</param>
    public void SetLocalPlayer(LobbyPlayer lobbyPlayer)
    {
        localPlayer = lobbyPlayer;
    }

    /// <summary>
    /// Adds a player to the GUI
    /// </summary>
    /// <param name="id">The player's netId</param>
    public void AddPlayer(uint id)
    {
        Instantiate(prefabPlayer, playersRoot).Code = id;
    }

    /// <summary>
    /// Removes a player from the GUI
    /// </summary>
    /// <param name="id">The player's netId</param>
    public void RemovePlayer(uint id)
    {
        foreach (Transform child in playersRoot)
        {
            if (child.GetComponent<LobbyPlayerGUI>().Code == id)
            {
                Destroy(child.gameObject);
                return;
            }
        }
    }

    /// <summary>
    /// Sets a player's name
    /// </summary>
    /// <param name="id">The player's netId</param>
    /// <param name="newName">The player's name</param>
    public void EditPlayerName(uint id, string newName)
    {
        LobbyPlayerGUI player;
        foreach (Transform child in playersRoot)
        {
            player = child.GetComponent<LobbyPlayerGUI>();
            if (player.Code == id)
            {
                player.SetPlayerName(newName);
                return;
            }
        }
    }

    /// <summary>
    /// Sets a player's color
    /// </summary>
    /// <param name="id">The player's netId</param>
    /// <param name="newColor">The player's color</param>
    public void EditPlayerColor(uint id, Color newColor)
    {
        LobbyPlayerGUI player;
        foreach (Transform child in playersRoot)
        {
            player = child.GetComponent<LobbyPlayerGUI>();
            if (player.Code == id)
            {
                player.SetColor(newColor);
                return;
            }
        }
    }


    public void Click_Quit()
    {
        if (localPlayer.isClientOnly)
        {
            NetworkManager.singleton.StopClient();
        }
        else
        {
            NetworkManager.singleton.StopHost();
        }
    }

    public void Click_ReadyUp()
    {
        readyUpButton.color = localPlayer.readyToBegin ? Color.white : Color.green;
        localPlayer.ToggleReadyUp();
    }

}
