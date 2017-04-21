using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {
	[HeaderAttribute("References")]
	[SerializeField]
	GameObject endPlayerTurnButton;

	enum Turn {PLAYER_TURN, ENEMY_TURN};
	Turn currentTurn = Turn.PLAYER_TURN;
	bool end_enemy_turn = false,
		end_player_turn = false;

	[SerializeField]
	EnemyManager enemyManager;
	[SerializeField]
	ArenaManager arenaManager;

	void Start() {
		endPlayerTurnButton.SetActive(false);

		enemyManager.end_enemy_attack_event += EndEnemyTurn;

		StartCoroutine(handleTurns());
	}

	IEnumerator handleTurns() {
		while (true) {

			StartPlayerTurn();
			yield return new WaitUntil(() => end_player_turn);
			end_player_turn = false;

			StartEnemyTurn();
			
			//pretend enemy is attacking
			yield return new WaitForSeconds(1.0f);

			yield return new WaitUntil(() => end_enemy_turn);
			end_enemy_turn = false;
		}
	}

	void StartPlayerTurn() {
		endPlayerTurnButton.SetActive(true);
		arenaManager.refresh_character_actions();
	}

	public void EndPlayerTurn() {
		endPlayerTurnButton.SetActive(false);
		end_player_turn = true;
	}

	void StartEnemyTurn() {
		enemyManager.attack();
		arenaManager.refresh_character_actions();
	}

	void EndEnemyTurn() {
		end_enemy_turn = true;
	}
}
