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
    public int maxSpectatorPlayers = 1;
    public int maxHunterPlayers = 2;
    public int maxSurvivorPlayers = 4;


    /* lobby data */
    // private LobbyPlayer[] lobbyPlayers; // use NetworkLobbyManager.lobbySlots;
    private List<LobbyPlayer> spectatorPlayers = new List<LobbyPlayer>();
    private List<LobbyPlayer> hunterPlayers = new List<LobbyPlayer>();
    private List<LobbyPlayer> survivorPlayers = new List<LobbyPlayer>();

    public GameObject lobbyUIContainer = null;
    public GameObject hunterPrefab = null;
    public GameObject survivorPrefab = null;
    public GameObject spectatorPrefab = null;

    int lastSpawnpointIndex = -1;

    private void Start()
    {
        s_singleton = this;

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

        GUILayout.Label("Survivors:");
        foreach(var p in survivorPlayers)
        {
            GUILayout.Label("Player " + p.netId.ToString());
        }
    }
    
    public bool TeamIsFull(TeamType team)
    {
        if (team == TeamType.Hunter)
            return hunterPlayers.Count >= maxHunterPlayers;
        else if (team == TeamType.Survivor)
            return survivorPlayers.Count >= maxSurvivorPlayers;
        else if (team == TeamType.Spectator)
            return spectatorPlayers.Count >= maxSpectatorPlayers;
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
    }

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
            var canvas = GameObject.Find("Lobby UI").GetComponent<Canvas>();
            if (canvas) canvas.enabled = false;
            foreach (var p in lobbySlots)
            {
                if(p.GetComponent<LobbyPlayer>())
                    p.GetComponent<LobbyPlayer>().RpcReadyForPlayScene();
            }
            ServerChangeScene(playScene);
        }
    }

    public override void OnLobbyClientSceneChanged(NetworkConnection conn)
    {
        base.OnLobbyClientSceneChanged(conn);

        if (SceneManager.GetActiveScene().name == playScene)
        {
            if (lobbyUIContainer)
            {
                lobbyUIContainer.SetActive(false);
            }
        }
    }

    // lobby hook
    public override bool OnLobbyServerSceneLoadedForPlayer(GameObject lobbyPlayer, GameObject gamePlayer)
    {
        if (lobbyUIContainer)
        {
            lobbyUIContainer.SetActive(false);
        }

        var spawnPoints = FindObjectsOfType<NetworkStartPosition>();
        if (lobbyPlayer.GetComponent<LobbyPlayer>().team == TeamType.Hunter)
        {
            var newPlayer = Instantiate(hunterPrefab);
            if (spawnPoints.Length > 0)
            {
                int i = (lastSpawnpointIndex + 1) % spawnPoints.Length;
                newPlayer.transform.position = spawnPoints[i].transform.position;
                newPlayer.transform.rotation = Quaternion.identity;
                var character = newPlayer.GetComponent<NetworkCharacter>();
                // assert character!
                character.SetTeam(TeamType.Hunter);
                lastSpawnpointIndex = i;
            }
            NetworkServer.Spawn(newPlayer);
            NetworkServer.Destroy(gamePlayer);
            NetworkServer.ReplacePlayerForConnection(lobbyPlayer.GetComponent<NetworkIdentity>().connectionToClient, newPlayer, 0);
        }
        else if (lobbyPlayer.GetComponent<LobbyPlayer>().team == TeamType.Survivor)
        {
            var newPlayer = Instantiate(survivorPrefab);
            if (spawnPoints.Length > 0)
            {
                int i = (lastSpawnpointIndex + 1) % spawnPoints.Length;
                newPlayer.transform.position = spawnPoints[i].transform.position;
                newPlayer.transform.rotation = Quaternion.identity;
                var character = newPlayer.GetComponent<NetworkCharacter>();
                // assert character!
                character.SetTeam(TeamType.Survivor);
                lastSpawnpointIndex = i;
            }
            NetworkServer.Spawn(newPlayer);
            NetworkServer.Destroy(gamePlayer);
            NetworkServer.ReplacePlayerForConnection(lobbyPlayer.GetComponent<NetworkIdentity>().connectionToClient, newPlayer, 0);
        }

        return false; //lobbyPlayer.GetComponent<LobbyPlayer>().team != TeamType.Spectator;
    }

}
