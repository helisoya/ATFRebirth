using UnityEngine;

/// <summary>
/// Represents the local player's data
/// </summary>
public class LocalPlayerData
{
    private static LocalPlayerData instance;
    private PlayerSave save;
    private string savePath = FileManager.savPath + "data.sav";


    /// <summary>
    /// Initialize the player's data
    /// </summary>
    public static void Init()
    {
        instance = new LocalPlayerData();
        LoadData();
    }

    public static string Username
    {
        get
        {
            return instance.save.username;
        }
        set
        {
            instance.save.username = value;
            SaveData();
        }
    }

    /// <summary>
    /// Gets a weapon from the player's inventory
    /// </summary>
    /// <param name="idx">The weapon's index</param>
    /// <returns>The weapon type</returns>
    public static WeaponType GetWeapon(int idx)
    {
        if (idx < 0 || idx > 1) return WeaponType.NONE;
        return instance.save.weapons[idx];
    }

    /// <summary>
    /// Sets the weapon at the given index
    /// </summary>
    /// <param name="idx">The index to modify</param>
    /// <param name="weapon">The new weapon</param>
    public static void SetWeapon(int idx, WeaponType weapon)
    {
        if (idx < 0 || idx > 1) return;
        instance.save.weapons[idx] = weapon;
        SaveData();
    }

    /// <summary>
    /// Load the data 
    /// </summary>
    public static void LoadData()
    {
        if (System.IO.File.Exists(instance.savePath))
        {
            instance.save = FileManager.LoadJSON<PlayerSave>(instance.savePath);
        }
        else
        {
            instance.save = new PlayerSave();
            SaveData();
        }
    }

    /// <summary>
    /// Saves the data
    /// </summary>
    private static void SaveData()
    {
        FileManager.SaveJSON(instance.savePath, instance.save);
    }
}

/// <summary>
/// Represents the player's save file
/// </summary>
[System.Serializable]
public class PlayerSave
{
    public string username;
    public WeaponType[] weapons;

    public PlayerSave()
    {
        username = "Operative";
        weapons = new WeaponType[2] { WeaponType.GLOCK, WeaponType.MP5 };
    }
}