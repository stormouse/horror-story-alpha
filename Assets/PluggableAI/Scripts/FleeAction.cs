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
	public override void Act(AIStateController controller) {
		if (controller.character.CurrentState != CharacterState.Normal) {
			return;
		}
		Flee (controller);
	}


    private void Flee(AIStateController controller)
    {
        bool fallback = false;
        var planner = controller.GetComponent<EscapePlanner>();

        if (planner && EscapeGraph.Initialized)
        {
            Vector3 dest = EscapeGraph.GetDestination(controller.transform.position, controller.visibleTargets, planner.areaName);
            if (Vector3.SqrMagnitude(dest - controller.transform.position) > 16.0f)
            {
                // Debug.Log("Recommended Exit Location: " + dest);
                controller.navMeshAgent.destination = dest;
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
		Vector3 runDir = new Vector3 (0, 0, 0);
		foreach (Transform hunters in controller.visibleTargets) {
			//Debug.Log (controller.transform.position.x - hunters.position.x);
			//Debug.Log (controller.transform.position.z - hunters.position.z);
			runDir += (10 / Vector3.Distance (hunters.position, controller.transform.position)) * (new Vector3(controller.transform.position.x - hunters.position.x, 0, controller.transform.position.z - hunters.position.z)).normalized;
		}
	
		runDir += BoarderRunOffset(controller, runDir);
		Vector3 runTo = controller.transform.position + runDir.normalized * fleeDisMultiplyBy;

		controller.navMeshAgent.destination = runTo;
		controller.navMeshAgent.isStopped = false;
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
