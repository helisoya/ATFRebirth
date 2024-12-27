using UnityEngine;
using Mirror;


/// <summary>
/// Represents a player in a lobby
/// </summary>
public class LobbyPlayer : NetworkRoomPlayer
{

    [SyncVar(hook = nameof(EditPlayerName))] private string username;

    /// <summary>
    /// Edits a player's name on the GUI
    /// </summary>
    /// <param name="oldValue">The old username</param>
    /// <param name="newValue">The new username</param>
    void EditPlayerName(string oldValue, string newValue)
    {
        username = newValue;
        LobbyGUI.instance.EditPlayerName(GetComponent<NetworkIdentity>().netId, username);
    }


    public override void OnStartClient()
    {
        if (isLocalPlayer)
        {
            LobbyGUI.instance.SetLocalPlayer(this);
            CmdEditPlayerName(LocalPlayerData.Username);
        }
    }

    public override void OnStopClient()
    {
        // Remove from GUI
        LobbyGUI.instance.RemovePlayer(GetComponent<NetworkIdentity>().netId);
    }

    /// <summary>
    /// Disconnect the player from the lobby
    /// </summary>
    [Command(requiresAuthority = false)]
    public void Disconnect()
    {
        GetComponent<NetworkIdentity>().connectionToClient.Disconnect();
    }

    /// <summary>
    /// Toggle the ready up states of the player
    /// </summary>
    public void ToggleReadyUp()
    {
        CmdChangeReadyState(!readyToBegin);
    }

    /// <summary>
    /// Edits the Player username (Server)
    /// </summary>
    /// <param name="newUsername"></param>
    [Command(requiresAuthority = false)]
    private void CmdEditPlayerName(string newUsername)
    {
        username = newUsername;
    }



    #region Room Client Callbacks

    /// <summary>
    /// This is a hook that is invoked on all player objects when entering the room.
    /// <para>Note: isLocalPlayer is not guaranteed to be set until OnStartLocalPlayer is called.</para>
    /// </summary>
    public override void OnClientEnterRoom()
    {
        LobbyGUI.instance.AddPlayer(GetComponent<NetworkIdentity>().netId);
        LobbyGUI.instance.EditPlayerName(GetComponent<NetworkIdentity>().netId, username);

        if (isLocalPlayer)
        {
            LobbyGUI.instance.SetLocalPlayer(this);
        }
    }

    /// <summary>
    /// This is a hook that is invoked on all player objects when exiting the room.
    /// </summary>
    public override void OnClientExitRoom() { }

    #endregion

    #region SyncVar Hooks

    /// <summary>
    /// This is a hook that is invoked on clients when the index changes.
    /// </summary>
    /// <param name="oldIndex">The old index value</param>
    /// <param name="newIndex">The new index value</param>
    public override void IndexChanged(int oldIndex, int newIndex) { }

    /// <summary>
    /// This is a hook that is invoked on clients when a RoomPlayer switches between ready or not ready.
    /// <para>This function is called when the a client player calls SendReadyToBeginMessage() or SendNotReadyToBeginMessage().</para>
    /// </summary>
    /// <param name="oldReadyState">The old readyState value</param>
    /// <param name="newReadyState">The new readyState value</param>
    public override void ReadyStateChanged(bool oldReadyState, bool newReadyState)
    {
        if (LobbyGUI.instance != null && gameObject != null)
            LobbyGUI.instance.EditPlayerColor(GetComponent<NetworkIdentity>().netId, newReadyState ? Color.green : Color.black);
    }

    #endregion

}
