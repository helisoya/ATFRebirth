using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents the loadout tab in the main menu
/// </summary>
public class MainMenuLoadoutTab : MainMenuTab
{
    [Header("Loadout")]
    [SerializeField] private TMP_InputField userNameInputField;
    [SerializeField] private Image[] weaponSprites;
    [SerializeField] private MainMenuLoadoutWeaponSelectionTab weaponSelectionTab;
    private int selectedWeaponIndex;

    void Awake()
    {
        weaponSelectionTab.SetParent(this);
    }

    void Start()
    {
        userNameInputField.SetTextWithoutNotify(LocalPlayerData.Username);

        for (int i = 0; i < weaponSprites.Length; i++)
        {
            weaponSprites[i].sprite = Resources.Load<Sprite>("Guns/Sprites/" + LocalPlayerData.GetWeapon(i).ToString());
        }
    }

    /// <summary>
    /// Opens the weapon selection tab
    /// </summary>
    /// <param name="selection">The selected weapon index</param>
    public void OpenWeaponSelection(int selection)
    {
        selectedWeaponIndex = selection;
        weaponSelectionTab.SetUnavailableWeapon(LocalPlayerData.GetWeapon(selection));
        weaponSelectionTab.Open();
    }

    /// <summary>
    /// Selects a new weapon
    /// </summary>
    /// <param name="type">The weapon's type</param>
    public void SelectWeapon(WeaponType type)
    {
        LocalPlayerData.SetWeapon(selectedWeaponIndex, type);
        weaponSprites[selectedWeaponIndex].sprite = Resources.Load<Sprite>("Guns/Sprites/" + type.ToString());
    }


    /// <summary>
    /// Applies the changes to the loadout
    /// </summary>
    public void ApplyChanges()
    {
        if (!string.IsNullOrEmpty(userNameInputField.text))
        {
            LocalPlayerData.Username = userNameInputField.text;
        }
        weaponSelectionTab.Close();

        menu.CloseLoadout();
    }
}
