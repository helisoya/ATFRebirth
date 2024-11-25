using System.Collections;
using System.Collections.Generic;
using TMPro;
using Mirror;
using UnityEngine;

/// <summary>
/// Represents the game's GUI
/// </summary>
public class GameGUI : MonoBehaviour
{
    public static GameGUI instance { get; private set; }

    public bool inMenu { get { return menuRoot.activeInHierarchy; } }

    [Header("Interractions")]
    [SerializeField] private TextMeshProUGUI interactionText;

    [Header("Weapons")]
    [SerializeField] private GUIWeapon[] weapons;


    [Header("Pause")]
    [SerializeField] private GameObject menuRoot;

    void Awake()
    {
        instance = this;
        //Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        SetInteractionText("");
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
    /// Opens the pause menu
    /// </summary>
    public void OpenPauseMenu()
    {
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Closes the pause menu
    /// </summary>
    public void ClosePauseMenu()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
        {
            FindFirstObjectByType<NetworkManager>().StartHost();
        }
        else if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            FindFirstObjectByType<NetworkManager>().StartClient();
        }
    }
}
