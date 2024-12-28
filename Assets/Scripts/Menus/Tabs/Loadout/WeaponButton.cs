using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a weapon's button on the loadout screen
/// </summary>
public class WeaponButton : MonoBehaviour
{
    [SerializeField] private Image weaponImg;
    [SerializeField] private Button button;
    private WeaponType type;
    private MainMenuLoadoutWeaponSelectionTab parent;


    /// <summary>
    /// Initialize the component
    /// </summary>
    /// <param name="type"></param>
    /// <param name="parent"></param>
    public void Init(WeaponType type, MainMenuLoadoutWeaponSelectionTab parent)
    {
        this.type = type;
        this.parent = parent;

        weaponImg.sprite = Resources.Load<Sprite>("Guns/Sprites/" + type.ToString());
    }

    /// <summary>
    /// Sets if the button can be interracted with
    /// </summary>
    /// <param name="value">Can the button be interracted with ?</param>
    public void SetInteractable(bool value)
    {
        button.interactable = value;
    }

    /// <summary>
    /// OnClick event
    /// </summary>
    public void Click()
    {
        parent.SelectWeapon(type);
    }
}
