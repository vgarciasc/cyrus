using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
	[Header("References")]
	[SerializeField]
	SlotColumn column;
	[SerializeField]
	AttackManager attackManager;

	//delegates
	public delegate void AttackDelegate();
	public event AttackDelegate start_enemy_attack_event,
								end_enemy_attack_event;

	public void enemy_turn() {
		StartCoroutine(handle_turn());
	}

	IEnumerator handle_turn() {
		yield return new WaitForSeconds(0.1f);

		if (start_enemy_attack_event != null) {
			start_enemy_attack_event();
		}

		for (int i = 0; i < column.charObj.Count; i++) {
			var character = column.charObj[i];
			
			//dead characters dont attack
			if (!character.gameObject.activeSelf) {
				continue;
			}

			attackManager.enemy_attack(character);
			yield return new WaitForSeconds(1f);
		}

		if (end_enemy_attack_event != null) {
			end_enemy_attack_event();
		}
	}
}
