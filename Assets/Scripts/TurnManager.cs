using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {
	public static TurnManager getTurnManager() {
		return (TurnManager) HushPuppy.safeFindComponent("TurnManager", "TurnManager");
	}

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
	[SerializeField]
	BuffManager buffManager;
	[SerializeField]
	SkillManagerDeluxe skillManager;

	public delegate void VoidDelegate();
	public event VoidDelegate another_turn;

	void Start() {
		endPlayerTurnButton.SetActive(false);

		enemyManager.end_enemy_attack_event += EndEnemyTurn;
		clickManager.conclude_turn_event += EndPlayerTurn;

		StartCoroutine(handleTurns());
	}

	IEnumerator handleTurns() {
		Coroutine aux = null;
		yield return StartCoroutine(skillManager.passiveManager.on_match_start());

		while (true) {
			yield return update_buffs();

			aux = StartCoroutine(skillManager.passiveManager.on_turn_start(arenaManager.get_player_column()));
			yield return aux;

			StartPlayerTurn();

			//player turn
			yield return new WaitUntil(() => end_player_turn);
			end_player_turn = false;
			//player turn

			yield return StartCoroutine(StartEnemyTurn());

			//enemy turn
			yield return new WaitForSeconds(1.0f);
			yield return new WaitUntil(() => end_enemy_turn);
			end_enemy_turn = false;
			//enemy turn
		}
	}

	void StartPlayerTurn() {
		clickManager.can_click(true);
		endPlayerTurnButton.SetActive(true);
		arenaManager.refresh_character_actions();
	}

	public void EndPlayerTurn() {
		clickManager.can_click(false);
		endPlayerTurnButton.SetActive(false);
		end_player_turn = true;

		if (another_turn != null) {
			another_turn();
		}
	}

	IEnumerator StartEnemyTurn() {
		arenaManager.refresh_character_actions();
		yield return enemyManager.enemy_turn();
	}

	void EndEnemyTurn() {
		end_enemy_turn = true;

		if (another_turn != null) {
			another_turn();
		}
	}

	IEnumerator update_buffs () {
		foreach (CharacterObject co in arenaManager.get_enemy_column().charObj) {
			yield return co.status.update_buff();
		}
		foreach (SlotBackground sb in arenaManager.get_enemy_column().slotsBackground) {
			yield return sb.slotBuff.update_buff();
		}
		foreach (CharacterObject co in arenaManager.get_player_column().charObj) {
			yield return co.status.update_buff();
		}
		foreach (SlotBackground sb in arenaManager.get_player_column().slotsBackground) {
			yield return sb.slotBuff.update_buff();
		}
	}
}
