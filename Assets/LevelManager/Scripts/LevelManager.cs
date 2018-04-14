using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public enum GameOverReason
{
    None = 0,
    TimeOut,
    Elimination,
    Breakout,
    Error
}


public enum GameState
{
    Waiting = 0,
    Playing,
    Over
}

public class LevelManager : NetworkBehaviour
{

    protected static LevelManager _singleton = null;
    public static LevelManager Singleton { get { return _singleton; } }


    /* character resources */
    private GameObject hunterPrefab;
    private GameObject survivorPrefab;
    private GameObject spectatorPrefab;
    private GameObject survivorSpiritPrefab;


    /* game scene resources */
    [Header("Game Resources")]
    public DoorControl[] m_Doors;
    public PowerSourceController[] m_PowerSources;
    public TreeControl[] m_Trees;

    /* game arguments */
    [Header("Game Settings")]
    // ~ dying animation duration
    public float timeBeforeDeadBodyDisappear;
    // use longer time to prevent 'null reference deletion' caused by early scene switch
    public float timeBeforeLoadingLobbyAfterGameOver;
    // round time
    public float roundTimeInMinute = 0.3f;


    /* game flags */
    public GameState gameState = GameState.Waiting;  // default value should be 'Waiting', currently for debug use
    GameOverReason gameOverReason = GameOverReason.None;

    float roundStartTime;
    bool m_PowerEnough = false;
    bool m_PowerFull = false;
    int m_EscapeCount = 0;

	/* zx modified for AI*/
	private List<Transform> targetPoint;


    private void Awake()
    {
        SetSingleton();
        GetPrefabsFromLobbyManager();
    }

    private void Start()
    {
        SetupAiMasterMinds();

        // TODO: move this out of Start function after finishing Waiting logic
        RoundStarting();
    }


    private void Update()
    {
        if (isServer)
        {
            GameLoop();
        }
    }

	/* zx added for AI */
    void SetupAiMasterMinds()
    {
        if (isServer)
        {
			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
			List<GameObject> survivors = new List<GameObject>();
			List<GameObject> hunters = new List<GameObject>();
			//Debug.Log ("here1!");
			//Debug.Log (players.Length);
			foreach (GameObject p in players) {
				Debug.Log (p.GetComponent<NetworkCharacter> ().Team);   
				if (p.GetComponent<NetworkCharacter> ().Team == GameEnum.TeamType.Survivor) {
					survivors.Add (p);
				} else if(p.GetComponent<NetworkCharacter>().Team == GameEnum.TeamType.Hunter) {
					hunters.Add (p);
				}
			}
			foreach (GameObject p in survivors) {
				
				var ai = p.GetComponent<AIStateController> ();
				//Debug.Log ("here2!");
				if (ai != null) {
					targetPoint = new List<Transform> ();
					foreach (var ps in m_PowerSources) {
						//Debug.Log(ps.transform.position.x);
						targetPoint.Add (ps.transform);
					}
					ai.SetupAI (true, targetPoint, survivors);
				}
			}
			//GameObject[] hunters = GameObject.FindGameObjectsWithTag("Player");
			foreach (GameObject p in hunters) {
				
				var ai = p.GetComponent<AIStateController> ();
				//Debug.Log ("here2!");
				if (ai != null) {
					//List<PowerSourceController> targetps = new List<PowerSourceController> ();
					targetPoint = new List<Transform> ();
					foreach (var ps in m_PowerSources) {
						//Debug.Log(ps.transform.position.x);
						targetPoint.Add (ps.transform);
					}
					ai.SetupAI (true, targetPoint, null);
				}
			}
			/*
            foreach (var p in players)
            {
                var ai = p.GetComponent<ISensible>();
                if (ai != null)
                {
                    ai.Activate();
                }
            }
            */
        }
    }

    #region GM_Setup

    private void SetSingleton()
    {
        _singleton = this;
    }

    private void GetPrefabsFromLobbyManager()
    {
        if (LobbyManager.Singleton)
        {
            hunterPrefab = LobbyManager.Singleton.hunterPrefab;
            survivorPrefab = LobbyManager.Singleton.survivorPrefab;
            spectatorPrefab = LobbyManager.Singleton.spectatorPrefab; // of no use here
            survivorSpiritPrefab = LobbyManager.Singleton.survivorSpiritPrefab;
        }
    }

    #endregion GM_Setup



    #region Game_Progress_Control

    private void GameLoop()
    {
        if(gameState == GameState.Waiting)
        {
            // FreezeTime here;
        }
        else if(gameState == GameState.Playing)
        {
            RoundPlaying();
        }
    }

    // server only
    private void RoundStarting()
    {
        //reset power source
        //disable player motion
        roundStartTime = Time.time;
        gameState = GameState.Playing;
    }

    // server only
    private void RoundPlaying()
    {
        // check if game is over
        if(GameOver(out gameOverReason))
        {
            Debug.Log("Game over, reason = " + gameOverReason.ToString());
        
            gameState = GameState.Over;
            RpcChangeGameState(GameState.Over, gameOverReason);
            RoundEnding();
            return;
        }

        // check if we should open doors
        if (!m_PowerEnough && PowerEnough()) { }
    }

    // server only
    private void RoundEnding()
    {
        // disable player control
        if(gameOverReason == GameOverReason.Elimination)
        {
            Debug.Log("All humans are eliminated.");
        }
        else if(gameOverReason == GameOverReason.Breakout)
        {
            Debug.Log("VIP escaped from the island.");
        }
        else if(gameOverReason == GameOverReason.TimeOut)
        {
            Debug.Log("It's too late for humans to go.");
        }

        if (isServer)
        {
            Invoke("ChangeToRoomScene", 3.0f);
        }
    }

    // server only
    private void ChangeToRoomScene()
    {
        LobbyManager.Singleton.ServerChangeScene(LobbyManager.Singleton.lobbyScene);
    }
    
    [ClientRpc]
    private void RpcChangeGameState(GameState newState, GameOverReason reason)
    {
        if (isServer) return;

        gameState = newState;
        if (gameState == GameState.Over)
        {
            gameOverReason = reason;
            RoundEnding();
        }
    }



    bool PowerEnough()
    {
        if (isServer && !m_PowerFull)
        {
            int num = 0;
            for (int i = 0; i < m_PowerSources.Length; i++)
            {
                if (m_PowerSources[i].Charged)
                {
                    num++;
                }
            }
            bool power = true;
            for (int i = 0; i < m_Doors.Length; i++)
            {
                if (num < m_Doors[i].NumberOfPowerToOpen)
                {
                    power = false;
                }
                if (num >= m_Doors[i].NumberOfPowerToOpen && !m_Doors[i].DoorOpen)
                {
                    m_PowerEnough = true;
                    RpcOpenDoor(i);
                }
            }
            m_PowerFull = power;
        }

        return m_PowerEnough;
    }
    #endregion Game_Progress_Control



    #region Game_State_Checker

    bool SurvivorAllDead()
    {
        return false;
    }

    bool TimesUp()
    {
        return Time.time - roundStartTime > roundTimeInMinute * 60.0f;
    }

    public void PlayerEscape()
    {
        Debug.Log("Player Escape");
        m_EscapeCount++;
    }
    public void PlayerNotEscape()
    {
        Debug.Log("Player Not Escape");
        m_EscapeCount--;
    }

    bool SurvivorAllEscaped()
    {
        return false;
    }


    bool GameOver(out GameOverReason reason)
    {
        if (SurvivorAllDead())
        {
            reason = GameOverReason.Elimination;
            return true;
        }
        if (SurvivorAllEscaped())
        {
            reason = GameOverReason.Breakout;
            return true;
        }
        if (TimesUp())
        {
            reason = GameOverReason.TimeOut;
            return true;
        }
        reason = GameOverReason.None;
        return false;
    }


    #endregion Game_State_Checker



    #region Game_Control_Interface

    [ClientRpc]
    void RpcOpenDoor(int door)
    {
        m_Doors[door].OpenDoor();
        //m_PowerEnough = true;
    }


    public void Observe(Observation observation)
    {
        Debug.Log("Observed: " + observation.subject.name + "'s " + observation.what);
        if (observation.what == Observation.Death)
        {
            KillSurvivor(observation.subject);
        }
    }


    // Destory a survivor and put the player in a spectator
    public void KillSurvivor(GameObject survivorObject)
    {
        Debug.Log("Kill survivor and free the spirit");
        if (survivorObject.GetComponent<AICharacter>() == null)
        {
            var character = survivorObject.GetComponent<NetworkCharacter>();
            var identity = survivorObject.GetComponent<NetworkIdentity>();
            NetworkConnection conn = identity.connectionToClient;

            GameObject spirit = GameObject.Instantiate(survivorSpiritPrefab);
            NetworkServer.ReplacePlayerForConnection(conn, spirit, 0);
            NetworkServer.Spawn(spirit);
        }

        DestoryNetworkObject(survivorObject, timeBeforeDeadBodyDisappear);
    }


    public void DestoryNetworkObject(GameObject obj, float after = 0.0f)
    {
        if (after < 0.001f)
        {
            NetworkServer.Destroy(obj);
        }
        else
        {
            StartCoroutine(DelayDestroy(obj, after));
        }
    }


    private IEnumerator DelayDestroy(GameObject obj, float after)
    {
        yield return new WaitForSeconds(after);
        NetworkServer.Destroy(obj);
    }


    public List<NetworkCharacter> GetAllSurvivorsAlive()
    {
        List<NetworkCharacter> survivors = new List<NetworkCharacter>();
        var players = GameObject.FindGameObjectsWithTag("Player");
        foreach (var player in players)
        {
            var character = player.GetComponent<NetworkCharacter>();
            if (character && character.Team == GameEnum.TeamType.Survivor && character.CurrentState != CharacterState.Dead)
            {
                survivors.Add(character);
            }
        }
        return survivors;
    }


    public TreeControl[] GetTrees()
    {
        return m_Trees;
    }

    #endregion Game_Control_Interface

}

