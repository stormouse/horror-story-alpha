using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIHunter/Actions/_ChaseChaserAction")]
public class _ChaseChaserAction : _Action {

	public _State PatrolState;

	public override void Act (_HunterStateController controller)
	{
		chase (controller);
	}

	public void chase(_HunterStateController controller){
		//check target legal
		if (null == controller.chaseTarget [0] || controller.chaseTarget [0].CurrentState == CharacterState.Dead) {
			controller.TransitionToState (PatrolState);
			return;
		}
		if (controller.character.CurrentState == CharacterState.Normal) {
			controller.navMeshAgent.destination = controller.chaseTarget [0].transform.position;
			controller.navMeshAgent.isStopped = false;
		}
	}
}
