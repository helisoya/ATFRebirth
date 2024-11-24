using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a weapon
/// </summary>
public class Weapon : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private WeaponData data;
    private int ammoInBag;
    private int ammoInMag;



    [Header("Barrel Animations")]
    [SerializeField] private ParticleSystem muzzleFlareSystem;
    [SerializeField] private Transform barrel;

    public bool canReload
    {
        get
        {
            return ammoInMag < data.maxAmmoInMag && ammoInBag > 0;
        }
    }

    public bool canFire
    {
        get
        {
            return ammoInMag > 0;
        }
    }

    /// <summary>
    /// Gets the weapon's data
    /// </summary>
    /// <returns>The weapon's data</returns>
    public WeaponData GetWeaponData()
    {
        return data;
    }


    void Start()
    {
        ammoInMag = data.maxAmmoInMag;
        ammoInBag = data.maxAmmoInBag;
    }

    /// <summary>
    /// Adds mags to the gun
    /// </summary>
    /// <param name="mags">The number of mags to add</param>
    /// <returns>Was ammo added ?</returns>
    public bool AddMags(int mags)
    {
        if (ammoInBag >= data.maxAmmoInBag) return false;

        ammoInBag = Mathf.Clamp(ammoInBag + mags * data.maxAmmoInMag, 0, data.maxAmmoInBag);
        return true;
    }

    /// <summary>
    /// Refill the gun
    /// </summary>
    public void RefillAmmoToMax()
    {
        ammoInBag = data.maxAmmoInBag;
        ammoInMag = data.maxAmmoInMag;
    }

    /// <summary>
    /// Reloads the gun
    /// </summary>
    public void Reload()
    {
        if (!canReload) return;

        int amoutThatCanBeLoadedIn = Mathf.Max(Mathf.Min(ammoInBag, data.maxAmmoInMag - ammoInMag), 0);
        print("Loading in " + amoutThatCanBeLoadedIn);
        ammoInBag -= amoutThatCanBeLoadedIn;
        ammoInMag += amoutThatCanBeLoadedIn;
    }

    /// <summary>
    /// Fires the gun
    /// </summary>
    public void Fire()
    {
        if (ammoInMag > 0)
        {
            ammoInMag--;
            ActivateMuzzleFlare();
        }
    }

    /// <summary>
    /// Activates the muzzle flare
    /// </summary>
    private void ActivateMuzzleFlare()
    {
        muzzleFlareSystem.Play();
    }

}
