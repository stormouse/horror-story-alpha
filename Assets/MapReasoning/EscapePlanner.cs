using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class EscapeGraph
{
    public static bool Initialized;
    public static HashSet<string> Areas;
    public static Dictionary<string, List<Vector3>> Exits;

    public static void InitializeEscapeGraph()
    {
        Areas = new HashSet<string>();
        Exits = new Dictionary<string, List<Vector3>>();

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
        }

        Initialized = true;
    }
}


public class EscapePlanner : MonoBehaviour {

    public string areaName;

    public Transform enemy;
    public NavMeshAgent agent;


    private void Start()
    {
        if (!EscapeGraph.Initialized)
        {
            EscapeGraph.InitializeEscapeGraph();
        }
    }

    private void Update()
    {
        if (EscapeGraph.Initialized)
        {
            if(Vector3.Distance(transform.position, enemy.position) < 3.0f 
                || !Physics.Raycast(transform.position, enemy.position, Vector3.Distance(transform.position, enemy.position), ~(1<<LayerMask.NameToLayer("Player"))))
            {
                agent.SetDestination(RecommendExit(transform.position, enemy.position, areaName));
                agent.stoppingDistance = 0;
            }
        }
    }

    public Vector3 RecommendExit(Vector3 myPos, Vector3 enemyPos, string areaName)
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

        return myPos;
    }


    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "AreaVolume")
        {
            areaName = other.GetComponent<AreaVolume>().areaName;
        }
    }


}
