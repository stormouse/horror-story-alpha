using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AIHunter/EnemyStats")]
public class _EnemyStats : ScriptableObject {
	public float moveSpeed = 1;
	public float lookRange = 20f;
	public float lookSphereCastRadius = 1;
	public FieldOfView fov;
	public float attackRange = 1f;
	public float attackRate = 1f;

	public float searchDuration = 4f;
	public float searchingTurnSpeed = 120f;

	public float fleeDuration = 3f;
}


