using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decision/Flee")]
public class FleeDecision : AIDecision {

	public override bool Decide (AIStateController controller) {
		bool isFleeTimeElapsed = IfFleeTimeElapsed (controller);
		return isFleeTimeElapsed;
	}

	private bool IfFleeTimeElapsed(AIStateController controller) {
		return controller.CheckIfCountDownElapsed (controller.enemyStats.fleeDuration);
	}
}
