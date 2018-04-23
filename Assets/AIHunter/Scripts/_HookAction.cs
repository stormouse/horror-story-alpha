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
		Vector3 selfpos = controller.transform.position;
		dir.direction = target - selfpos;
		dir.direction.y = 0.0f;
		controller.character.Perform ("Hook", controller.gameObject, dir);
	}
}
