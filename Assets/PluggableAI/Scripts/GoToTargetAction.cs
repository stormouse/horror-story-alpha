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
		if (controller.targetIndx == -1 || !controller.wayPointList [controller.targetIndx].gameObject.activeSelf) {
			float minDis = float.MaxValue;
			for (int i = 0; i < controller.wayPointList.Count; i++) {
				//Debug.Log (i);
				//Debug.Log (controller.wayPointList [i].gameObject.activeSelf);
				if (controller.wayPointList [i].gameObject.activeSelf) {
					float tempDis = Vector3.Distance(controller.wayPointList [i].position, controller.transform.position);
					if (tempDis < minDis) {
						minDis = tempDis;
						controller.targetIndx = i;
					}
				}
			}
			//Debug.Log (controller.targetIndx);
		}

		controller.navMeshAgent.destination = controller.wayPointList [controller.targetIndx].position;
		controller.navMeshAgent.isStopped = false;
		if (controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.stoppingDistance && !controller.navMeshAgent.pathPending) {
			//Debug.Log (controller.navMeshAgent.remainingDistance);
			controller.navMeshAgent.isStopped = true;
			
		}

	}
}
