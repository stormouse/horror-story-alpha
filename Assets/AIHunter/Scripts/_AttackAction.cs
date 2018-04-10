using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIHunter/Actions/_AttackAction")]
public class _AttackAction : _Action {

	public override void Act (_HunterStateController controller)
	{
		if (controller.character.CurrentState == CharacterState.Normal) {
			if (!controller.navMeshAgent.enabled)
				controller.navMeshAgent.enabled = true;
			Attack (controller);
		} else {
			controller.navMeshAgent.enabled = false;
		}
	}

	public void Attack(_HunterStateController controller){
		//attack kill here
		controller.character.Perform ("Attack", controller.gameObject, null);
		controller.chaseTarget [0].Perform ("Die", controller.chaseTarget[0].gameObject, null);
		controller.chaseTarget.Remove (controller.chaseTarget [0]);
	}
}
