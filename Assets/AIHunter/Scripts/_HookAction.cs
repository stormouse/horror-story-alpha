using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIHunter/Actions/_HookAction")]
public class _HookAction : AIAction {

	public override void Act (AIStateController controller)
	{
			lunchHook (controller);
	}

	public void lunchHook(AIStateController controller){
		//lunch hook here
		DirectionArgument dir = new DirectionArgument();
		Vector3 target = controller.chaseTarget [0].transform.position;
		target.y = 1.0f;
		Vector3 selfpos = controller.transform.position;
		selfpos.y = 1.0f;
		Vector3 dirToTarget = (target - selfpos).normalized;
		float dstToTarget = Vector3.Distance(selfpos, target);
		if (!Physics.Raycast (controller.transform.position, dirToTarget, dstToTarget)) {
			dir.direction = target - selfpos;
			controller.character.Perform ("Hook", controller.gameObject, dir);
		}
	}
}
