using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIHunter/Actions/_AttackAction")]
public class _AttackAction : AIAction {

	public override void Act (AIStateController controller)
	{
		Attack (controller);
	}

	public void Attack(AIStateController controller){
		//attack kill here
		controller.character.Perform ("Attack", controller.gameObject, null);
		controller.chaseTarget [0].Perform ("Die", controller.chaseTarget[0].gameObject, null);
		controller.chaseTarget.Remove (controller.chaseTarget [0]);
	}
}
