using UnityEngine;

/// <summary>
/// Represents a tab in the main menu
/// </summary>
public class MainMenuTab : MonoBehaviour
{

    [Header("General")]
    [SerializeField] protected GameObject root;
    protected MainMenuManager menu;

    /// <summary>
    /// Sets the tab's parent menu
    /// </summary>
    /// <param name="mainMenuManager">The main menu</param>
    public void SetMenu(MainMenuManager mainMenuManager)
    {
        menu = mainMenuManager;
    }

    /// <summary>
    /// Opens the tab
    /// </summary>
    public void Open()
    {
        if (root.activeInHierarchy) return;

        root.SetActive(true);
        Open();
    }

    /// <summary>
    /// Closes the tab
    /// </summary>
    public void Close()
    {
        if (!root.activeInHierarchy) return;

        root.SetActive(false);
        OnClose();
    }

    /// <summary>
    /// Callback called when opening the tab
    /// </summary>
    protected virtual void OnOpen() { }

    /// <summary>
    /// Callback called when closed the tab
    /// </summary>
    protected virtual void OnClose() { }
}
