using System.Collections;
using System.Collections.Generic;
using TMPro;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents the game's GUI
/// </summary>
public class GameGUI : MonoBehaviour
{
    public static GameGUI instance { get; private set; }

    public bool inMenu { get { return menuRoot.activeInHierarchy || endRoot.activeInHierarchy; } }

    [Header("Interaction")]
    [SerializeField] private TextMeshProUGUI interactionText;

    [Header("Health")]
    [SerializeField] private Image healthFill;

    [Header("Weapons")]
    [SerializeField] private GUIWeapon[] weapons;

    [Header("General")]
    [SerializeField] private GameObject gameplayRoot;

    [Header("Pause")]
    [SerializeField] private GameObject menuRoot;
    private GameObject lastGameplayRoot;

    [Header("Dead")]
    [SerializeField] private GameObject deadRoot;
    [SerializeField] private TextMeshProUGUI deadCurrentPlayerText;

    [Header("End")]
    [SerializeField] private GameObject endRoot;
    [SerializeField] private GameObject toLobbyButton;

    void Awake()
    {
        instance = this;

        lastGameplayRoot = gameplayRoot;

        SetInteractionText("");
        SetHealthFillAmount(1f);
    }

    /// <summary>
    /// Sets the interaction's text
    /// </summary>
    /// <param name="text">The text to draw</param>
    public void SetInteractionText(string text)
    {
        interactionText.text = text;
    }

    /// <summary>
    /// Sets the health bar's fill amount
    /// </summary>
    /// <param name="fillAmount"></param>
    public void SetHealthFillAmount(float fillAmount)
    {
        healthFill.fillAmount = fillAmount;
    }

    /// <summary>
    /// Sets a weapon's name
    /// </summary>
    /// <param name="idx">The weapon's index</param>
    /// <param name="newName">The weapon's name</param>
    public void SetGUIWeaponName(int idx, string newName)
    {
        weapons[idx].SetWeaponName(newName);
    }

    /// <summary>
    /// Sets a weapon's ammo count
    /// </summary>
    /// <param name="idx">The weapon's index</param>
    /// <param name="ammoMag">The weapon's mag ammo</param>
    /// <param name="ammoBag">The weapons's bag ammo</param>
    public void SetGUIWeaponAmmo(int idx, int ammoMag, int ammoBag)
    {
        weapons[idx].SetWeaponAmmo(ammoMag, ammoBag);
    }

    /// <summary>
    /// Sets the active weapon on the GUI
    /// </summary>
    /// <param name="newIdx">The new weapon's index</param>
    public void SetCurrentGUIWeapon(int newIdx)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetWeaponActive(i == newIdx);
        }
    }

    /// <summary>
    /// Opens the death menu
    /// </summary>
    public void OpenDeadMenu()
    {
        gameplayRoot.SetActive(false);
        deadRoot.SetActive(true);
        lastGameplayRoot = deadRoot;
    }

    /// <summary>
    /// Sets the currently's followed player in dead mode
    /// </summary>
    /// <param name="username">The player's username</param>
    public void SetDeadUserName(string username)
    {
        deadCurrentPlayerText.text = "< " + username + " >";
    }


    /// <summary>
    /// Sets if the pause menu is open or not
    /// </summary>
    /// <param name="isOpen">Is the pause menu open ?</param>
    public void SetPauseOpen(bool isOpen)
    {
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isOpen;
        menuRoot.SetActive(isOpen);
        lastGameplayRoot.SetActive(!isOpen);
    }

    /// <summary>
    /// Opens the end screen
    /// </summary>
    public void OpenEndMenu()
    {
        endRoot.SetActive(true);
        menuRoot.SetActive(false);
        deadRoot.SetActive(false);
        gameplayRoot.SetActive(false);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        toLobbyButton.SetActive(!PlayerNetwork.localPlayer.isClientOnly);
    }

    /// <summary>
    /// Click event for returning to the lobby
    /// </summary>
    public void Click_ToLobby()
    {
        NetworkManager.singleton.ServerChangeScene(((NetworkRoomManager)NetworkManager.singleton).RoomScene);
    }

    /// <summary>
    /// Click event for continuing the game 
    /// </summary>
    public void Click_Continue()
    {
        SetPauseOpen(false);
    }

    /// <summary>
    /// Click event for quiting the game
    /// </summary>
    public void Click_Quit()
    {
        if (PlayerNetwork.localPlayer.isClientOnly)
        {
            NetworkManager.singleton.StopClient();
        }
        else
        {
            NetworkManager.singleton.StopHost();
        }
    }



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !endRoot.activeInHierarchy)
        {
            SetPauseOpen(!inMenu);
        }

        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            NetworkManager.singleton.StartHost();
        }
        else if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            NetworkManager.singleton.StartClient();
        }
    }
}
