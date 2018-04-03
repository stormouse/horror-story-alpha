using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIHunter/Actions/_ChaseChaserAction")]
public class _ChaseChaserAction : _Action {

	public override void Act (_HunterStateController controller)
	{
		chase (controller);
	}

	public void chase(_HunterStateController controller){
		controller.navMeshAgent.destination = controller.chaseTarget[0].transform.position;
		controller.navMeshAgent.isStopped = false;
	}
}
