using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Actions/GoToTarget")]
public class GoToTargetAction : AIAction{
	
	public override void Act (AIStateController controller)
	{
		if (controller.character.CurrentState != CharacterState.Normal) {
			return;
		}
		GoToTarget (controller);
	}

	private void GoToTarget(AIStateController controller) {	
		
		if (controller.targetIndx == -1 || !controller.wayPointList [controller.targetIndx].gameObject.activeSelf || controller.survivorBD.IsPSBlock(controller.targetIndx)) {
			/*
			float minDis = float.MaxValue;
			for (int i = 0; i < controller.wayPointList.Count; i++) {
				
				if (IsSuitableWayPoint(controller, i)) {
					float tempDis = Vector3.Distance(controller.wayPointList [i].position, controller.transform.position);
					if (tempDis < minDis) {
						minDis = tempDis;
						controller.targetIndx = i;
					}
				}
			}
			*/
			AssignTargets (controller);

		}

		controller.navMeshAgent.destination = controller.wayPointList [controller.targetIndx].position;
		controller.navMeshAgent.isStopped = false;
		if (controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.stoppingDistance && !controller.navMeshAgent.pathPending) {
			
			controller.navMeshAgent.isStopped = true;
			
		}

	}
	private void AssignTargets(AIStateController controller) {
		
		for (int i = 0; i < controller.predictTargets.Length; i++) {
			controller.predictTargets [i] = -1;
		}
		int sum = 0;
		int threshold = 0, ways = 0, plys = 0;
		bool[] assigned = new bool[controller.wayPointList.Count];
		for (int i = 0; i < controller.wayPointList.Count; i++) {
			if (controller.wayPointList [i].gameObject.activeSelf && !controller.survivorBD.IsPSBlock(i)) {
				ways++;
				assigned [i] = false;
			} else {
				assigned [i] = true;
			}
		}
		for (int i = 0; i < controller.players.Count; i++) {
			if (controller.players [i] != null) {
				plys++;
			}
		}
		threshold = Mathf.Min (ways, plys);
		while (sum < threshold) {
			for (int i = 0; i < controller.wayPointList.Count; i++) {
				if (!assigned[i]) {
					float min = float.MaxValue;
					int tempindx = -1;
					for (int k = 0; k < controller.players.Count; k++) {
						if (controller.players [k] != null && controller.predictTargets [k] == -1) {
							float dis = Vector3.Distance (controller.players [k].transform.position, controller.wayPointList [i].position);
							if (dis < min) {
								min = dis;
								tempindx = k;
							}
						}
					}
					min = float.MaxValue;
					int tempi = -1;
					if (tempindx != -1) {
						for (int k = 0; k < controller.wayPointList.Count; k++) {
							if (!assigned [k]) {
								float dis = Vector3.Distance (controller.players [tempindx].transform.position, controller.wayPointList [k].position);
								if (dis < min) {
									min = dis;
									tempi = k;
								}
							}
						}
						if (tempi == i) {
							controller.predictTargets [tempindx] = i;
							assigned [i] = true;
							sum++;
						}
					}
				}
			}
		}
		if (controller.predictTargets [controller.playerIndex] == -1) {
			for (int i = 0; i < controller.players.Count; i++) {
				if (controller.players [i] != null && controller.predictTargets [i] != -1) {
					controller.predictTargets [controller.playerIndex] = controller.predictTargets [i];
					break;
				}
			}
		}
		controller.targetIndx = controller.predictTargets [controller.playerIndex];
	}
	/*
	private bool IsSuitableWayPoint(AIStateController controller, int i) {
		float min = (controller.transform.position - controller.wayPointList [i].position).magnitude;
		if (controller.wayPointList [i].gameObject.activeSelf) {
			for (int k = 0; k < controller.players.Length; k++) {
				if (controller.players[k].activeSelf && controller.players[k] != controller.gameObject) {
					float dis = (controller.players [k].transform.position - controller.wayPointList [i].position).magnitude;
					if (dis < min) {
						return false;
					}
				}
			}
			return true;
		}
		return false;
	}
	*/

}
