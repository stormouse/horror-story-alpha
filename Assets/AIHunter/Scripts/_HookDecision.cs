using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AIHunter/Decisions/_HookDecision")]
public class _HookDecision : _Decision {

	public override bool Decide(_HunterStateController controller){
		bool hookAble = HookAble(controller);
		return hookAble;
	}

	public bool HookAble(_HunterStateController controller){
		if (controller.chaseTarget.Count == 0)
			return false;
		if (controller.chaseTarget [0] == null) {
			controller.chaseTarget.Remove (controller.chaseTarget [0]);
			return false;
		}
		float distance = (controller.chaseTarget [0].transform.position - controller.transform.position).magnitude;
		if (distance <= controller.hookRange && controller.character.CurrentState == CharacterState.Normal) {
			return true;
		}
		else
			return false;
	}
}
