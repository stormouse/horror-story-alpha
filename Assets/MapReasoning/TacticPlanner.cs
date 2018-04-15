using System;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterRole
{
    NA = 0,
    Roamer = 1,
    Defender = 2,
    Hunter = 3,
    Defuser = 4,
    Support = 8,
    Survivor = 12
}


public static class TacticalTask
{
    public static Dictionary<string, TacticalTaskPrototype> prototypes = new Dictionary<string, TacticalTaskPrototype>();

    public static TacticalTaskInstance CreateInstance(string type)
    {
        var instance = new TacticalTaskInstance();
        instance.prototype = prototypes[type];
        return instance;
    }

    public static TacticalTaskInstance CreateInstance(TacticalTaskPrototype prototype)
    {
        var instance = new TacticalTaskInstance();
        instance.prototype = prototype;
        return instance;
    }
}




public class TacticalTaskInstance
{
    public TacticalTaskPrototype prototype;
    public TacticalTaskConfig config;
    public int controllerId;
    public GameObject target;
    public float score;
    public bool done;
}


public static class TacticalTaskType
{
    /* ----- survivors ----- */
    
    // move to and defuse a power source
    public static readonly string Defuse = "Defuse";

    // move to and defuse a power source until it turns red
    public static readonly string FakeDefuse = "FakeDefuse";

    // follow escape path till the chaser is out of sight
    public static readonly string Flee = "Flee";

    // deliberately appear in hunter's sight to lure them
    public static readonly string Distract = "Distract";

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




public static class TacticalPlanner
{
    private static Dictionary<int, TacticalTaskInstance> mostValuableTaskForController = new Dictionary<int, TacticalTaskInstance>();
    private static Dictionary<int, TacticalTaskInstance> currentTaskForController = new Dictionary<int, TacticalTaskInstance>();


    public static TacticalTaskInstance CurrentTask(AIStateController controller)
    {
        int id = controller.GetInstanceID();
        return currentTaskForController[id];
    }


    public static bool ShouldChangeTask(AIStateController controller)
    {
        int id = controller.GetInstanceID();
        return (mostValuableTaskForController[id] != currentTaskForController[id]);
    }


    public static AIState NextState(AIStateController controller)
    {
        int id = controller.GetInstanceID();
        return mostValuableTaskForController[id].prototype.firstState;
    }


    private static TacticalTaskInstance MostValuableTask(AIStateController controller)
    {
        var currentTask = CurrentTask(controller);
        TacticalTaskInstance nextBestTask = currentTask;
        float commitmentBias = 0.25f;

        // re-evaluate currentTask
        currentTask.score = currentTask.prototype.Evaluate(currentTask.config);
        

        if ((controller.characterRole & CharacterRole.Hunter) != CharacterRole.NA)
        {
            if (controller.characterRole == CharacterRole.Roamer)
            {
                var flank = GetFlankInstanceWithBestTarget(controller);
                var followAttack = GetFollowAndAttackInstanceWithBestTarget(controller);
                nextBestTask = flank.score > followAttack.score ? flank : followAttack;
            }
            else if (controller.characterRole == CharacterRole.Defender)
            {
                var guard = GetGuardInstanceWithBestTarget(controller);
                var followAttack = GetFollowAndAttackInstanceWithBestTarget(controller);
                nextBestTask = guard.score > followAttack.score ? guard : followAttack;
            }
        }
        else if ((controller.characterRole & CharacterRole.Survivor) != CharacterRole.NA)
        {
            if (controller.characterRole == CharacterRole.Defuser)
            {
                var defuse = GetDefuseInstanceWithBestTarget(controller);
                var flee = GetFleeInstance(controller);

                nextBestTask = defuse.score > flee.score ? defuse : flee;
            }
            else if (controller.characterRole == CharacterRole.Support)
            {
                var fakeDefuse = GetFakeDefuseInstanceWithBestTarget(controller);
                var flee = GetFleeInstance(controller);
                var trap = GetDeployAmbushInstanceWithBestTarget(controller);

                nextBestTask = fakeDefuse;
                if (flee.score > nextBestTask.score) nextBestTask = flee;
                if (trap.score > nextBestTask.score) nextBestTask = trap;
            }
        }

        if (currentTask.done)
        {
            return nextBestTask;    // should be wander
        }
        else if(currentTask.score + commitmentBias < nextBestTask.score)
        {
            return nextBestTask;
        }
        

        return currentTask;
    }

    // TODO: !!!!

    private static TacticalTaskInstance GetGuardInstanceWithBestTarget(AIStateController controller) {
        throw new NotImplementedException();
    }

    private static TacticalTaskInstance GetFlankInstanceWithBestTarget(AIStateController controller) {
        throw new NotImplementedException();
    }

    private static TacticalTaskInstance GetFollowAndAttackInstanceWithBestTarget(AIStateController controller) {
        throw new NotImplementedException();
    }

    private static TacticalTaskInstance GetDefuseInstanceWithBestTarget(AIStateController controller) {
        throw new NotImplementedException();
    }

    private static TacticalTaskInstance GetFakeDefuseInstanceWithBestTarget(AIStateController controller) {
        throw new NotImplementedException();
    }

    private static TacticalTaskInstance GetFleeInstance(AIStateController controller) {
        throw new NotImplementedException();
    }

    private static TacticalTaskInstance GetDeployAmbushInstanceWithBestTarget(AIStateController controller) {
        throw new NotImplementedException();
    }


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





    //#region Hunter Common
    //public static bool ShouldUseHook(HunterRole me) { return false; }
    //public static Vector3 GetBestChokepoint(HunterRole me) { return Vector3.zero; }
    //public static Vector3 GetBestBunker(HunterRole me) { return Vector3.zero; }
    //#endregion Hunter Common


    //#region Hunter Defender
    //public static Vector3 GetBestOverwatchPosition(HunterDefender me) { return Vector3.zero; }
    //public static Transform GetMostUrgentPowerSource(HunterDefender me) { return null; }
    //#endregion Hunter Defender



    //#region Hunter Roamer
    //public static bool ShouldUseWarSense(HunterRoamer me) { return false; }
    //#endregion Hunter Roamer



    //#region Survivor Common
    //public static bool ShouldUseSmoke(SurvivorRole me) { return false; }
    //public static Vector3 GetNextEscapeWaypoint(SurvivorRole me) { return me.transform.position; }
    //public static Transform GetBestExit(SurvivorRole me) { return null; }
    //#endregion Surivor Common
    

    //#region Survivor Defuser
    //public static Transform GetBestPowerSource(SurvivorDefuser me) { return null; }
    //#endregion Survivor Defuser


    //#region Survivor Support
    //public static NetworkCharacter GetTeammateToSupport(SurvivorSupport me) { return null; }
    //public static NetworkCharacter GetHunterToDistact(SurvivorSupport me) { return null; }
    //public static bool ShouldFakeDefuse(SurvivorSupport me) { return false; }
    //public static bool ShouldUseTrap(SurvivorSupport me) { return false; }
    //public static Vector3 GetFakeMovementDestination(SurvivorSupport me) { return me.transform.position; }
    //public static Vector3 GetSupportivePosition(SurvivorSupport me) { return me.transform.position; }
    //#endregion Survivor Support
}