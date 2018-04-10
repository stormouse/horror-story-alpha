using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AIHunter/Actions/_PatrolAction")]
public class _PatrolAction : _Action {
	public override void Act (_HunterStateController controller)
	{
		if (controller.character.CurrentState == CharacterState.Normal) {
			if (!controller.navMeshAgent.enabled)
				controller.navMeshAgent.enabled = true;
			Patrol (controller);
		} else {
			controller.navMeshAgent.enabled = false;
		}
	}

	public void Patrol(_HunterStateController controller){

		//check valid
		foreach (PowerSourceController psc in controller.wayPointList) {
			if (null == psc || psc.Charged) {
				controller.wayPointList.Remove (psc);
			}
		}

		if (controller.nextWayPoint >= controller.wayPointList.Count) {
			return;
		}

		if (controller.character.CurrentState == CharacterState.Normal) {
			if (!controller.navMeshAgent.enabled)
				controller.navMeshAgent.enabled = true;
			controller.navMeshAgent.destination = controller.wayPointList [controller.nextWayPoint].transform.position;
			controller.navMeshAgent.isStopped = false;
		} else {
			controller.navMeshAgent.enabled = false;
		}

		float distance = System.Math.Abs((controller.wayPointList [controller.nextWayPoint].transform.position.x - controller.transform.position.x))
			+ System.Math.Abs ((controller.wayPointList [controller.nextWayPoint].transform.position.z - controller.transform.position.z));

		if (distance <= controller.pSStopDistance) {
			int ran = Random.Range (0, controller.wayPointList.Count);
			controller.nextWayPoint = ran;
		}
	}
}
