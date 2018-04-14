using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AIHunter/Decisions/_HookDecision")]
public class _HookDecision : AIDecision {

	public override bool Decide(AIStateController controller){
		bool hookAble = HookAble(controller);
		return hookAble;
	}

	public bool HookAble(AIStateController controller){
		if (controller.chaseTarget.Count == 0)
			return false;
		if (controller.chaseTarget [0] == null) {
			controller.chaseTarget.Remove (controller.chaseTarget [0]);
			return false;
		}
		float distance = (controller.chaseTarget [0].transform.position - controller.transform.position).magnitude;
		if (distance <= controller.hookRange && controller.character.CurrentState == CharacterState.Normal
			&& controller.hskills.HookReady) {
			return true;
		}
		else
			return false;
	}
}
