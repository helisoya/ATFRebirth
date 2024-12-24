using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Mirror;
using Mirror.Discovery;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the main menu
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("Normal Screen")]
    [SerializeField] private GameObject normalScreenRoot;


    [Header("Settings Screen")]
    [SerializeField] private GameObject settingsScreenRoot;
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown resolutionsDropdown;
    private Resolution[] resolutions;

    [Header("Loadout")]
    [SerializeField] private GameObject loadoutScreenRoot;

    [Header("Servers Screen")]
    [SerializeField] private GameObject serversScreenRoot;
    [SerializeField] private Transform serverParent;
    [SerializeField] private GameObject prefabServer;
    [SerializeField] private TMP_InputField ipInput;


    Dictionary<long, ServerResponse> discoveredServers = new Dictionary<long, ServerResponse>();
    public NetworkDiscovery networkDiscovery;


    void Awake()
    {
        LocalPlayerData.Init();
    }


    /// <summary>
    /// Opens the settings menu
    /// </summary>
    public void OpenSettings()
    {
        normalScreenRoot.SetActive(false);
        settingsScreenRoot.SetActive(true);
    }

    /// <summary>
    /// Closes the settings menu
    /// </summary>
    public void CloseSettings()
    {
        normalScreenRoot.SetActive(true);
        settingsScreenRoot.SetActive(false);
    }

    /// <summary>
    /// Opens the loadout menu
    /// </summary>
    public void OpenLoadout()
    {
        normalScreenRoot.SetActive(false);
        loadoutScreenRoot.SetActive(true);
    }

    /// <summary>
    /// Closes the loadout menu
    /// </summary>
    public void CloseLoadout()
    {
        normalScreenRoot.SetActive(true);
        loadoutScreenRoot.SetActive(false);
    }

    /// <summary>
    /// Opens the servers menu
    /// </summary>
    public void OpenServers()
    {
        normalScreenRoot.SetActive(false);
        serversScreenRoot.SetActive(true);
        SearchServers();
    }

    /// <summary>
    /// Closes the servers menu
    /// </summary>
    public void CloseServers()
    {
        normalScreenRoot.SetActive(true);
        serversScreenRoot.SetActive(false);
        StopSearchingServers();
    }

    /// <summary>
    /// Quits the game
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

    void Start()
    {
        /// Setup Discovery
        networkDiscovery.transport = NetworkManager.singleton.transport;
        string[] ipSplit = GetLocalIPv4().Split(".");
        string ipCorrect = ipSplit[0] + "." + ipSplit[1] + "." + ipSplit[2] + ".255";
        networkDiscovery.BroadcastAddress = ipCorrect;
        ipInput.SetTextWithoutNotify(networkDiscovery.BroadcastAddress);



        // Setup settings
        resolutions = Screen.resolutions;
        Resolution current = Screen.currentResolution;
        int currentRes = 0;
        List<string> descs = new List<string>();
        for (int i = 0; i < resolutions.Length; i++)
        {
            Resolution tmp = resolutions[i];
            if (currentRes == 0 && current.width == tmp.width && current.height == tmp.height)
            {
                currentRes = i;
            }
            descs.Add(tmp.width + "x" + tmp.height + " (" + tmp.refreshRateRatio + ")");
        }
        resolutionsDropdown.ClearOptions();
        resolutionsDropdown.AddOptions(descs);
        resolutionsDropdown.SetValueWithoutNotify(currentRes);
        fullscreenToggle.isOn = Screen.fullScreen;
    }


    // Settings

    /// <summary>
    /// Accepts the settings changes
    /// </summary>
    public void AcceptSettingsChanges()
    {

        Resolution res = resolutions[resolutionsDropdown.value];
        Screen.SetResolution(res.width, res.height, fullscreenToggle.isOn);

        CloseSettings();
    }


    // Servers

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
        IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
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
    public void StartClient(ServerResponse info)
    {
        networkDiscovery.StopDiscovery();
        NetworkManager.singleton.StartClient(info.uri);
    }

    /// <summary>
    /// Callback for discovering a server
    /// </summary>
    /// <param name="info">The server's info</param>
    public void OnDiscoveredServer(ServerResponse info)
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
