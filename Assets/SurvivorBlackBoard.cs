using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class SurvivorBlackBoard : NetworkBehaviour {
	public float blockTime = 8;
	public int powerSourceNumber = 8;
	private bool[] powerSourcesBlock;
	private float[] powerSourcesTimer;
	private bool flag = true;
	private readonly object syncLock = new object();
	void Start(){
		powerSourcesTimer = new float[powerSourceNumber];
		powerSourcesBlock = new bool[powerSourceNumber];


	}
	

	void Update () {
		if (flag) {
			SetupforAI ();
		}
		UpdateTimer ();
	}
	private void SetupforAI() {
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject p in players) {
			if (p.GetComponent<NetworkCharacter> ().Team == GameEnum.TeamType.Survivor) {
				AIStateController ai = p.GetComponent<AIStateController> ();
				if (ai != null) {
					ai.SetupSurvivorBD (this);
					flag = false;
					Debug.Log ("Here!!!!");
				}
			}
		}
	}
	private void UpdateTimer () {
		for (int i = 0; i < powerSourceNumber; i++) {
			if (powerSourcesTimer[i] > 0 ) {
				powerSourcesTimer[i] -= Time.deltaTime;
				if (powerSourcesTimer [i] < 0) {
					powerSourcesTimer [i] = 0;
					powerSourcesBlock[i] = false;
				}
			}
		}
	}
	public void SetPSTimer(int i) {
		lock(syncLock) { 
			powerSourcesTimer [i] = blockTime;
			powerSourcesBlock [i] = true;
		}
	}
	public bool IsPSBlock(int i) {
		return powerSourcesBlock [i];
	}
}
