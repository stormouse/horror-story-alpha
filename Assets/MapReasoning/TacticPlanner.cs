using System.Collections.Generic;
using UnityEngine;


public struct TacticalTask
{
    public string taskType;
    public Transform target;
    public float value;
}


public static class TacticalTaskType
{
    /* ----- survivors ----- */
    
    // move to and defuse a power source
    public static readonly string Defuse = "Defuse";

    // move to and defuse a power source until it turns red
    public static readonly string FakeDefuse = "FakeDefuse";

    // follow escape path till the chaser is out of sight
    public static readonly string Escape = "Escape";

    // deliberately appear in hunter's sight to lure them
    public static readonly string FakeMove = "FakeMove";

    // move to a choke point and deploy a trap
    public static readonly string DeployAmbush = "DeployAmbush";

    // move to a certain position and release smoke
    public static readonly string ProvideCover = "ProvideCover";



    /* ----- hunters ----- */

    // move to an objective(ps/door) and defend
    public static readonly string Guard = "Guard";

    // move between several objective(ps/door)
    public static readonly string Patrol = "Patrol";

    // move to a overwatch point and wander for a while
    public static readonly string Overwatch = "Overwatch";

    // predict and move ahead of an opponent
    public static readonly string Flank = "Flank";

    // follow an opponent and try to beat/hook&beat it to death (hunter instinct)
    public static readonly string FollowAndAttack = "FollowAndAttack";

    // move to and hide behind in bunker and wait for opponents
    public static readonly string Camp = "Camp";

}



public static class TacticalGraph
{
    public static bool Initialized = false;


    #region Graph Components

    public static List<PowerSourceController> powerSources;
    public static List<Vector3> chokePoints;
    public static List<Vector3> tacticalPoints;
    public static List<Vector3> bunkers;
    public static List<Vector3> overwatchPoints;



    #endregion Graph Components


    #region Graph Maintenance
    public static void UpdateGraph() { }

    #endregion Graph maintenance





    #region Hunter Common
    public static bool ShouldUseHook(HunterRole me) { return false; }
    public static Vector3 GetBestChokepoint(HunterRole me) { return Vector3.zero; }
    public static Vector3 GetBestBunker(HunterRole me) { return Vector3.zero; }
    #endregion Hunter Common


    #region Hunter Defender
    public static Vector3 GetBestOverwatchPosition(HunterDefender me) { return Vector3.zero; }
    public static Transform GetMostUrgentPowerSource(HunterDefender me) { return null; }
    #endregion Hunter Defender



    #region Hunter Roamer
    public static bool ShouldUseWarSense(HunterRoamer me) { return false; }
    #endregion Hunter Roamer



    #region Survivor Common
    public static bool ShouldUseSmoke(SurvivorRole me) { return false; }
    public static Vector3 GetNextEscapeWaypoint(SurvivorRole me) { return me.transform.position; }
    public static Transform GetBestExit(SurvivorRole me) { return null; }
    #endregion Surivor Common
    

    #region Survivor Defuser
    public static Transform GetBestPowerSource(SurvivorDefuser me) { return null; }
    #endregion Survivor Defuser


    #region Survivor Support
    public static NetworkCharacter GetTeammateToSupport(SurvivorSupport me) { return null; }
    public static NetworkCharacter GetHunterToDistact(SurvivorSupport me) { return null; }
    public static bool ShouldFakeDefuse(SurvivorSupport me) { return false; }
    public static bool ShouldUseTrap(SurvivorSupport me) { return false; }
    public static Vector3 GetFakeMovementDestination(SurvivorSupport me) { return me.transform.position; }
    public static Vector3 GetSupportivePosition(SurvivorSupport me) { return me.transform.position; }
    #endregion Survivor Support
}