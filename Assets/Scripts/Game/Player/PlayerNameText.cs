using TMPro;
using UnityEngine;

/// <summary>
/// Represents the player name hovering above them
/// </summary>
public class PlayerNameText : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;

    void Update()
    {
        if (PlayerNetwork.localPlayer == null) return;

        transform.LookAt(PlayerNetwork.localPlayer.transform);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y + 180, 0);
    }

    /// <summary>
    /// Sets the text's value
    /// </summary>
    /// <param name="value">The new value</param>
    public void SetText(string value)
    {
        text.text = value;
    }

}

