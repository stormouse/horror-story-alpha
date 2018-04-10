using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AIHunter/Actions/_ChasePSAction")]
public class _ChasePSAction : _Action {
	public override void Act (_HunterStateController controller)
	{
		if (controller.character.CurrentState == CharacterState.Normal) {
			if (!controller.navMeshAgent.enabled)
				controller.navMeshAgent.enabled = true;
			CatchTo (controller);
		} else {
			controller.navMeshAgent.enabled = false;
		}
	}

	public void CatchTo(_HunterStateController controller){
		if (controller.character.CurrentState == CharacterState.Normal) {
			controller.navMeshAgent.destination = controller.underInvokeList [0].position;
			controller.navMeshAgent.isStopped = false;
		}
	}
}
