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

	public IEnumerator enemy_turn() {
		yield return StartCoroutine(handle_turn());
	}

	IEnumerator handle_turn() {
		yield return new WaitForSeconds(0.1f);

		if (start_enemy_attack_event != null) {
			start_enemy_attack_event();
		}

		//get all enemies
		List<CharacterObject> aux = new List<CharacterObject>();
		foreach (CharacterObject c in column.charObj) {
			if (c.gameObject.activeSelf) {
				aux.Add(c);
			}
		}

		//puts enemies in random order
		List<CharacterObject> enemies = new List<CharacterObject>();
		while (aux.Count > 0) {
			int index = Random.Range(0, aux.Count - 1);
			enemies.Add(aux[index]);
			aux.RemoveAt(index);
		}

		Coroutine toWait = null;

		//make enemy attack
		for (int i = 0; i < enemies.Count; i++) {
			var character = enemies[i];

			int dice = Random.Range(0, 13);
			switch (dice) {
				default:
					toWait = StartCoroutine(attackManager.enemy_attack(character));
					yield return toWait;
					break;
				case 0:
					toWait = StartCoroutine(swapManager.random_swap(character));
					yield return toWait;
					break;
			}

			yield return new WaitForSeconds(1f);
		}

		if (end_enemy_attack_event != null) {
			end_enemy_attack_event();
		}
	}
}
