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
