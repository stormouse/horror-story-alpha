using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIHunter/Actions/_ChaseChaserAction")]
public class _ChaseChaserAction : AIAction {

	public override void Act (AIStateController controller)
	{
		chase (controller);
	}

	public void chase(AIStateController controller){
		if (controller.character.CurrentState == CharacterState.Normal) {
			if (!controller.navMeshAgent.enabled)
				controller.navMeshAgent.enabled = true;
			controller.navMeshAgent.destination = controller.chaseTarget [0].transform.position;
			controller.navMeshAgent.isStopped = false;
		} else {
			controller.navMeshAgent.enabled = false;
		}
	}
}
