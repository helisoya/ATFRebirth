using TMPro;
using UnityEngine;

/// <summary>
/// Represents a player's GUI in the lobby
/// </summary>
public class LobbyPlayerGUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerNameText;
    public uint Code { get; set; }

    /// <summary>
    /// Sets the player's name
    /// </summary>
    /// <param name="playerName">The player's name</param>
    public void SetPlayerName(string playerName)
    {
        playerNameText.text = playerName;
    }

    /// <summary>
    /// Sets the player's color
    /// </summary>
    /// <param name="color">The player's color</param>
    public void SetColor(Color color)
    {
        playerNameText.color = color;
    }
}
