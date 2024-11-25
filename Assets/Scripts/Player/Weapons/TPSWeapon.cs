using UnityEngine;

/// <summary>
/// Represents a TPS weapon
/// </summary>
public class TPSWeapon : MonoBehaviour
{
    [Header("Barrel Animations")]
    [SerializeField] private ParticleSystem muzzleFlareSystem;


    /// <summary>
    /// Activates the muzzle flare
    /// </summary>
    public void ActivateMuzzleFlare()
    {
        muzzleFlareSystem.Play();
    }
}
