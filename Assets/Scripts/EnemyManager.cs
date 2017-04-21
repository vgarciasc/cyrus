using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
	public delegate void AttackDelegate();
	public event AttackDelegate start_enemy_attack_event,
								end_enemy_attack_event;

	public void attack() {
		if (start_enemy_attack_event != null) {
			start_enemy_attack_event();
		}

		Debug.Log("enemy is attacking...");

		if (end_enemy_attack_event != null) {
			end_enemy_attack_event();
		}
	}
}
