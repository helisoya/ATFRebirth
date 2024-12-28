using UnityEngine;

/// <summary>
/// Handles the main menu
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("Tabs")]
    [SerializeField] private MainMenuTab normalTab;
    [SerializeField] private MainMenuTab settingsTab;
    [SerializeField] private MainMenuTab serversTab;
    [SerializeField] private MainMenuTab loadoutTab;

    void Awake()
    {
        LocalPlayerData.Init();

        normalTab.SetMenu(this);
        settingsTab.SetMenu(this);
        serversTab.SetMenu(this);
        loadoutTab.SetMenu(this);
    }


    /// <summary>
    /// Opens the settings menu
    /// </summary>
    public void OpenSettings()
    {
        normalTab.Close();
        settingsTab.Open();
    }

    /// <summary>
    /// Closes the settings menu
    /// </summary>
    public void CloseSettings()
    {
        settingsTab.Close();
        normalTab.Open();
    }

    /// <summary>
    /// Opens the loadout menu
    /// </summary>
    public void OpenLoadout()
    {
        normalTab.Close();
        loadoutTab.Open();
    }

    /// <summary>
    /// Closes the loadout menu
    /// </summary>
    public void CloseLoadout()
    {
        normalTab.Open();
        loadoutTab.Close();
    }

    /// <summary>
    /// Opens the servers menu
    /// </summary>
    public void OpenServers()
    {
        normalTab.Close();
        serversTab.Open();
    }

    /// <summary>
    /// Closes the servers menu
    /// </summary>
    public void CloseServers()
    {
        normalTab.Open();
        serversTab.Close();
    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }
}
