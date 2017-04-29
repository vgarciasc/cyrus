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
	[SerializeField]
	ClickManager clickManager;

	public delegate void VoidDelegate();
	public event VoidDelegate another_turn;

	void Start() {
		endPlayerTurnButton.SetActive(false);

		enemyManager.end_enemy_attack_event += EndEnemyTurn;
		clickManager.conclude_turn_event += EndPlayerTurn;

		StartCoroutine(handleTurns());
	}

	IEnumerator handleTurns() {
		while (true) {
			StartPlayerTurn();

			//player turn
			yield return new WaitUntil(() => end_player_turn);
			end_player_turn = false;
			//player turn

			StartEnemyTurn();

			//enemy turn
			yield return new WaitForSeconds(1.0f);
			yield return new WaitUntil(() => end_enemy_turn);
			end_enemy_turn = false;
			//enemy turn
		}
	}

	void StartPlayerTurn() {
		endPlayerTurnButton.SetActive(true);
		arenaManager.refresh_character_actions();
	}

	public void EndPlayerTurn() {
		endPlayerTurnButton.SetActive(false);
		end_player_turn = true;

		if (another_turn != null) {
			another_turn();
		}
	}

	void StartEnemyTurn() {
		enemyManager.enemy_turn();
		arenaManager.refresh_character_actions();
	}

	void EndEnemyTurn() {
		end_enemy_turn = true;

		if (another_turn != null) {
			another_turn();
		}
	}
}
