using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Represents a weapon's data
/// </summary>
[CreateAssetMenu(fileName = "WeaponData", menuName = "ATF/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Stats")]
    public WeaponType type;
    public int dmg;
    public int maxAmmoInMag;
    public int maxAmmoInBag;
    public float maxRange;
    public bool automatic;
    public bool silenced;

    [Header("Animations")]
    public float fireCooldown;
    public float reloadCooldown;
    public RuntimeAnimatorController animType;
    public RuntimeAnimatorController bodyAnimType;
    public float drawTime;
    public float undrawTime;

    [Header("Sounds")]
    public AudioClip fireSound;
    public AudioClip reloadSound;
}
