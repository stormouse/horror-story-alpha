using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using GameEnum;
using UnityEngine.SceneManagement;
using System;



[Serializable]
public class LobbyPlayerSummary
{
    public int index;
    public string name;
    // public string connection; // not useful for clients
    public int team; // uint
    public bool ready;
}

[Serializable]
public class LobbyStateSummary
{
    public int survivorCount;
    public int hunterCount;
    public int spectatorCount;
    public List<LobbyPlayerSummary> playerSummaries;
}


public class SyncLobbyStateMessage : MessageBase
{
    public string data;
}


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
    // server only!
    private HashSet<NetworkConnection> connections = new HashSet<NetworkConnection>();
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
    public GameObject survivorSpiritPrefab = null;
    public GameObject spectatorPrefab = null;

    int survivorSpawned = 0;
    int hunterSpawned = 0;

    private string[] survivorFunnyNames = new string[] { "TRAJAN", "HADRIAN", "ANTONINUS", "MARCUS" };
    private string[] hunterFunnyNames = new string[] { "COMMODUS", "NERO" };


    private void Start()
    {
        s_singleton = this;

        if (roomUIContainer)
        {
            RoomUI.singleton = roomUIContainer.GetComponent<RoomUI>();
        }

        RegisterSpawnablePrefabs();
    }


    // Debug GUI
    /*
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
    */


    // Register runtime spawnables to ClientScene
    private void RegisterSpawnablePrefabs()
    {
        if (hunterPrefab != null)
        {
            ClientScene.RegisterPrefab(hunterPrefab);
        }
        if (survivorPrefab != null)
        {
            ClientScene.RegisterPrefab(survivorPrefab);
        }
        if (spectatorPrefab != null)
        {
            ClientScene.RegisterPrefab(spectatorPrefab);
        }
        if (survivorSpiritPrefab != null)
        {
            ClientScene.RegisterPrefab(survivorSpiritPrefab);
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



    #region State Synchronization

    // summarize a human player
    private LobbyPlayerSummary SummarizePlayer(LobbyPlayer player, int index)
    {
        var summary = new LobbyPlayerSummary();
        summary.index = index;
        summary.name = player.playerName;
        summary.team = (int)player.team;
        summary.ready = player.readyToBegin;
        return summary;
    }

    // summarize an AI player
    private LobbyPlayerSummary SummarizePlayer(string player, TeamType team, int index)
    {
        var summary = new LobbyPlayerSummary();
        summary.index = index;
        summary.name = player;
        summary.team = (int)team;
        summary.ready = true;
        return summary;
    }

    private string SerializeLobbyState()
    {
        LobbyStateSummary state = new LobbyStateSummary();
        state.playerSummaries = new List<LobbyPlayerSummary>();
        state.hunterCount = hunterPlayers.Count + hunterAIs.Count;
        state.survivorCount = survivorPlayers.Count + survivorAIs.Count;
        state.spectatorCount = spectatorPlayers.Count;
        for(int i = 0; i < survivorPlayers.Count; i++)
        {
            state.playerSummaries.Add(SummarizePlayer(survivorPlayers[i], i));
        }
        for(int i = 0; i < survivorAIs.Count; i++)
        {
            state.playerSummaries.Add(SummarizePlayer(survivorAIs[i], TeamType.Survivor, i + survivorPlayers.Count));
        }
        for(int i = 0; i < hunterPlayers.Count; i++)
        {
            state.playerSummaries.Add(SummarizePlayer(hunterPlayers[i], i));
        }
        for(int i = 0; i < hunterAIs.Count; i++)
        {
            state.playerSummaries.Add(SummarizePlayer(hunterAIs[i], TeamType.Hunter, i + hunterPlayers.Count));
        }
        for (int i = 0; i < spectatorPlayers.Count; i++)
        {
            state.playerSummaries.Add(SummarizePlayer(spectatorPlayers[i], i));
        }

        return JsonUtility.ToJson(state);
    }


    public void PushLobbyStateToClient()
    {
        var msg = new SyncLobbyStateMessage();
        msg.data = SerializeLobbyState();
        NetworkServer.SendByChannelToAll(GameNetworkMsg.SyncLobbyState, msg, Channels.DefaultReliable);
    }


    private void UpdateLobbyStateClient(NetworkMessage _msg)
    {
        var state = JsonUtility.FromJson<LobbyStateSummary>(_msg.ReadMessage<SyncLobbyStateMessage>().data);

        // clear everything
        for (int i = 0; i < 4; i++)
        {
            RoomUI.singleton.survivorImages[i].sprite = RoomUI.singleton.emptySlotAvatar;
            RoomUI.singleton.survivorNameTexts[i].text = "";
            RoomUI.singleton.survivorReadyIndicators[i].color = Color.clear;
        }
        for (int i = 0; i < 2; i++)
        {
            RoomUI.singleton.hunterImages[i].sprite = RoomUI.singleton.emptySlotAvatar;
            RoomUI.singleton.hunterNameTexts[i].text = "";
            RoomUI.singleton.hunterReadyIndicators[i].color = Color.clear;
        }
        for (int i = 0; i < 2; i++)
        {
            RoomUI.singleton.spectatorImages[i].color = new Color(0.55f, 0.55f, 0.55f);
            RoomUI.singleton.spectatorNameTexts[i].text = "";
            RoomUI.singleton.spectatorReadyIndicators[i].color = Color.clear;
        }

        // refill slots
        for (int i = 0; i < state.playerSummaries.Count; i++)
        {
            var player = state.playerSummaries[i];
            if (player.team == (int)TeamType.Survivor)
            {
                RoomUI.singleton.survivorImages[player.index].sprite = RoomUI.singleton.survivorAvatar;
                RoomUI.singleton.survivorNameTexts[player.index].text = player.name;
                RoomUI.singleton.survivorReadyIndicators[player.index].color = player.ready ? Color.green : Color.red;
            }
            else if (player.team == (int)TeamType.Hunter)
            {
                RoomUI.singleton.hunterImages[player.index].sprite = RoomUI.singleton.hunterAvatar;
                RoomUI.singleton.hunterNameTexts[player.index].text = player.name;
                RoomUI.singleton.hunterReadyIndicators[player.index].color = player.ready ? Color.green : Color.red;
            }
            else if (player.team == (int)TeamType.Spectator)
            {
                RoomUI.singleton.spectatorImages[player.index].color = Color.white;
                RoomUI.singleton.spectatorNameTexts[player.index].text = player.name;
                RoomUI.singleton.spectatorReadyIndicators[player.index].color = player.ready ? Color.green : Color.red;
            }
        }
    }


    // regenerate player list and sync to clients
    public void UpdateSlots()
    {
        spectatorPlayers.Clear();
        hunterPlayers.Clear();
        survivorPlayers.Clear();

        GameObject[] players = GameObject.FindGameObjectsWithTag("LobbyPlayer");
        foreach (var player in players)
        {
            var lp = player.GetComponent<LobbyPlayer>();
            if (lp == null) continue;
            TeamAdd(lp.team, lp);
        }

        PushLobbyStateToClient();
    }


    #endregion State Synchronization



    #region Team Management

    public void AddSurvivorAI()
    {
        if (!TeamIsFull(TeamType.Survivor))
        {
            TeamAddAI(TeamType.Survivor, survivorFunnyNames[survivorAIs.Count]);
        }
    }


    public void AddHunterAI()
    {
        if (!TeamIsFull(TeamType.Hunter))
        {
            TeamAddAI(TeamType.Hunter, hunterFunnyNames[hunterAIs.Count]);
        }
    }


    public void RemoveSurvivorAI()
    {
        if (survivorAIs.Count > 0) {
            TeamRemoveAI(TeamType.Survivor, survivorAIs[survivorAIs.Count - 1]);
        }
    }

    public void RemoveHunterAI()
    {
        if (hunterAIs.Count > 0)
        {
            TeamRemoveAI(TeamType.Hunter, hunterAIs[hunterAIs.Count - 1]);
        }
    }


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

        PushLobbyStateToClient();
    }

    
    private void TeamAdd(TeamType team, LobbyPlayer player)
    {
        if (team == TeamType.Hunter)
            hunterPlayers.Add(player);
        else if (team == TeamType.Spectator)
            spectatorPlayers.Add(player);
        else if (team == TeamType.Survivor)
            survivorPlayers.Add(player);

        PushLobbyStateToClient();
    }


    private void TeamAddAI(TeamType team, string player)
    {
        if (team == TeamType.Hunter)
            hunterAIs.Add(player);
        else if (team == TeamType.Survivor)
            survivorAIs.Add(player);

        PushLobbyStateToClient();
    }

    private void TeamRemoveAI(TeamType team, string player)
    {
        if (team == TeamType.Hunter)
            hunterAIs.Remove(player);
        else if (team == TeamType.Survivor)
            survivorAIs.Remove(player);

        PushLobbyStateToClient();
    }
    #endregion Team Management



    #region Server Callbacks


    // Auto assign a team for the new lobby player
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        if (connections.Contains(conn)) return;

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
        connections.Add(conn);
        PushLobbyStateToClient();
    }


    // Remove player callback
    public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    {
        base.OnServerRemovePlayer(conn, player);
        connections.Remove(conn);
        PushLobbyStateToClient();
    }

    // Single player ready
    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);
        Debug.Log(conn.ToString() + " is ready");
        // PushLobbyStateToClient(); done by lobby player
    }


    // Called on server when everyone is ready
    public override void OnLobbyServerPlayersReady()
    {
        Debug.Log("Everyone is ready.");
        var canvas = lobbyUIContainer.GetComponent<Canvas>();
        if (canvas) canvas.enabled = false;
        ServerChangeScene(playScene);
    }



    // Called when the scene changed on server
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
        client.RegisterHandler(GameNetworkMsg.SyncLobbyState, UpdateLobbyStateClient);
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



    #region Lobby Hook
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
                character.playerName = lobbyPlayer.GetComponent<LobbyPlayer>().playerName;
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
                character.playerName = lobbyPlayer.GetComponent<LobbyPlayer>().playerName;
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
            character.playerName = ai;
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
            character.playerName = ai;
            survivorSpawned++;
            NetworkServer.Spawn(newPlayer);
        }
    }

    #endregion Lobby Hook


}
