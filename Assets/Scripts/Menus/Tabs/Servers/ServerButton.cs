using Mirror.Discovery;
using TMPro;
using UnityEngine;

/// <summary>
/// Represents a button that can connect to a server on the Main Menu
/// </summary>
public class ServerButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    private DiscoveryResponse response;
    private MainMenuServerTab parent;

    /// <summary>
    /// Initialize the button 
    /// </summary>
    /// <param name="info">The linked server</param>
    /// <param name="list">The button's parent</param>
    public void Init(DiscoveryResponse info, MainMenuServerTab parent)
    {
        text.text = info.EndPoint.Address.ToString() + " (" + info.TotalPlayers + "/" + info.MaxPlayers + ")";
        response = info;
        this.parent = parent;
    }

    /// <summary>
    /// OnClick event
    /// </summary>
    public void OnClick()
    {
        parent.StartClient(response);
    }
}
