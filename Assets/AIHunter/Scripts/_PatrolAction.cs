using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AIHunter/Actions/_PatrolAction")]
public class _PatrolAction : AIAction {
	public override void Act (AIStateController controller)
	{
		Patrol (controller);
	}

	public void Patrol(AIStateController controller){

		//check valid
		foreach (Transform ps in controller.wayPointList) {
			/*
			PowerSourceController psc = ps.GetComponent<PowerSourceController> ();
			if (null == psc || psc.Charged) {
				controller.wayPointList.Remove (ps);
			}
			*/
			if (!ps.gameObject.activeSelf) {
				controller.wayPointList.Remove (ps);
			}
		}

		if (controller.nextWayPoint >= controller.wayPointList.Count) {
            // Get next waypoint
            if (controller.wayPointList.Count > 0)
            {
                controller.nextWayPoint = Random.Range(0, controller.wayPointList.Count);
            }
            else
            {
                Debug.Log("Should be wandering here.");
                return;
            }
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
