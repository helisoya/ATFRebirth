using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents a map button on the lobby menu
/// </summary>
public class MapButton : MonoBehaviour
{
    [SerializeField] private Image previewImg;
    private ATFMap linkedMap;

    /// <summary>
    /// Initialize the button
    /// </summary>
    /// <param name="map">The linked map</param>
    public void Init(ATFMap map)
    {
        linkedMap = map;
        previewImg.sprite = map.mapSprite;
    }

    /// <summary>
    /// Click event
    /// </summary>
    public void Click()
    {
        LobbyGUI.instance.SelectMap(linkedMap);
    }
}
