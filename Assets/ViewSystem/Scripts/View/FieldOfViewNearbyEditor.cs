using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor (typeof (FieldOfViewNearby))]
public class FieldOfViewNearbyEditor : Editor {

	void OnSceneGUI() {
		FieldOfViewNearby fow = (FieldOfViewNearby)target;
		Handles.color = Color.white;
		Handles.DrawWireArc (fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);

		Handles.color = Color.red;
		foreach (Transform visibleTarget in fow.visibleTargets) {
			Handles.DrawLine (fow.transform.position, visibleTarget.position);
		}
	}

}
#endif