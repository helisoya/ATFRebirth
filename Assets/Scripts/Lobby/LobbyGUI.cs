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
    /// Gets the player's linked GUI
    /// </summary>
    /// <param name="id">The player's ID</param>
    /// <returns>The player's GUI (or null if it wasn't setup)</returns>
    private LobbyPlayerGUI GetPlayerGUI(uint id)
    {
        LobbyPlayerGUI gui;
        foreach (Transform child in playersRoot)
        {
            if (child == null) continue;

            gui = child.GetComponent<LobbyPlayerGUI>();
            if (gui.Code == id)
            {
                return gui;
            }
        }
        return null;
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
        LobbyPlayerGUI gui = GetPlayerGUI(id);
        if (!gui)
        {
            Instantiate(prefabPlayer, playersRoot).Code = id;
        }
    }

    /// <summary>
    /// Removes a player from the GUI
    /// </summary>
    /// <param name="id">The player's netId</param>
    public void RemovePlayer(uint id)
    {
        LobbyPlayerGUI gui = GetPlayerGUI(id);
        if (gui)
        {
            Destroy(gui.gameObject);
        }
    }

    /// <summary>
    /// Sets a player's name
    /// </summary>
    /// <param name="id">The player's netId</param>
    /// <param name="newName">The player's name</param>
    public void EditPlayerName(uint id, string newName)
    {
        LobbyPlayerGUI gui = GetPlayerGUI(id);
        if (gui)
        {
            gui.SetPlayerName(newName);
        }
    }

    /// <summary>
    /// Sets a player's color
    /// </summary>
    /// <param name="id">The player's netId</param>
    /// <param name="newColor">The player's color</param>
    public void EditPlayerColor(uint id, Color newColor)
    {
        LobbyPlayerGUI gui = GetPlayerGUI(id);
        if (gui)
        {
            gui.SetColor(newColor);
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
