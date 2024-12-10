using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

/// <summary>
/// Represents the networked player
/// </summary>
public class PlayerNetwork : NetworkBehaviour
{
    [Header("Setup")]
    [SerializeField] private GameObject[] objectsToDisableIfNotClient;
    [SerializeField] private MonoBehaviour[] behavioursToDisableIfNotClient;
    [SerializeField] private GameObject[] objectsToDisableIfClient;

    [Header("Components")]
    [SerializeField] private PlayerWeapon _weapon;
    [SerializeField] private PlayerInterraction _interraction;
    [SerializeField] private PlayerHealth _health;

    public PlayerWeapon weapon { get { return _weapon; } }
    public PlayerInterraction interraction { get { return _interraction; } }
    public PlayerHealth health { get { return _health; } }
    public static PlayerNetwork localPlayer;

    public override void OnStartClient()
    {
        foreach (GameObject obj in objectsToDisableIfNotClient)
        {
            obj.SetActive(isLocalPlayer);
        }
        foreach (MonoBehaviour obj in behavioursToDisableIfNotClient)
        {
            obj.enabled = isLocalPlayer;
        }

        foreach (GameObject obj in objectsToDisableIfClient)
        {
            obj.SetActive(!isLocalPlayer);
        }

        if (isLocalPlayer)
        {
            localPlayer = this;


            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        GameManager.instance.players.Add(this);
        gameObject.name = "Player(" + GetComponent<NetworkIdentity>().netId + ")";
    }
}
