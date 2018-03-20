using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decision/PowerSourceActive")]
public class PowerSourceActiveDecision : AIDecision {

	public override bool Decide(AIStateController controller) {
		//Debug.Log (controller.targetIndx);
		bool ispowerSourceActive = controller.survivorsk.m_InteractingPowerSource != null && controller.survivorsk.m_InteractingPowerSource.gameObject.activeSelf;
		//Debug.Log (ispowerSourceActive);
		return ispowerSourceActive;
	}
}
