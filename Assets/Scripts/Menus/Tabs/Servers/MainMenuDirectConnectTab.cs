using UnityEngine;
using TMPro;
using Mirror;

/// <summary>
/// Represents the direct connect popup on the main menu
/// </summary>
public class MainMenuDirectConnectTab : MainMenuTab
{
    [Header("Direct Connect")]
    [SerializeField] private TMP_InputField directConnectInputField;



    /// <summary>
    /// Tries to connect directly to the specified address
    /// </summary>
    public void TryDirectConnect()
    {
        NetworkManager.singleton.networkAddress = directConnectInputField.text;
        try
        {
            NetworkManager.singleton.StartClient();
        }
        catch
        {
            Close();
        }
    }
}
