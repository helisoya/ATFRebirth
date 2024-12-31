using UnityEngine;
using TMPro;
using Mirror;
using Mirror.Discovery;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System;
using System.Net;
using System.Collections.Generic;

/// <summary>
/// Represents the servers tab in the main menu
/// </summary>
public class MainMenuServerTab : MainMenuTab
{
    [Header("Servers")]
    [SerializeField] private Transform serverParent;
    [SerializeField] private GameObject prefabServer;
    [SerializeField] private TMP_InputField ipInput;
    [SerializeField] private ATFNetworkDiscovery networkDiscovery;
    [SerializeField] private MainMenuTab directConnectTab;

    Dictionary<long, DiscoveryResponse> discoveredServers = new Dictionary<long, DiscoveryResponse>();


    protected override void OnOpen()
    {
        SearchServers();
    }

    protected override void OnClose()
    {
        StopSearchingServers();
        directConnectTab.Close();
    }

    void Awake()
    {
        /// Setup Discovery
        networkDiscovery.transport = NetworkManager.singleton.transport;
        string[] ipSplit = GetLocalIPv4().Split(".");
        string ipCorrect = ipSplit[0] + "." + ipSplit[1] + "." + ipSplit[2] + ".255";
        networkDiscovery.BroadcastAddress = ipCorrect;
        ipInput.SetTextWithoutNotify(networkDiscovery.BroadcastAddress);
    }


    /// <summary>
    /// Change the broadcast IP
    /// </summary>
    public void ChangeBroadcastIP()
    {
        networkDiscovery.BroadcastAddress = ipInput.text;
        SearchServers();
    }

    /// <summary>
    /// Gets the local IP
    /// </summary>
    /// <returns>The local IP</returns>
    /// <exception cref="System.Exception">Error</exception>
    public string GetLocalIPv4()
    {

        foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
        {
            foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
            {
                if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork && adapter.OperationalStatus == OperationalStatus.Up)
                {
                    uint ipAddress = BitConverter.ToUInt32(unicastIPAddressInformation.Address.GetAddressBytes(), 0);
                    uint ipMaskV4 = BitConverter.ToUInt32(unicastIPAddressInformation.IPv4Mask.GetAddressBytes(), 0);
                    uint broadCastIpAddress = ipAddress | ~ipMaskV4;

                    return new IPAddress(BitConverter.GetBytes(broadCastIpAddress)).ToString();
                }
            }
        }

        return "localhost";
    }

    /// <summary>
    /// Stops searching for servers
    /// </summary>
    public void StopSearchingServers()
    {
        discoveredServers.Clear();
        foreach (Transform t in serverParent.transform)
        {
            Destroy(t.gameObject);
        }
        networkDiscovery.StopDiscovery();
    }

    /// <summary>
    /// Start searching for servers
    /// </summary>
    public void SearchServers()
    {
        StopSearchingServers();
        networkDiscovery.StartDiscovery();
    }

    /// <summary>
    /// Refresh the servers list
    /// </summary>
    public void RefreshServers()
    {
        SearchServers();
    }

    /// <summary>
    /// Start hosting a game
    /// </summary>
    public void StartHost()
    {
        discoveredServers.Clear();
        NetworkManager.singleton.StartHost();
        networkDiscovery.AdvertiseServer();
    }

    /// <summary>
    /// Connect to an existing server
    /// </summary>
    /// <param name="info">The server info</param>
    public void StartClient(DiscoveryResponse info)
    {
        if (info.TotalPlayers >= info.MaxPlayers) return;
        networkDiscovery.StopDiscovery();
        NetworkManager.singleton.StartClient(info.uri);
    }

    /// <summary>
    /// Callback for discovering a server
    /// </summary>
    /// <param name="info">The server's info</param>
    public void OnDiscoveredServer(DiscoveryResponse info)
    {
        if (discoveredServers.ContainsKey(info.serverId)) return;

        discoveredServers[info.serverId] = info;
        print("Serveur : " + info.serverId + " - " + info.uri);
        GameObject button = Instantiate(prefabServer, serverParent);
        button.GetComponent<ServerButton>().Init(info, this);
        RectTransform rect = serverParent.GetComponent<RectTransform>();

        rect.sizeDelta = new Vector2(rect.sizeDelta.x, 30 * discoveredServers.Keys.Count);
    }
}
