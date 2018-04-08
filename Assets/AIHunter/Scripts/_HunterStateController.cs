using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class _HunterStateController : MonoBehaviour {

	public _State currentState;
	private bool aiActive;
	public Transform eye;
	public _EnemyStats enemyStats;
	public float visionRange;
	public float attackRange;
	public float hookRange;
	public float navStopDistance;
	public float pSStopDistance;
	public LayerMask targetMask;
	public LayerMask obstacleMask;
	public _State remainStats;

	[HideInInspector] public NavMeshAgent navMeshAgent;
	[HideInInspector] public List<PowerSourceController> wayPointList;
	[HideInInspector] public int nextWayPoint;
	[HideInInspector] public List<NetworkCharacter> chaseTarget;
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

	public void SetupAI(bool aiActivationFromManager, List<PowerSourceController> wayPointsFromManager){
		wayPointList = wayPointsFromManager;
		aiActive = aiActivationFromManager;
		nextWayPoint = Random.Range (0, wayPointList.Count);
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

		if (nextState == remainStats) {
			return;
		}
		currentState = nextState;
	}
}
