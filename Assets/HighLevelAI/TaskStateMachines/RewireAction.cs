using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "HighLevelAI/Actions/RewireAction")]
public class RewireAction : AIAction
{
    public override void Act(AIStateController controller)
    {
        controller.TransitionToState(TacticalPlanner.NextState(controller));
    }
}
