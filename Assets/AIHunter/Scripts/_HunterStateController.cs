using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class _HunterStateController : MonoBehaviour {

	public _State currentState;
	private bool aiActive;
	public Transform eye;
	public _EnemyStats enemyStats;
	public int visionRange;
	public int attackRange;
	public int hookRange;
	public int navStopDistance;
	public LayerMask targetMask;
	public LayerMask obstacleMask;

	[HideInInspector] public NavMeshAgent navMeshAgent;
	[HideInInspector] public List<Transform> wayPointList;
	[HideInInspector] public int nextWayPoint;
	[HideInInspector] public List<Transform> chaseTarget;
	[HideInInspector] public Vector3 wonderPoint;
	[HideInInspector] public List<Transform> underInvokeList;
	[HideInInspector] public HunterSkills hskills;
	[HideInInspector] public NetworkCharacter character;

	void Start ()
	{
		
	}

	void Awake(){
		navMeshAgent = GetComponent<NavMeshAgent> ();
		navMeshAgent.stoppingDistance = navStopDistance;
		hskills = GetComponent<HunterSkills> ();
		character = GetComponent<NetworkCharacter> ();
	}

	public void SetupAI(bool aiActivationFromManager, List<Transform> wayPointsFromManager){
		wayPointList = wayPointsFromManager;
		aiActive = aiActivationFromManager;
		 
		if (aiActive) {
			navMeshAgent.enabled = true;
		} else {
			navMeshAgent.enabled = false;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!aiActive)
			return;
		else
			currentState.UpdateStates (this);
	}

	public void TransitionToState (_State nextState) {
		currentState = nextState;
	}
}
