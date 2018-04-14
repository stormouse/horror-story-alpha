using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIStateController : MonoBehaviour {
	//hunter's parameters
	public float visionRange;
	public float attackRange;
	public float hookRange;
	public float navStopDistance;
	public float pSStopDistance;
	[HideInInspector] public List<NetworkCharacter> chaseTarget;
	[HideInInspector] public Vector3 wonderPoint;
	[HideInInspector] public List<Transform> underInvokeList;
	[HideInInspector] public HunterSkills hskills;



	public AIState currentState;
	public EnemyStats enemyStats;
	public AIState remainState;

	public LayerMask targetMask;
	public LayerMask obstacleMask;

	[HideInInspector] public Vector3 fleeDest;
	[HideInInspector] public float fleeOffsetMultiplyBy;
	[HideInInspector] public NavMeshAgent navMeshAgent;
	[HideInInspector] public List<Transform> wayPointList;
	[HideInInspector] public int nextWayPoint;
	[HideInInspector] public List<Transform> visibleTargets = new List<Transform>();
	[HideInInspector] public int targetIndx = -1;
	[HideInInspector] public SurvivorSkills survivorsk;
	[HideInInspector] public float timeElapsed;

	[HideInInspector] public bool aiActive;
	[HideInInspector] public NetworkCharacter character;
	[HideInInspector] public List<GameObject> players;
	[HideInInspector] public int[] predictTargets;
	[HideInInspector] public int playerIndex; //this index of this AI in players array
	void Awake() {
		navMeshAgent = GetComponent<NavMeshAgent> ();
		survivorsk = GetComponent<SurvivorSkills> ();
		character = GetComponent<NetworkCharacter> ();
		hskills = GetComponent<HunterSkills> ();
		navMeshAgent.stoppingDistance = navStopDistance;
	}

	public void SetupAI(bool aiActivationFromLevelManager, List<Transform> wayPointFromLevelManager, List<GameObject> playersFromLevelManager) {
		wayPointList = wayPointFromLevelManager;
		aiActive = aiActivationFromLevelManager;
		if (playersFromLevelManager != null) {
			players = playersFromLevelManager;
			predictTargets = new int[players.Count];
		}
		nextWayPoint = Random.Range (0, wayPointList.Count); //Random for hunter starting patrol point
		for (int i = 0; i < players.Count; i++) {
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
		if (survivorsk != null) {
			survivorsk.m_Charging = false;
		}
		targetIndx = -1;
		navMeshAgent.isStopped = true;
		fleeOffsetMultiplyBy = 0;
	}
}
