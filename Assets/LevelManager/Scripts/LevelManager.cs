using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LevelManager : NetworkBehaviour
{

    public static LevelManager Singleton
    {
        get { return _singleton; }
    }
    protected static LevelManager _singleton = null;

    private void Awake()
    {
        _singleton = this;
    }

    public int m_NumPowerSourceToOpenDoor = 3;
    public DoorControl m_TheDoor;
    public PowerSourceController[] m_PowerSources;

    bool m_GameEnd = false;
    bool m_PowerEnough = false;
    int m_EscapeCount = 0;


    void Start()
    {

        //SpwanAllPowerSources()

        StartCoroutine(GameLoop());
    }


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
        if (isServer && !m_PowerEnough)
        {
            int num = 0;
            for (int i = 0; i < m_PowerSources.Length; i++)
            {
                if (m_PowerSources[i].Charged)
                {
                    num++;
                }
            }
            if (num >= m_NumPowerSourceToOpenDoor)
            {
                RpcOpenDoor();
            }
        }

        return m_PowerEnough;
    }

    [ClientRpc]
    void RpcOpenDoor()
    {
        m_TheDoor.OpenDoor();
        m_PowerEnough = true;
    }

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

}

