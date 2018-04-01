using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class _Decision : ScriptableObject {
	public abstract bool Decide (_HunterStateController controller);
}
