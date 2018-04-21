using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class EscapeGraph
{
    public static bool Initialized;
    public static HashSet<string> Areas;
    public static Dictionary<string, List<Vector3>> Exits;
    public static List<Vector3> AllExits;

    public static void InitializeDoorGraph()
    {
        Areas = new HashSet<string>();
        Exits = new Dictionary<string, List<Vector3>>();
        AllExits = new List<Vector3>();

        var areaVolumes = GameObject.FindObjectsOfType<AreaVolume>();
        for (int i = 0; i < areaVolumes.Length; i++)
        {
            if (!Areas.Contains(areaVolumes[i].areaName))
            {
                Areas.Add(areaVolumes[i].areaName);
                Exits.Add(areaVolumes[i].areaName, new List<Vector3>());
            }
        }

        var doors = GameObject.FindObjectsOfType<Door>();
        for (int i = 0; i < doors.Length; i++)
        {
            Exits[doors[i].areasJoined[0]].Add(doors[i].transform.position + doors[i].exitOffset[0]);
            Exits[doors[i].areasJoined[1]].Add(doors[i].transform.position + doors[i].exitOffset[1]);
            AllExits.Add(doors[i].transform.position);
        }

        Initialized = true;
    }


    public static void InitializeExitGraph()
    {
        Areas = new HashSet<string>();
        Exits = new Dictionary<string, List<Vector3>>();
        AllExits = new List<Vector3>();

        var areaVolumes = GameObject.FindObjectsOfType<AreaVolume>();
        for (int i = 0; i < areaVolumes.Length; i++)
        {
            if (!Areas.Contains(areaVolumes[i].areaName))
            {
                Areas.Add(areaVolumes[i].areaName);
                Exits.Add(areaVolumes[i].areaName, new List<Vector3>());
            }
        }

        var exits = GameObject.FindObjectsOfType<ExitMarker>();
        for(int i = 0; i < exits.Length; i++)
        {
            for (int j = 0; j < exits[i].areaNames.Length; j++)
            {
                Exits[exits[i].areaNames[j]].Add(exits[i].transform.position);
            }
            AllExits.Add(exits[i].transform.position);
        }

        Initialized = true;

        Debug.Log("Escape Planner Initialized.");
    }


    public static Vector3 GetDestination(Vector3 myPos, Vector3 enemyPos, string areaName)
    {
        if (EscapeGraph.Areas.Contains(areaName))
        {
            Vector3 bestExitPos = enemyPos;
            float bestDiff = -10000.0f;
            foreach (var exitPos in EscapeGraph.Exits[areaName])
            {
                float diff = Vector3.Distance(enemyPos, exitPos) - Vector3.Distance(myPos, exitPos);
                if (diff > bestDiff)
                {
                    bestExitPos = exitPos;
                    bestDiff = diff;
                }
            }

            return bestExitPos;
        }

        return (myPos - enemyPos).normalized * 3.0f;
    }


    public static Vector3 GetDestination(Vector3 myPos, Vector3 myForward, List<Transform> enemies, string areaName)
    {
        Vector3 bestExit = myPos;

        if (areaName == "")
        {
            return myPos;
        }
        
        else
        {
            int n = Exits[areaName].Count;
            float bestScore = -2000.0f;
            for (int i = 0; i < n; i++)
            {
                // waypoint too far away, move it out of consideration
                if (Vector3.Distance(myPos, Exits[areaName][i]) > 30.0f)
                {
                    if (bestScore < -1000.0f)
                    {
                        bestScore = -1000.0f;
                        bestExit = Exits[areaName][i];
                    }
                    continue;
                }


                float d_me = Vector3.Distance(myPos, Exits[areaName][i]);
                float d_wolf = 1000.0f;
                for (int j = 0; j < enemies.Count; j++)
                {
                    d_wolf = Mathf.Min(Vector3.Distance(enemies[j].position, Exits[areaName][i]), d_wolf);
                }

                if(enemies.Count == 0)
                {
                    d_wolf = Vector3.Distance(myPos - myForward * 10.0f, Exits[areaName][i]);
                }

                float score = (d_wolf - d_me);


                if (score > bestScore)
                {
                    bestScore = score;
                    bestExit = Exits[areaName][i];
                }
            }

        }

        return bestExit;
    }

}


public class EscapePlanner : MonoBehaviour {

    public string areaName;
    
    private void Start()
    {
        if (!EscapeGraph.Initialized)
        {
            EscapeGraph.InitializeExitGraph();
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "AreaVolume")
        {
            areaName = other.GetComponent<AreaVolume>().areaName;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //if(other.tag == "AreaVolume")
        //{
        //    if(areaName == other.GetComponent<AreaVolume>().areaName)
        //    {
        //        areaName = "";
        //    }
        //}
    }

}
