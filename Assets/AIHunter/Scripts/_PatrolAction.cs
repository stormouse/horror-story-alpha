using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AIHunter/Actions/_PatrolAction")]
public class _PatrolAction : _Action {
	public override void Act (_HunterStateController controller)
	{
		Patrol (controller);
	}

	public void Patrol(_HunterStateController controller){
		controller.navMeshAgent.destination = controller.wayPointList [controller.nextWayPoint].position;
		controller.navMeshAgent.isStopped = false;

		float distance = System.Math.Abs((controller.wayPointList [controller.nextWayPoint].position.x - controller.transform.position.x))
			+ System.Math.Abs ((controller.wayPointList [controller.nextWayPoint].position.z - controller.transform.position.z));

		if (distance <= controller.navMeshAgent.stoppingDistance) {
			int ran = Random.Range (0, controller.wayPointList.Count);
			controller.nextWayPoint = ran;
		}
	}
}
