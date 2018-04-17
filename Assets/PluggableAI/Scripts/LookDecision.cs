using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decision/Look")]
public class LookDecision : AIDecision {

	public override bool Decide(AIStateController controller) {
		bool targetVisible = Look (controller);
		if (targetVisible && controller.IsValidState()) {
			controller.survivorBD.SetPSTimer (controller.targetIndx, 100.0f / (Vector3.Distance(controller.wayPointList[controller.targetIndx].transform.position, controller.transform.position) + 10.0f));
		}
		return targetVisible;
	}

	private bool Look(AIStateController controller) {
		controller.visibleTargets.Clear ();

		Collider[] targetsInViewRadius = Physics.OverlapSphere (controller.transform.position, controller.enemyStats.lookRange, controller.targetMask);

		for (int i = 0; i < targetsInViewRadius.Length; i++) {
			if (targetsInViewRadius [i].GetComponent<NetworkCharacter>().Team == GameEnum.TeamType.Hunter) {
				Transform target = targetsInViewRadius [i].transform;
				Vector3 dirToTarget = (target.position - controller.transform.position).normalized;
				//if (Vector3.Angle (controller.transform.forward, dirToTarget) < controller.enemyStats.fov.viewAngle / 2) {
					float dstToTarget = Vector3.Distance (controller.transform.position, target.position);
					if (!Physics.Raycast (controller.transform.position, dirToTarget, dstToTarget, controller.obstacleMask)) {

						controller.visibleTargets.Add (target);
					}
				//}
			} 
		}
			
		/*
		List<Transform> tempNearTargets = controller.GetComponent<FieldOfViewNearby> ().visibleTargets;
		foreach (Transform target in tempNearTargets) {
			controller.visibleTargets.Add (target);
		}

		FieldOfView tempFOV = controller.GetComponent<FieldOfView> ();
		if (tempFOV != null) {
			List<Transform> tempTargets = tempFOV.visibleTargets;
			foreach (Transform target in tempTargets) {
				controller.visibleTargets.Add (target);
			}
		}
		*/
		if (controller.visibleTargets.Count > 0) {
			return true;
		} else {
			return false;
		}


	}
}
