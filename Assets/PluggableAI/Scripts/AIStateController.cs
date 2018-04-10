using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIStateController : MonoBehaviour {

	public AIState currentState;
	public EnemyStats enemyStats;
	public Transform eyes;
	public AIState remainState;

	public LayerMask targetMask;
	public LayerMask obstacleMask;

	[HideInInspector] public NavMeshAgent navMeshAgent;
	[HideInInspector] public List<Transform> wayPointList;
	[HideInInspector] public int nextWayPoint;
	[HideInInspector] public List<Transform> visibleTargets = new List<Transform>();
	[HideInInspector] public int targetIndx = -1;
	[HideInInspector] public SurvivorSkills survivorsk;
	[HideInInspector] public float timeElapsed;

	[HideInInspector] public bool aiActive;
	[HideInInspector] public NetworkCharacter character;
	[HideInInspector] public GameObject[] players;
	[HideInInspector] public int[] predictTargets;
	[HideInInspector] public int playerIndex; //this index of this AI in players array
	void Awake() {
		navMeshAgent = GetComponent<NavMeshAgent> ();
		survivorsk = GetComponent<SurvivorSkills> ();
		character = GetComponent<NetworkCharacter> ();
	}

	public void SetupAI(bool aiActivationFromLevelManager, List<Transform> wayPointFromLevelManager, GameObject[] playersFromLevelManager) {
		wayPointList = wayPointFromLevelManager;
		aiActive = aiActivationFromLevelManager;
		players = playersFromLevelManager;
		predictTargets = new int[players.Length];
		for (int i = 0; i < players.Length; i++) {
			if (players [i] == this.gameObject) {
				playerIndex = i;
				break;
			}
		}
		if (aiActive) {
			navMeshAgent.enabled = true;
		} else {
			navMeshAgent.enabled = false;
		}
	}

    
    public void DisableAI()
    {
        aiActive = false;
        navMeshAgent.enabled = false;
    }

    public void ResumeAI()
    {
        aiActive = true;
        navMeshAgent.enabled = true;
    }


	void Update() {
		if (!aiActive) {
			return;
		}
		currentState.UpdateState (this);
	}
	//Draw Gizmos
	void OnDrawGizmos() {
		if (currentState != null && eyes != null) {
			Gizmos.color = currentState.sceneGizmoColor;
			Gizmos.DrawWireSphere (eyes.position, enemyStats.lookSphereCastRadius);
		}
	}

    public void TransitionToState (AIState nextState) {
		if (nextState != remainState) {
			currentState = nextState;
			OnExitState ();
		}
	}

	public bool CheckIfCountDownElapsed(float duration) {
		timeElapsed += Time.deltaTime;
		return (timeElapsed >= duration);
	}

	private void OnExitState() {
		timeElapsed = 0;
		survivorsk.m_Charging = false;
		targetIndx = -1;
		navMeshAgent.isStopped = true;
	}
}
