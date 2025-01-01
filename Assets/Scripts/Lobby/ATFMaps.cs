using Mirror;
using UnityEngine;

/// <summary>
/// Represents all the game's map
/// </summary>
[CreateAssetMenu(fileName = "Maps", menuName = "ATF/Maps")]
public class ATFMaps : ScriptableObject
{
	public ATFMap[] maps;
}


/// <summary>
/// Represents a map's metadata
/// </summary>
[System.Serializable]
public class ATFMap
{
	public string displayName;

	[Scene]
	public string mapName;
	public Sprite mapSprite;
}