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
    public float defaultMoveSpeed;

	[HideInInspector] public List<NetworkCharacter> chaseTarget;
	[HideInInspector] public Vector3 wonderPoint;
	[HideInInspector] public List<Transform> underInvokeList;
	[HideInInspector] public HunterSkills hskills;


	public List<AIState> stateForBlock;
	public AIState currentState;
	public EnemyStats enemyStats;
	public AIState remainState;
    public SpeedMultiplier speedMultiplier;

    public float lastTimeInDanger;

    public CharacterRole characterRole;

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
	[HideInInspector] public SurvivorBlackBoard survivorBD;
	[HideInInspector] public float SmokeUsageCooldownMultiply = 0;

	void Awake() {
		navMeshAgent = GetComponent<NavMeshAgent> ();
		survivorsk = GetComponent<SurvivorSkills> ();
		character = GetComponent<NetworkCharacter> ();
		hskills = GetComponent<HunterSkills> ();
		navMeshAgent.stoppingDistance = navStopDistance;
        if(speedMultiplier == null)
            speedMultiplier = GetComponent<SpeedMultiplier>();
    }
	public void SetupSurvivorBD(SurvivorBlackBoard sbd) {
		survivorBD = sbd;
	}
	public void SetupAI(bool aiActivationFromLevelManager, List<Transform> wayPointFromLevelManager, List<GameObject> playersFromLevelManager) {
		wayPointList = wayPointFromLevelManager;
		aiActive = aiActivationFromLevelManager;
		targetIndx = -1;
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
        if (navMeshAgent && navMeshAgent.isActiveAndEnabled && speedMultiplier)
        {
            navMeshAgent.speed = defaultMoveSpeed * speedMultiplier.value;
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
		if (survivorsk != null) {
			survivorsk.m_Charging = false;
		}
		if (!IsValidState ()) {
			targetIndx = -1;
		}
		navMeshAgent.isStopped = true;
		fleeOffsetMultiplyBy = 0;
		SmokeUsageCooldownMultiply = 0;
	}
	public bool IsValidState() {
		foreach (AIState s in stateForBlock) {
			if (currentState == s) {
				return true;
			}
		}
		return false;
	}

    

    public bool Look()
    {
        visibleTargets.Clear();

        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, enemyStats.lookRange, targetMask);

        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            if (targetsInViewRadius[i].GetComponent<NetworkCharacter>().Team == GameEnum.TeamType.Hunter)
            {
                Transform target = targetsInViewRadius[i].transform;
                Vector3 dirToTarget = (target.position - transform.position).normalized;
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                //if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
               // {
                    visibleTargets.Add(target);
               // }
            }
        }

        if (visibleTargets.Count > 0)
        {
            lastTimeInDanger = Time.time;
            return true;
        }
        else
        {
            return false;
        }

    }
}
