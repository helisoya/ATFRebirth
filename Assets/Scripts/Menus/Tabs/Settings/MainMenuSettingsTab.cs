using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents the settings tab of the main menu
/// </summary>
public class MainMenuSettingsTab : MainMenuTab
{
    [Header("Settings")]
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown resolutionsDropdown;
    private Resolution[] resolutions;

    void Start()
    {
        // Setup settings
        resolutions = Screen.resolutions;
        Resolution current = Screen.currentResolution;
        int currentRes = 0;
        List<string> descs = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            Resolution tmp = resolutions[i];
            if (currentRes == 0 && current.width == tmp.width && current.height == tmp.height)
            {
                currentRes = i;
            }
            descs.Add(tmp.width + "x" + tmp.height);
        }
        resolutionsDropdown.ClearOptions();
        resolutionsDropdown.AddOptions(descs);
        resolutionsDropdown.SetValueWithoutNotify(currentRes);
        fullscreenToggle.isOn = Screen.fullScreen;
    }

    /// <summary>
    /// Accepts the settings changes
    /// </summary>
    public void AcceptSettingsChanges()
    {

        Resolution res = resolutions[resolutionsDropdown.value];
        Screen.SetResolution(res.width, res.height, fullscreenToggle.isOn);

        menu.CloseSettings();
    }
}
