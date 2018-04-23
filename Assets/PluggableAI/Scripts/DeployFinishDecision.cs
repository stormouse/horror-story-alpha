using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decision/DeployFinish")]
public class DeployFinishDecision : AIDecision {
	private float finishTime = 1.5f;
	public override bool Decide (AIStateController controller)
	{
		return controller.CheckIfCountDownElapsed (finishTime);
	}

}
