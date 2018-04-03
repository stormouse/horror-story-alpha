using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AIHunter/Decisions/_Look")]
public class _LookDecision : _Decision {

	public override bool Decide(_HunterStateController controller){
		bool targetVisiable = Look(controller);
		return targetVisiable;
	}

	private bool Look(_HunterStateController controller){
//		RaycastHit hit;
//
//		Debug.DrawRay (controller.eye.position, controller.eye.forward.normalized * controller.enemyStats.lookRange, Color.green);
//
//		if (Physics.SphereCast (controller.eye.position, controller.enemyStats.lookSphereCastRadius, controller.eye.forward, out hit, controller.enemyStats.lookRange)
//		    && hit.collider.CompareTag ("Player")) {
//			controller.chaseTarget = hit.transform;
//			return true;
//		} else {
//			return false;
//		}
		controller.chaseTarget.Clear ();

		Collider[] targetsInViewRadius = Physics.OverlapSphere (controller.transform.position, controller.enemyStats.lookRange, controller.targetMask);

		for (int i = 0; i < targetsInViewRadius.Length; i++) {
			if (targetsInViewRadius [i].tag.Equals ("Player")) {
				Transform target = targetsInViewRadius [i].transform;
				Vector3 dirToTarget = (target.position - controller.transform.position).normalized;
				//if (Vector3.Angle (controller.transform.forward, dirToTarget) < controller.enemyStats.fov.viewAngle / 2) {
				float dstToTarget = Vector3.Distance (controller.transform.position, target.position);
			//	if (!Physics.Raycast (controller.transform.position, dirToTarget, dstToTarget, controller.obstacleMask)) 
					controller.chaseTarget.Add (target);
				}
				//}
			} 
			
		if (controller.chaseTarget.Count > 0)
			return true;
		else
			return false;
	}
}
