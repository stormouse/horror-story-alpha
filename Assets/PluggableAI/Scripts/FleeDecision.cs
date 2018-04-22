using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decision/Flee")]
public class FleeDecision : AIDecision {

    public override bool Decide(AIStateController controller)
    {
        controller.Look();
        if (controller.visibleTargets.Count == 0 && Time.time - controller.lastTimeInDanger > 10.0f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }


    private bool IfFleeTimeElapsed(AIStateController controller) {
		return controller.CheckIfCountDownElapsed (controller.enemyStats.fleeDuration);
	}
}
