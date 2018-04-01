using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu (menuName = "AIHunter/States")] 
public class _State : ScriptableObject {

	public _Action[] actions;
	public _Transition[] transitions;

	public Color sceneGizmoColor = Color.grey;

	public void UpdateStates(_HunterStateController controller){
		DoActions (controller);
		CheckTransitions (controller);
	}

	public void DoActions(_HunterStateController controller){
		for (int i = 0; i < actions.Length; i++) {
			actions [i].Act (controller);
		}
	}

	private void CheckTransitions(_HunterStateController controller){
		for (int i = 0; i < transitions.Length; i++) {
			bool decisionSucceed = transitions [i].decision.Decide (controller);
			if (decisionSucceed) {
				controller.TransitionToState (transitions [i].trueState);
			} else {
				controller.TransitionToState (transitions [i].falseState);
			}
		}
	}

}
