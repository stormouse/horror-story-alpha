using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decision/ChokePoint")]

public class ChokePointDecision : AIDecision{
	float maxDetectDistance = 10.0f;
	float chokeDistance = 8.0f;

	public override bool Decide(AIStateController controller) {
		//Debug.Log (controller.deployedTimer);
		if (controller.CheckDeployUsageCooldown(controller.defaultDeployedUsageCooldown)) {
			bool isChokePoint = DetectChokePoint (controller);
		//	Debug.Log (isChokePoint);
			if (isChokePoint) {
				controller.deployedTimer = 0;
			}
			return isChokePoint;
		} else {
			return false;
		}
	}
	private bool DetectChokePoint(AIStateController controller) {
		Transform self = controller.transform;
		if (controller.targetIndx >=0 && controller.targetIndx < controller.wayPointList.Count && Vector3.Distance (self.position, controller.wayPointList [controller.targetIndx].transform.position) < 20.0f) {
			Vector3 forward = self.forward;
			Vector3 up = self.up;
			Vector3 right = Vector3.Cross (forward.normalized, up.normalized);
			Vector3 left = -right;
			right.y = 1.0f;
			left.y = 1.0f;
			RaycastHit hitleft;
			RaycastHit hitright;
			if (Physics.Raycast (self.position, left, out hitleft, maxDetectDistance) && Physics.Raycast(self.position, right, out hitright, maxDetectDistance)) {
				float tempdis = Vector3.Distance (hitleft.point, hitright.point);
				if (tempdis < chokeDistance) {
					return true;
				} 
			}
		}
		return false;
	}
}
