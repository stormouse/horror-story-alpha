using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Actions/Patrol")]
public class PatrolAction : AIAction {

	public override void Act(AIStateController controller) {
		Patrol (controller);
	}

	private void Patrol(AIStateController controller) {
		controller.navMeshAgent.destination = controller.wayPointList [controller.nextWayPoint].position;
		controller.navMeshAgent.isStopped = false; // Resume?

		if (controller.navMeshAgent.remainingDistance <= controller.navMeshAgent.stoppingDistance && !controller.navMeshAgent.pathPending) {
			controller.nextWayPoint = (controller.nextWayPoint + 1) % controller.wayPointList.Count;
		}
	}
}
