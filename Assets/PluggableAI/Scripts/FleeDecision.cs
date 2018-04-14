using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decision/Flee")]
public class FleeDecision : AIDecision {

	public override bool Decide (AIStateController controller) {
		/*
		bool isFleeTimeElapsed = IfFleeTimeElapsed (controller);
		return isFleeTimeElapsed;
		*/
		if (controller.navMeshAgent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathComplete) {
			controller.fleeOffsetMultiplyBy = 0;
			return true;
		} else {
			
			controller.fleeOffsetMultiplyBy += 5;
			Debug.Log (controller.fleeOffsetMultiplyBy);
			return false;
		}
	}

	private bool IfFleeTimeElapsed(AIStateController controller) {
		return controller.CheckIfCountDownElapsed (controller.enemyStats.fleeDuration);
	}
}
