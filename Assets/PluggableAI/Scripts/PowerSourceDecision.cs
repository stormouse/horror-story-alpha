using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "PluggableAI/Decision/PowerSource")]
public class PowerSourceDecision : AIDecision {
	public override bool Decide(AIStateController controller) {
		bool nearPowerSource = CheckPowerSourceNearBy(controller); 
		return nearPowerSource;
	}

	private bool CheckPowerSourceNearBy(AIStateController controller) {
		
		SurvivorSkills skills = controller.survivorsk;
		//Debug.Log (skills.m_InteractingPowerSource);
		if (skills.m_InteractingPowerSource != null && skills.m_InteractingPowerSource.gameObject.activeSelf && skills.m_InteractingPowerSource.gameObject == controller.wayPointList[controller.targetIndx].gameObject) {
			controller.navMeshAgent.isStopped = true;
			return true;
		} else {
			return false;
		} 
	}
}
