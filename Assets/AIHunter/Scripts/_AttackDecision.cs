using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AIHunter/Decisions/_AttackDecision")]
public class _AttackDecision : AIDecision {

	public override bool Decide(AIStateController controller){
		bool attackable = AttackAble(controller);
		return attackable;
	}

	public bool AttackAble(AIStateController controller){
		if (controller.chaseTarget.Count == 0)
			return false;
		//is died
		if (controller.chaseTarget[0].CurrentState == CharacterState.Dead) {
			controller.chaseTarget.Remove (controller.chaseTarget [0]);
			return false;
		}
		float distance = (controller.chaseTarget [0].transform.position - controller.transform.position).magnitude;
		if (distance <= controller.attackRange)
			return true;
		else
			return false;
	}
}
