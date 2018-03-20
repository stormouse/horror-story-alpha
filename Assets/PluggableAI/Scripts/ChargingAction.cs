using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Actions/Charging")]
public class ChargingAction : AIAction {
	public override void Act(AIStateController controller) {
		Charging (controller);	
	}

	private void Charging(AIStateController controller){
		controller.survivorsk.ChargePerform ();
	}
}
