using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AIHunter/Actions/_ChasePSAction")]
public class _ChasePSAction : _Action {
	public override void Act (_HunterStateController controller)
	{
		CatchTo (controller);
	}

	public void CatchTo(_HunterStateController controller){
		if (controller.underInvokeList.Count > 0)
			controller.navMeshAgent.destination = controller.underInvokeList [0].position;
	}
}
