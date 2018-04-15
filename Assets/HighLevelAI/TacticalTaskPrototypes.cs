using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class TacticalTaskConfig {
    public GameObject target;
    public AIStateController controller;
    public List<AIStateController> teammates;
    public List<GameObject> enemies;

    public static TacticalTaskConfig MakeFrom(AIStateController controller)
    {
        throw new NotImplementedException();
    }
}


public abstract class TacticalTaskPrototype
{
    public string type;
    public CharacterRole applicableMask;
    public AIState firstState;

    public bool Applicable(CharacterRole characterMask)
    {
        return ((characterMask & applicableMask) != CharacterRole.NA);
    }

    public abstract float Evaluate(TacticalTaskConfig config);
}


public class FollowAndAttackPrototype : TacticalTaskPrototype
{
    private Dictionary<int, float> lastSeenTargetTime;
    public float timeFollowProcessProlongs = 3.0f;

    public FollowAndAttackPrototype()
    {
        type = TacticalTaskType.FollowAndAttack;
        applicableMask = CharacterRole.Hunter;
        // firstState = TacticalTask.StateRefs[type];

        // prolong the following method for a bit longer
        lastSeenTargetTime = new Dictionary<int, float>();
    }


    public override float Evaluate(TacticalTaskConfig config)
    {
        var target = config.target;
        var controller = config.controller;
        var controllerID = controller.GetInstanceID();
        var now = Time.time;

        float distanceScore = 1.0f / Mathf.Min(Vector3.Distance(target.transform.position, controller.transform.position), 1.0f);
        if (controller.visibleTargets.Contains(target.transform))
        {
            lastSeenTargetTime[controllerID] = now;
        }

        if(now - lastSeenTargetTime[controllerID] > timeFollowProcessProlongs)
        {
            return 0.0f;
        }

        float initiative = 0.3f;
        float distanceWeight = 5.0f;
        float finalScore = initiative + distanceWeight * distanceScore;

        return Mathf.Clamp01(finalScore);
    }
}


public class FlankPrototype : TacticalTaskPrototype
{
    public FlankPrototype()
    {
        type = TacticalTaskType.Flank;
        applicableMask = CharacterRole.Roamer;
        // firstState = TacticalTask.StateRefs[type];
    }

    public override float Evaluate(TacticalTaskConfig config)
    {
        var target = config.target; // here target is an area node
        var controller = config.controller;

        float distanceScore = 1.0f / (1.0f + Vector3.Distance(controller.transform.position, target.transform.position));
        float bootyScore = 0.0f;  // target.bootyScore

        float initiative = 0.1f;
        float distanceWeight = 0.3f;
        float bootyWeight = 0.5f;
        float finalScore = initiative + distanceWeight * distanceScore + bootyWeight * bootyScore;

        return Mathf.Clamp01(finalScore);
    }
}


public class GuardPrototype : TacticalTaskPrototype
{

    public float friendlyCoverRange = 20.0f;
    public float importantDistanceThreshold = 50.0f;

    public GuardPrototype()
    {
        type = TacticalTaskType.Guard;
        applicableMask = CharacterRole.Defender;
        // firstState = TacticalTask.StateRefs[type];
    }

    
    public override float Evaluate(TacticalTaskConfig config)
    {
        // assert target != null
        // assert enemis.Count > 0

        
        var target = config.target; // here target is a power source controller, or exit
        var controller = config.controller;
        var teammates = config.teammates;
        var enemies = config.enemies;
        var ps = target.GetComponent<PowerSourceController>();
        bool isExit = (ps == null);

        
        float shortestDistanceFromEnemy = importantDistanceThreshold;
        for(int i = 0; i < enemies.Count; i++)
        {
            var dist = Vector3.Distance(enemies[i].transform.position, target.transform.position);
            if(dist < shortestDistanceFromEnemy)
            {
                shortestDistanceFromEnemy = dist;
            }
        }

        float importanceScore;
        if (isExit)
            importanceScore = 1.0f - shortestDistanceFromEnemy / (importantDistanceThreshold + 1.0f);
        else
            importanceScore = (1.0f - shortestDistanceFromEnemy / (importantDistanceThreshold + 1.0f)) * 0.5f + (ps.CurrentPower / ps.m_MaxPower) * 0.5f;
        


        float distanceScore = 1.0f / (1.0f + Vector3.Distance(target.transform.position, controller.transform.position));



        bool friendlyCovered = false;
        for(int i = 0; i < teammates.Count; i++)
        {
            if(Vector3.Distance(teammates[i].transform.position, target.transform.position) < friendlyCoverRange)
            {
                friendlyCovered = true;
                break;
            }
        }
        float friendlyScore = friendlyCovered ? -1.0f : 1.0f;


        float initiative = Random.Range(0.0f, 0.2f);
        float distanceWeight = -0.1f;
        float importanceWeight = 0.3f;
        float friendlyWeight = -0.5f;
        float finalScore = initiative + distanceWeight * distanceScore + importanceWeight * importanceScore + friendlyWeight * friendlyScore;

        return Mathf.Clamp01(finalScore);
    }
}


public class DefusePrototype : TacticalTaskPrototype
{

    public DefusePrototype()
    {
        type = TacticalTaskType.Defuse;
        applicableMask = CharacterRole.Defuser;
        // firstState = TacticalTask.StateRefs[type];
    }

    public override float Evaluate(TacticalTaskConfig config)
    {
        // unpack config
        var target = config.target;
        var controller = config.controller;
        var teammates = config.teammates;
        var enemies = config.enemies;


        // score wrt distance
        float distanceScore = 1.0f / Mathf.Min(Vector3.Distance(target.transform.position, controller.transform.position), 1.0f);

        // score wrt duplicate work
        bool duplicate = false;

        for(int i = 0; i < teammates.Count; i++)
        {
            // check if this teammate is doing the same
            var task = TacticalPlanner.CurrentTask(teammates[i]);
            if(task.prototype.type == this.type && task.target == target)
            {
                duplicate = true;
                break;
            }
        }
        float duplicateScore = duplicate ? 0.0f : 1.0f;


        // score wrt danger
        float dangerScoreBase = 0.25f;
        float dangerScore = dangerScoreBase;
        for(int i = 0; i < enemies.Count; i++)
        {
            if(Vector3.Distance(enemies[i].transform.position, target.transform.position) < 20.0f)
            {
                dangerScore *= 2.0f;
            }
        }
        dangerScore -= dangerScoreBase;


        // score wrt easiness
        var ps = target.GetComponent<PowerSourceController>();
        float easinessScore = (ps.CurrentPower / ps.m_MaxPower);


        float initiative = Random.Range(0.0f, 0.3f);
        float distanceWeight = 3.0f;
        float duplicateWeight = -0.9f;
        float dangerWeight = -0.5f;
        float easinessWeight = 0.5f;

        float finalScore = initiative + distanceWeight * distanceScore + duplicateWeight * duplicateScore + dangerWeight * dangerScore + easinessWeight * easinessScore;

        return Mathf.Clamp01(finalScore);
    }
}


public class FleePrototype : TacticalTaskPrototype
{
    private Dictionary<int, float> lastTimeSeenChased;

    public float timeFleeProcessProlongs = 8.0f;

    public FleePrototype()
    {
        type = TacticalTaskType.Flee;
        applicableMask = CharacterRole.Survivor;
        // firstState = TacticalTask.StateRefs[type];

        // record last time the character saw the chaser
        // used to prolong the flee process to avoid potential flanks
        lastTimeSeenChased = new Dictionary<int, float>();

    }

    public override float Evaluate(TacticalTaskConfig config)
    {
        var controller = config.controller;
        var controllerID = controller.GetInstanceID();
        var enemies = config.enemies;
        var now = Time.time;

        for(int i = 0; i < enemies.Count; i++)
        {
            if (controller.visibleTargets.Contains(enemies[i].transform))
            {
                var rigidbody = enemies[i].GetComponent<Rigidbody>();
                var angleDiff = Vector3.Angle(rigidbody.velocity.normalized, controller.transform.forward);
                if(angleDiff < 15.0f)
                {
                    lastTimeSeenChased[controllerID] = now;
                }
            }
        }

        if(now - lastTimeSeenChased[controllerID] > timeFleeProcessProlongs)
        {
            return 0.0f;
        }
        else
        {
            return 1.0f - (now - lastTimeSeenChased[controllerID]) * 0.5f / timeFleeProcessProlongs;
        }
    }
}


public class DeployAmbushPrototype : TacticalTaskPrototype
{
    public DeployAmbushPrototype()
    {
        type = TacticalTaskType.DeployAmbush;
        applicableMask = CharacterRole.Support;
        // firstState = TacticalTask.StateRefs[type];
    }


    public override float Evaluate(TacticalTaskConfig config)
    {
        var target = config.target; // here target is an chokepoint area volume
        var controller = config.controller;
        var teammates = config.teammates;
        var enemies = config.enemies;

        float distanceScore = 1.0f / (Vector3.Distance(target.transform.position, controller.transform.position) + 1.0f);

        // score wrt duplicate work
        bool duplicate = false;

        for (int i = 0; i < teammates.Count; i++)
        {
            // check if this teammate is doing the same
            var task = TacticalPlanner.CurrentTask(teammates[i]);
            if (task.prototype.type == this.type && task.target == target)
            {
                duplicate = true;
                break;
            }
        }
        
        float duplicateScore = duplicate ? 0.0f : 1.0f;


        float dangerScoreBase = 0.25f;
        float dangerScore = dangerScoreBase;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (Vector3.Distance(enemies[i].transform.position, target.transform.position) < 20.0f)
            {
                dangerScore *= 2.0f;
            }
        }
        dangerScore -= dangerScoreBase;

        float initiative = 0.0f;    // = target.areaImportance * (1 - duplicateScore);
        float distanceWeight = 0.5f;
        float dangerWeight = -0.2f;

        float finalScore = initiative + distanceWeight * distanceScore + dangerWeight * dangerScore;

        return Mathf.Clamp01(finalScore);
    }
}


public class FakeDefusePrototype : TacticalTaskPrototype
{
    public FakeDefusePrototype()
    {
        type = TacticalTaskType.FakeDefuse;
        applicableMask = CharacterRole.Support;
        // firstState = TacticalTask.StateRefs[type];
    }


    public override float Evaluate(TacticalTaskConfig config)
    {
        // unpack config
        var target = config.target;
        var controller = config.controller;
        var teammates = config.teammates;
        var enemies = config.enemies;


        // score wrt distance
        float distanceScore = 1.0f / Mathf.Min(Vector3.Distance(target.transform.position, controller.transform.position), 1.0f);

        // score wrt duplicate work
        bool duplicate = false;

        for (int i = 0; i < teammates.Count; i++)
        {
            // check if this teammate is doing the same
            var task = TacticalPlanner.CurrentTask(teammates[i]);
            if (task.prototype.type == this.type && task.target == target)
            {
                duplicate = true;
                break;
            }
        }
        float duplicateScore = duplicate ? 0.0f : 1.0f;


        // score wrt danger
        float dangerScoreBase = 0.25f;
        float dangerScore = dangerScoreBase;
        for (int i = 0; i < enemies.Count; i++)
        {
            if (Vector3.Distance(enemies[i].transform.position, target.transform.position) < 20.0f)
            {
                dangerScore *= 2.0f;
            }
        }
        dangerScore -= dangerScoreBase;


        float initiative = (0.5f - duplicateScore);
        float distanceWeight = 0.1f;
        float dangerWeight = -1.0f;
        float finalScore = initiative + distanceWeight * distanceScore + dangerWeight * dangerScore;

        return Mathf.Clamp01(finalScore);
    }
}