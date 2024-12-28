using System;
using UnityEngine;

/// <summary>
/// Represents the weapon selection tab in the main menu
/// </summary>
public class MainMenuLoadoutWeaponSelectionTab : MainMenuTab
{
    private MainMenuLoadoutTab parent;

    [Header("Weapon Selection")]
    [SerializeField] private WeaponButton buttonPrefab;
    [SerializeField] private Transform buttonsRoot;

    void Awake()
    {
        Array array = Enum.GetValues(typeof(WeaponType));

        for (int i = 1; i < array.Length; i++)
        {
            Instantiate(buttonPrefab, buttonsRoot).Init((WeaponType)array.GetValue(i), this);
        }
    }

    /// <summary>
    /// Sets the currently unavailable weapon
    /// </summary>
    /// <param name="weaponType">The weapon type</param>
    public void SetUnavailableWeapon(WeaponType weaponType)
    {
        int idx = (int)weaponType - 1;
        for (int i = 0; i < buttonsRoot.childCount; i++)
        {
            buttonsRoot.GetChild(i).GetComponent<WeaponButton>().SetInteractable(idx != i);
        }
    }

    /// <summary>
    /// Sets this tab's parent
    /// </summary>
    /// <param name="parent">The tab's parent</param>
    public void SetParent(MainMenuLoadoutTab parent)
    {
        this.parent = parent;
    }

    /// <summary>
    /// Callback for choosing a weapon
    /// </summary>
    /// <param name="type">The weapon's type</param>
    public void SelectWeapon(WeaponType type)
    {
        parent.SelectWeapon(type);
        Close();
    }
}
