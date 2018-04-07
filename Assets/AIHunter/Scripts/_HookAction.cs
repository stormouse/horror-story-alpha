using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIHunter/Actions/_HookAction")]
public class _HookAction : _Action {

	public _State PatrolState;

	public override void Act (_HunterStateController controller)
	{
			lunchHook (controller);
	}

	public void lunchHook(_HunterStateController controller){
		//check target legal
		if (null == controller.chaseTarget [0] || controller.chaseTarget [0].CurrentState == CharacterState.Dead) {
			controller.TransitionToState (PatrolState);
			return;
		}
		//lunch hook here
		DirectionArgument dir = new DirectionArgument();
		dir.direction = controller.chaseTarget [0].transform.position - controller.transform.position;
		dir.direction.y = 0;
		controller.character.Perform ("Hook", controller.gameObject, dir);
	}
}
