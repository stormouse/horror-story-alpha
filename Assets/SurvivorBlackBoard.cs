using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class SurvivorBlackBoard : NetworkBehaviour {
	//public float blockTime = 8;
	public int targetsNumber = 8;
	private bool[] powerSourcesBlock;
	private float[] powerSourcesTimer;

    
	void Start(){
        if (!isServer)
        {
            Destroy(gameObject);
        }
		powerSourcesTimer = new float[targetsNumber];
		powerSourcesBlock = new bool[targetsNumber];
        SetupforAI();
    }


    void Update () {
		UpdateTimer ();
	}

	public void ResetBD(int targetsNum) {
		powerSourcesTimer = new float[targetsNum];
		powerSourcesBlock = new bool[targetsNum];
		targetsNumber = targetsNum;
	}
	private void SetupforAI() {
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject p in players) {
			if (p.GetComponent<NetworkCharacter> ().Team == GameEnum.TeamType.Survivor) {
				AIStateController ai = p.GetComponent<AIStateController> ();
				if (ai != null) {
					ai.SetupSurvivorBD (this);
					Debug.Log ("Here!!!!");
				}
			}
		}
	}


	private void UpdateTimer () {
		for (int i = 0; i < targetsNumber; i++) {
			if (powerSourcesTimer[i] > 0) {
				powerSourcesTimer[i] -= Time.deltaTime;
				if (powerSourcesTimer [i] < 0) {
					powerSourcesTimer [i] = 0;
					powerSourcesBlock[i] = false;
				}
			}
		}
	}


	public void SetPSTimer(int i, float blockTime) {
			powerSourcesTimer [i] = blockTime;
			powerSourcesBlock [i] = true;
	}

	public bool IsPSBlock(int i) {
		return powerSourcesBlock [i];
	}
}
