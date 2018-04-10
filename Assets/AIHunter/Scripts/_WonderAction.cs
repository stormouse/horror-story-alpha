using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIHunter/Actions/_WonderAction")]
public class _WonderAction : _Action {

	public override void Act (_HunterStateController controller)
	{
		if (controller.character.CurrentState == CharacterState.Normal) {
			if (!controller.navMeshAgent.enabled)
				controller.navMeshAgent.enabled = true;
			Wonder (controller);
		} else {
			controller.navMeshAgent.enabled = false;
		}
	}

	public void Wonder(_HunterStateController controller){
		controller.navMeshAgent.destination = controller.wonderPoint;
	}
}
