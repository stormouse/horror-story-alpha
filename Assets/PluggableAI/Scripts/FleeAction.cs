using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Actions/Flee")]
public class FleeAction : AIAction {
	private float westBoarder = -52;
	private float eastBoarder = 54;
	private float northBoarder = 63;
	private float southBoarder = 65;
	public float fleeDisMultiplyBy = 10;
	private float smokeUsageInterval = 10.0f;
	public override void Act(AIStateController controller) {
		if (controller.character.CurrentState != CharacterState.Normal) {
			return;
		}

		if(controller.visibleTargets.Count > 0 && controller.CheckIfCountDownElapsed(controller.SmokeUsageCooldownMultiply * smokeUsageInterval + 3.5f) && !controller.CheckIfCountDownElapsed(controller.SmokeUsageCooldownMultiply * smokeUsageInterval + 4.0f))
        {
            controller.character.Perform("Smoke", controller.gameObject, null);
			controller.SmokeUsageCooldownMultiply++;
        }

		Flee (controller);
	}


    private void Flee(AIStateController controller)
    {
        controller.Look();

        bool fallback = false;
        var planner = controller.GetComponent<EscapePlanner>();

        if (planner && EscapeGraph.Initialized)
        {
            //Vector3 prevDest = controller.navMeshAgent.destination;
            Vector3 dest = EscapeGraph.GetDestination(controller.transform.position, controller.transform.forward, controller.visibleTargets, planner.areaName);
            if (Vector3.SqrMagnitude(dest - controller.transform.position) > 1.0f)
            {
                controller.navMeshAgent.destination = dest;
                controller.navMeshAgent.stoppingDistance = 1e-3f;
                controller.navMeshAgent.isStopped = false; // is this safe?
            }
            else fallback = true;
        }
        else fallback = true;

        
        if (fallback)
        {
            Flee_DirectOpposite(controller);
        }
    }


    // Deprecated by Shawnc 4/12. Replaced with EscapePlanner system.
	private void Flee_DirectOpposite(AIStateController controller) {
		/*
		Vector3 runDir = new Vector3 (0, 0, 0);
		foreach (Transform hunters in controller.visibleTargets) {
			runDir += (10 / Vector3.Distance (hunters.position, controller.transform.position)) * (new Vector3(controller.transform.position.x - hunters.position.x, 0, controller.transform.position.z - hunters.position.z)).normalized;
		}
	
		runDir += BoarderRunOffset(controller, runDir);
		Vector3 runTo = controller.transform.position + runDir.normalized * fleeDisMultiplyBy;

		controller.navMeshAgent.destination = runTo;
		controller.navMeshAgent.isStopped = false;
		*/
		Vector3 runDir = new Vector3 (0, 0, 0);
		foreach (Transform hunters in controller.visibleTargets) {
			runDir += (10 / Vector3.Distance (hunters.position, controller.transform.position)) * (new Vector3(controller.transform.position.x - hunters.position.x, 0, controller.transform.position.z - hunters.position.z)).normalized;
		}
		float max = float.MinValue;
		int pwrsrcIndex = -1;
		for (int i = 0; i < controller.wayPointList.Count; i++) {
			if (controller.wayPointList [i].gameObject.activeSelf) {
				float tempDis = Vector3.Distance(controller.wayPointList [i].position, controller.transform.position);
				if (max < tempDis) {
					max = tempDis;
					pwrsrcIndex = i;
				}
			}
		}
		if (pwrsrcIndex != -1) {
			runDir += (max / 20) * (new Vector3 (controller.wayPointList [pwrsrcIndex].position.x - controller.transform.position.x, 0, controller.wayPointList [pwrsrcIndex].position.z - controller.transform.position.z)).normalized * controller.fleeOffsetMultiplyBy;
		}

		Vector3 runTo = controller.transform.position + runDir.normalized * fleeDisMultiplyBy + CalRunOffset (controller, runDir) * controller.fleeOffsetMultiplyBy;

		controller.fleeDest = runTo;
		controller.navMeshAgent.destination = runTo;
		controller.navMeshAgent.isStopped = false;
	}
	//calculate rundir offset
	private Vector3 CalRunOffset(AIStateController controller, Vector3 runDir) {
		Vector3 offsetDir = new Vector3 (Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));
		return offsetDir;
	}
		//Boarder Check
	private Vector3 BoarderRunOffset(AIStateController controller, Vector3 runDir) {
		Vector3 offsetDir = new Vector3 (0, 0, 0);
		if (runDir.x >= 0 && runDir.z >= 0) {
			offsetDir += (10 / Mathf.Abs (northBoarder - controller.transform.position.z)) * (new Vector3 (runDir.x, 0, 0)).normalized;
			offsetDir += (10 / Mathf.Abs (eastBoarder - controller.transform.position.x)) * (new Vector3 (0, 0, runDir.z)).normalized;
		}
		if (runDir.x >= 0 && runDir.z < 0) {
			offsetDir += (10 / Mathf.Abs (southBoarder - controller.transform.position.z)) * (new Vector3 (runDir.x, 0, 0)).normalized;
			offsetDir += (10 / Mathf.Abs (eastBoarder - controller.transform.position.x)) * (new Vector3 (0, 0, runDir.z)).normalized;
		}
		if (runDir.x < 0 && runDir.z >= 0) {
			offsetDir += (10 / Mathf.Abs (northBoarder - controller.transform.position.z)) * (new Vector3 (runDir.x, 0, 0)).normalized;
			offsetDir += (10 / Mathf.Abs (westBoarder - controller.transform.position.x)) * (new Vector3 (0, 0, runDir.z)).normalized;
		}
		if (runDir.x < 0 && runDir.z < 0) {
			offsetDir += (10 / Mathf.Abs (southBoarder - controller.transform.position.z)) * (new Vector3 (runDir.x, 0, 0)).normalized;
			offsetDir += (10 / Mathf.Abs (westBoarder - controller.transform.position.x)) * (new Vector3 (0, 0, runDir.z)).normalized;
		}
		return offsetDir;
	}
}
