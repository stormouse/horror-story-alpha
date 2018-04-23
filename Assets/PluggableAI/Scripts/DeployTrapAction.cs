using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Actions/ChokePoint")]
public class DeployTrapAction : AIAction{

	public override void Act(AIStateController controller) {
		if (controller.character.CurrentState == CharacterState.Normal) {
			DeployTrap (controller); 
		}
	}
	private void DeployTrap(AIStateController controller) {
		controller.character.Perform ("Deploy", controller.gameObject, null);
	}

}
