using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using GameEnum;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkLobbyManager {

    /* make singleton */
    public static LobbyManager Singleton { get { return s_singleton; } }
    private static LobbyManager s_singleton;


    /* lobby settings */
    public int maxSpectatorPlayers = 2;
    public int maxHunterPlayers = 2;
    public int maxSurvivorPlayers = 4;

    /* lobby status */
    public bool hasConnection = false;

    /* lobby data */
    // private LobbyPlayer[] lobbyPlayers; // use NetworkLobbyManager.lobbySlots;
    private List<LobbyPlayer> spectatorPlayers = new List<LobbyPlayer>();
    private List<LobbyPlayer> hunterPlayers = new List<LobbyPlayer>();
    private List<string> hunterAIs = new List<string>();
    private List<LobbyPlayer> survivorPlayers = new List<LobbyPlayer>();
    private List<string> survivorAIs = new List<string>();

    public GameObject lobbyUIContainer = null;
    public GameObject roomUIContainer = null;
    public GameObject hunterPrefab = null;
    public GameObject hunterAiPrefab = null;
    public GameObject survivorPrefab = null;
    public GameObject survivorAiPrefab = null;
    public GameObject spectatorPrefab = null;

    int survivorSpawned = 0;
    int hunterSpawned = 0;

    private void Start()
    {
        s_singleton = this;

        if (roomUIContainer)
        {
            RoomUI.singleton = roomUIContainer.GetComponent<RoomUI>();
        }

        if(hunterPrefab != null)
        {
            ClientScene.RegisterPrefab(hunterPrefab);
        }
        if(survivorPrefab != null)
        {
            ClientScene.RegisterPrefab(survivorPrefab);
        }
        if(spectatorPrefab != null)
        {
            ClientScene.RegisterPrefab(spectatorPrefab);
        }
        if (hunterAiPrefab != null)
        {
            ClientScene.RegisterPrefab(hunterAiPrefab);
        }
        if (survivorAiPrefab != null)
        {
            ClientScene.RegisterPrefab(survivorAiPrefab);
        }
    }


    private void OnGUI()
    {
        int i = 0;

        GUILayout.Label("Spectators:"); 
        foreach(var p in spectatorPlayers)
        {
            GUILayout.Label("Player " + p.netId.ToString());
            i++;
        }

        GUILayout.Label("Hunters:");
        foreach(var p in hunterPlayers)
        {
            GUILayout.Label("Player " + p.netId.ToString());
            i++;
        }
        foreach(var p in hunterAIs)
        {
            GUILayout.Label(p);
        }

        GUILayout.Label("Survivors:");
        foreach(var p in survivorPlayers)
        {
            GUILayout.Label("Player " + p.netId.ToString());
        }
        foreach (var p in survivorAIs)
        {
            GUILayout.Label(p);
        }
    }


    public void AddSurvivorAI()
    {
        if (!TeamIsFull(TeamType.Survivor))
        {
            TeamAddAI(TeamType.Survivor, "AI_Survivor_" + (survivorAIs.Count + 1).ToString());
        }
    }


    public void AddHunterAI()
    {
        if (!TeamIsFull(TeamType.Hunter))
        {
            TeamAddAI(TeamType.Hunter, "AI_Hunter_" + (hunterAIs.Count + 1).ToString());
        }
    }



    #region Team Management
    public bool TeamIsFull(TeamType team)
    {
        if (team == TeamType.Hunter)
        {
            return hunterPlayers.Count + hunterAIs.Count >= maxHunterPlayers;
        }
        else if (team == TeamType.Survivor)
        {
            return survivorPlayers.Count + survivorAIs.Count >= maxSurvivorPlayers;
        }
        else if (team == TeamType.Spectator)
        {
            return spectatorPlayers.Count >= maxSpectatorPlayers;
        }

        return false;
    }


    public void SwitchToTeam(LobbyPlayer player, TeamType team)
    {
        Debug.Log("Server: SwitchToTeam");

        player.team = team; // trigger SyncVar

        UpdateSlots();
    }

    
    private void TeamRemove(TeamType team, LobbyPlayer player)
    {
        if (team == TeamType.Hunter)
            hunterPlayers.Remove(player);
        else if (team == TeamType.Spectator)
            spectatorPlayers.Remove(player);
        else if (team == TeamType.Survivor)
            survivorPlayers.Remove(player);
    }

    
    private void TeamAdd(TeamType team, LobbyPlayer player)
    {
        if (team == TeamType.Hunter)
            hunterPlayers.Add(player);
        else if (team == TeamType.Spectator)
            spectatorPlayers.Add(player);
        else if (team == TeamType.Survivor)
            survivorPlayers.Add(player);
    }


    private void TeamAddAI(TeamType team, string player)
    {
        if (team == TeamType.Hunter)
            hunterAIs.Add(player);
        else if (team == TeamType.Survivor)
            survivorAIs.Add(player);
        UpdateSlots();
    }

    private void TeamRemoveAI(TeamType team, string player)
    {
        if (team == TeamType.Hunter)
            hunterAIs.Remove(player);
        else if (team == TeamType.Survivor)
            survivorAIs.Remove(player);
        UpdateSlots();
    }


    public void UpdateSlots()
    {
        Debug.Log("UpdateSlots Called");

        spectatorPlayers.Clear();
        hunterPlayers.Clear();
        survivorPlayers.Clear();

        GameObject[] players = GameObject.FindGameObjectsWithTag("LobbyPlayer");
        foreach(var player in players)
        {
            var lp = player.GetComponent<LobbyPlayer>();
            if (lp == null) continue;
            switch (lp.team)
            {
                case TeamType.Spectator:
                    spectatorPlayers.Add(lp);
                    break;
                case TeamType.Hunter:
                    hunterPlayers.Add(lp);
                    break;
                case TeamType.Survivor:
                    survivorPlayers.Add(lp);
                    break;
                default:
                    break;
            }
        }

        for(int i = 0; i < survivorAIs.Count + survivorPlayers.Count; i++)
        {
            RoomUI.singleton.survivorImages[i].sprite = RoomUI.singleton.survivorAvatar;
        }
        for(int i = survivorAIs.Count + survivorPlayers.Count; i < maxSurvivorPlayers; i++)
        {
            RoomUI.singleton.survivorImages[i].sprite = RoomUI.singleton.emptySlotAvatar;
        }

        for (int i = 0; i < hunterAIs.Count + hunterPlayers.Count; i++)
        {
            RoomUI.singleton.hunterImages[i].sprite = RoomUI.singleton.hunterAvatar;
        }
        for (int i = hunterAIs.Count + hunterPlayers.Count; i < maxHunterPlayers; i++)
        {
            RoomUI.singleton.hunterImages[i].sprite = RoomUI.singleton.emptySlotAvatar;
        }

        for(int i = 0; i < spectatorPlayers.Count; i++)
        {
            RoomUI.singleton.spectatorImages[i].color = Color.white;
        }
        for(int i=spectatorPlayers.Count; i < maxSpectatorPlayers; i++)
        {
            RoomUI.singleton.spectatorImages[i].color = new Color(0.55f, 0.55f, 0.55f);
        }
        
    }
    #endregion Team Management



    #region Server Callbacks
    // ---- Server Callbacks

    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        GameObject player = Instantiate(lobbyPlayerPrefab.gameObject, Vector3.zero, Quaternion.identity);
        if (!TeamIsFull(TeamType.Hunter))
        {
            player.GetComponent<LobbyPlayer>().team = TeamType.Hunter;
            hunterPlayers.Add(player.GetComponent<LobbyPlayer>());
        }
        else
        {
            player.GetComponent<LobbyPlayer>().team = TeamType.Survivor;
            survivorPlayers.Add(player.GetComponent<LobbyPlayer>());
        }
        NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        UpdateSlots();

        Debug.Log("Server: OnServerAddPlayer");
    }

    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        base.OnServerRemovePlayer(conn, player);
        UpdateSlots();
    }

    public override void OnLobbyServerPlayersReady()
    {
        base.OnLobbyServerPlayersReady();

        bool allready = true;
        for(int i = 0; i < lobbySlots.Length; i++)
        {
            if (lobbySlots[i])
                allready &= lobbySlots[i].readyToBegin;
        }
        if (allready)
        {
            var canvas = lobbyUIContainer.GetComponent<Canvas>();
            if (canvas) canvas.enabled = false;
            foreach (var p in lobbySlots)
            {
                if(p.GetComponent<LobbyPlayer>())
                    p.GetComponent<LobbyPlayer>().RpcReadyForPlayScene();
            }
            ServerChangeScene(playScene);
        }
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        if (sceneName == playScene)
        {
            InitializeServerOnlyObjects();
        }
        else if(sceneName == lobbyScene)
        {
            hunterSpawned = 0;
            survivorSpawned = 0;
        }
    }
    #endregion Server Callbacks



    #region Client Callbacks

    public override void OnLobbyClientConnect(NetworkConnection conn)
    {
        base.OnLobbyClientConnect(conn);
        hasConnection = true;
        if (lobbyUIContainer)
        {
            lobbyUIContainer.SetActive(false);
        }
    }


    public override void OnLobbyClientDisconnect(NetworkConnection conn)
    {
        base.OnLobbyClientDisconnect(conn);
        hasConnection = false;
        if (lobbyUIContainer)
        {
            lobbyUIContainer.SetActive(true);
        }
        if (roomUIContainer)
        {
            roomUIContainer.SetActive(false);
        }
    }

    public override void OnLobbyClientSceneChanged(NetworkConnection conn)
    {
        base.OnLobbyClientSceneChanged(conn);

        if(SceneManager.GetActiveScene().name == lobbyScene)
        {
            if (lobbyUIContainer)
            {
                lobbyUIContainer.SetActive(!hasConnection);
            }
            if (roomUIContainer)
            {
                roomUIContainer.SetActive(hasConnection);
            }
        }

        if (SceneManager.GetActiveScene().name == playScene)
        {
            if (lobbyUIContainer)
            {
                lobbyUIContainer.SetActive(false);
            }
            if (roomUIContainer)
            {
                roomUIContainer.SetActive(false);
            }
        }
    }
    #endregion Client Callbacks


    // --------- lobby hook -------
    /// <summary>
    /// Callback function when the player scene is loaded on server
    /// also the same time when the networked player object is about to spawned
    /// </summary>
    /// <param name="lobbyPlayer"></param>
    /// <param name="gamePlayer"></param>
    /// <returns></returns>
    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        var spManager = FindObjectOfType<SpawnpointManager>();

        // Spawn player object according to team selection
        if (lobbyPlayer.GetComponent<LobbyPlayer>().team == TeamType.Hunter)
        {
            var newPlayer = Instantiate(hunterPrefab);
            if (spManager != null)
            {
                int i = hunterSpawned % spManager.hunterSpawnpoints.Length;
                newPlayer.transform.position = spManager.hunterSpawnpoints[i].transform.position;
                newPlayer.transform.rotation = Quaternion.identity;
                var character = newPlayer.GetComponent<NetworkCharacter>();
                character.SetTeam(TeamType.Hunter);
                hunterSpawned++;
            }
            NetworkServer.Spawn(newPlayer);
            NetworkServer.Destroy(gamePlayer);
            NetworkServer.ReplacePlayerForConnection(lobbyPlayer.GetComponent<NetworkIdentity>().connectionToClient, newPlayer, 0);
        }

        else if (lobbyPlayer.GetComponent<LobbyPlayer>().team == TeamType.Survivor)
        {
            var newPlayer = Instantiate(survivorPrefab);
            if (spManager != null)
            {
                int i = survivorSpawned % spManager.survivorSpawnpoints.Length;
                newPlayer.transform.position = spManager.survivorSpawnpoints[i].transform.position;
                newPlayer.transform.rotation = Quaternion.identity;
                var character = newPlayer.GetComponent<NetworkCharacter>();
                character.SetTeam(TeamType.Survivor);
                survivorSpawned++;
            }
            NetworkServer.Spawn(newPlayer);
            NetworkServer.Destroy(gamePlayer);
            NetworkServer.ReplacePlayerForConnection(lobbyPlayer.GetComponent<NetworkIdentity>().connectionToClient, newPlayer, 0);
        }
        else if (lobbyPlayer.GetComponent<LobbyPlayer>().team == TeamType.Spectator)
        {
            var newPlayer = Instantiate(spectatorPrefab);
            newPlayer.transform.position = Vector3.up * 5.0f;
            NetworkServer.Spawn(newPlayer);
            NetworkServer.Destroy(gamePlayer);
            NetworkServer.ReplacePlayerForConnection(lobbyPlayer.GetComponent<NetworkIdentity>().connectionToClient, newPlayer, 0);
        }
        return false;
    }


    void InitializeServerOnlyObjects()
    {
        var spManager = FindObjectOfType<SpawnpointManager>();
        // Spawn ai players
        foreach (var ai in hunterAIs)
        {
            var newPlayer = Instantiate(hunterAiPrefab);
            int i = hunterSpawned % spManager.hunterSpawnpoints.Length;
            newPlayer.transform.position = spManager.hunterSpawnpoints[i].transform.position;
            newPlayer.transform.rotation = Quaternion.identity;
            var character = newPlayer.GetComponent<NetworkCharacter>();
            character.SetTeam(TeamType.Hunter);
            hunterSpawned++;
            NetworkServer.Spawn(newPlayer);
        }

        foreach (var ai in survivorAIs)
        {
            var newPlayer = Instantiate(survivorAiPrefab);
            int i = survivorSpawned % spManager.survivorSpawnpoints.Length;
            newPlayer.transform.position = spManager.survivorSpawnpoints[i].transform.position;
            newPlayer.transform.rotation = Quaternion.identity;
            var character = newPlayer.GetComponent<NetworkCharacter>();
            character.SetTeam(TeamType.Survivor);
            survivorSpawned++;
            NetworkServer.Spawn(newPlayer);
        }
    }

    

}
