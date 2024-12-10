using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles the game's management
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Data")]
    [SerializeField] private WeaponDataContainer[] weapons;
    public List<PlayerNetwork> players { get; private set; }

    void Awake()
    {
        instance = this;
        players = new List<PlayerNetwork>();
    }

    public WeaponData GetWeaponData(WeaponType type)
    {
        foreach (WeaponDataContainer container in weapons)
        {
            if (container.type == type) return container.data;
        }
        return null;
    }
}


/// <summary>
/// Container for weapons data
/// </summary>
[System.Serializable]
public class WeaponDataContainer
{
    public WeaponType type;
    public WeaponData data;
}

/// <summary>
/// Represents the game's different weapons
/// </summary>
public enum WeaponType
{
    NONE,
    GLOCK,
    REVOLVER,
    MP5,
    AK47
}
