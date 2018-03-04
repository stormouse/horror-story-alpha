using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class PlayerCameraComtroller : NetworkBehaviour {
	public float m_cameraDistance_z = 20f;

	public Transform focusPlayer;//which player to look at
	private Transform mainCamera;
	private Vector3 cameraOffset;
	// Use this for initialization
	void Start () {
		if (!isLocalPlayer) {
			Destroy (this);
		}
		focusPlayer = this.transform;
		cameraOffset = new Vector3 (0f, 0f, -m_cameraDistance_z);
		mainCamera = Camera.main.transform;
		mainCamera.rotation = Quaternion.Euler (90f, 0f, 0f);
		CameraMove ();
	}

	private void CameraMove() {
		mainCamera.position = focusPlayer.position;
		mainCamera.LookAt (focusPlayer);
		mainCamera.Translate (cameraOffset);

	}
	// Update is called once per frame
	void Update () {
		CameraMove ();
	}
}
