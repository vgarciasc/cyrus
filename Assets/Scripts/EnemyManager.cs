using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {
	[Header("References")]
	[SerializeField]
	SlotColumn column;
	[SerializeField]
	AttackManager attackManager;
	[SerializeField]
	SwapManager swapManager;

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

		List<CharacterObject> aux = new List<CharacterObject>();
		foreach (CharacterObject c in column.charObj) {
			if (c.gameObject.activeSelf) {
				aux.Add(c);
			}
		}

		List<CharacterObject> enemies = new List<CharacterObject>();
		while (aux.Count > 0) {
			int index = Random.Range(0, aux.Count - 1);
			enemies.Add(aux[index]);
			aux.RemoveAt(index);
		}

		for (int i = 0; i < enemies.Count; i++) {
			var character = enemies[i];

			int dice = Random.Range(0, 13);
			switch (dice) {
				default:
					attackManager.enemy_attack(character);
					break;
				case 0:
					swapManager.random_swap(character);
					break;
			}

			yield return new WaitForSeconds(1f);
		}

		if (end_enemy_attack_event != null) {
			end_enemy_attack_event();
		}
	}
}
