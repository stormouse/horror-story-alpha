using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIHunter/Actions/_WonderAction")]
public class _WonderAction : _Action {

	public override void Act (_HunterStateController controller)
	{
		Wonder (controller);
	}

	public void Wonder(_HunterStateController controller){
		controller.navMeshAgent.destination = controller.wonderPoint;
	}
}
