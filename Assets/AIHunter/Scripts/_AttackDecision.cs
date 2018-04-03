using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AIHunter/Decisions/_AttackDecision")]
public class _AttackDecision : _Decision {

	public override bool Decide(_HunterStateController controller){
		bool attackable = AttackAble(controller);
		return attackable;
	}

	public bool AttackAble(_HunterStateController controller){
		if (controller.chaseTarget.Count == 0)
			return false;
		float distance = (controller.chaseTarget [0].position - controller.transform.position).magnitude;
		if (distance <= controller.attackRange)
			return true;
		else
			return false;
	}
}
