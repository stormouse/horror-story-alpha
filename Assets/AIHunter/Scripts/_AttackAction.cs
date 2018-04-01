using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIHunter/Actions/_AttackAction")]
public class _AttackAction : _Action {

	public override void Act (_HunterStateController controller)
	{
		Attack (controller);
	}

	public void Attack(_HunterStateController controller){
		//attack kill here
		controller.character.Perform ("Attack", controller.gameObject, null);
	}
}
