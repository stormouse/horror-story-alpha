using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AIHunter/Decisions/_AlarmDecision")]
public class _AlarmDecision : _Decision {

	public override bool Decide(_HunterStateController controller){
		bool alarms = Alarm(controller);
		return alarms;
	}

	public bool Alarm(_HunterStateController controller){
		if (controller.underInvokeList.Count > 0)
			return true;
		else
			return false;
	}
}
