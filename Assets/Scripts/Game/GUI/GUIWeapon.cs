using TMPro;
using UnityEngine;

/// <summary>
/// Represents a weapon of the GUI
/// </summary>
public class GUIWeapon : MonoBehaviour
{
    [Header("Infos")]
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI weaponAmmoText;

    /// <summary>
    /// Sets the weapon's name
    /// </summary>
    /// <param name="weaponName">The weapon's name</param>
    public void SetWeaponName(string weaponName)
    {
        weaponNameText.text = weaponName;
    }

    /// <summary>
    /// Sets the weapon's ammo
    /// </summary>
    /// <param name="inMag">The ammo in the mag</param>
    /// <param name="inBag">The ammo in the bag</param>
    public void SetWeaponAmmo(int inMag, int inBag)
    {
        weaponAmmoText.text = inMag + "/" + inBag;
    }

    /// <summary>
    /// Sets if the weapon is used or not
    /// </summary>
    /// <param name="value">Is the weapon used ?</param>
    public void SetWeaponActive(bool value)
    {
        weaponNameText.color = value ? Color.white : Color.grey;
        weaponAmmoText.color = value ? Color.white : Color.grey;
    }
}
