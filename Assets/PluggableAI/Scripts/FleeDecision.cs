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
            controller.Look();
            if (controller.visibleTargets.Count == 0 && Time.time - controller.lastTimeInDanger > 10.0f)
            {
                controller.fleeOffsetMultiplyBy = 0;
                return true;
            }
            else
            {
                return false;
            }
		} else {
			//controller.fleeOffsetMultiplyBy += 5;
			//Debug.Log (controller.fleeOffsetMultiplyBy);
			return false;
		}
	}





    private bool IfFleeTimeElapsed(AIStateController controller) {
		return controller.CheckIfCountDownElapsed (controller.enemyStats.fleeDuration);
	}
}
