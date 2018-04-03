using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class LevelManager : NetworkBehaviour
{

    protected static LevelManager _singleton = null;
    public static LevelManager Singleton { get { return _singleton; } }


    /* character resources */
    private GameObject hunterPrefab;
    private GameObject survivorPrefab;
    private GameObject spectatorPrefab;


    /* game scene resources */
    [Header("Game Resources")]
    public DoorControl[] m_Doors;
    public PowerSourceController[] m_PowerSources;


    /* game arguments */
    [Header("Game Settings")]
    // ~ dying animation duration
    public float timeBeforeDeadBodyDisappear;
    // use longer time to prevent 'null reference deletion' caused by early scene switch
    public float timeBeforeLoadingLobbyAfterGameOver;


    /* game flags */
    bool m_GameEnd = false;
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
        StartCoroutine(GameLoop());
        SetupAiMasterMinds();
    }
	/* zx added for AI */
	/*zx added for AI */
    void SetupAiMasterMinds()
    {
        if (isServer)
        {
			GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
			//Debug.Log ("here1!");
			//Debug.Log (players.Length);
			foreach (GameObject p in players) {
				var ai = p.GetComponent<AIStateController> ();
				//Debug.Log ("here2!");
				if (ai != null) {
					targetPoint = new List<Transform> ();
					foreach (var ps in m_PowerSources) {
						//Debug.Log(ps.transform.position.x);
						targetPoint.Add (ps.transform);
					}
					ai.SetupAI (true, targetPoint, players);
				}
			}
			GameObject[] hunters = GameObject.FindGameObjectsWithTag("Hunter");
			foreach (GameObject p in hunters) {
				var ai = p.GetComponent<_HunterStateController> ();
				//Debug.Log ("here2!");
				if (ai != null) {
					List<PowerSourceController> targetps = new List<PowerSourceController> ();
					foreach (PowerSourceController ps in m_PowerSources) {
						//Debug.Log(ps.transform.position.x);
						targetps.Add (ps);
					}
					ai.SetupAI (true, targetps);
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
            spectatorPrefab = LobbyManager.Singleton.spectatorPrefab;
        }
    }

    #endregion GM_Setup



    #region Game_Progress_Control

    private IEnumerator GameLoop()
    {
        //yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        //yield return StartCoroutine(RoundEnding());

        if (!m_GameEnd)
        {
            StartCoroutine(GameLoop());
        }
        else
        {

        }
    }

    private IEnumerator RoundStarting()
    {
        //reset power source
        //disable player motion
        yield return null;
    }


    private IEnumerator RoundPlaying()
    {
        //enable player motion

        while (!SurvivorAllDead() || !TimesUp() || !SurvivorAllEscaped())
        {
            if (PowerEnough())
            {

            }
            // ... return on the next frame.
            yield return null;
        }
    }


    private IEnumerator RoundEnding()
    {
        // disable player


        yield return null;
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
        return false;
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
        if (survivorObject.GetComponent<AICharacter>() == null)
        {
            var character = survivorObject.GetComponent<NetworkCharacter>();
            var identity = survivorObject.GetComponent<NetworkIdentity>();
            NetworkConnection conn = identity.connectionToClient;

            GameObject spectator = GameObject.Instantiate(spectatorPrefab);
            NetworkServer.ReplacePlayerForConnection(conn, spectator, 0);
            NetworkServer.Spawn(spectator);
        }

        DestoryNetworkObject(survivorObject, timeBeforeDeadBodyDisappear);
    }


    public void DestoryNetworkObject(GameObject obj, float after = 0.0f)
    {
        if (after < 0.01f)
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


    public List<NetworkCharacter> GetAllSurvivorCharacters()
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


    #endregion Game_Control_Interface

}

