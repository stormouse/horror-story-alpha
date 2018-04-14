using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Actions/FocusFlee")]
public class FocusFleeAction : AIAction {

	public override void Act (AIStateController controller)
	{
		//Debug.Log (controller.fleeDest);
		controller.navMeshAgent.destination = controller.fleeDest;
		controller.navMeshAgent.isStopped = false;
	}
}
