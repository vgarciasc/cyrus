using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackManager : MonoBehaviour {
	[HeaderAttribute("References")]
	public GameObject attackScreen;
	public Text textLeft, textRight;
	[HideInInspector]
	public CharacterObject char_attacking,
							char_attacked;

	[SerializeField]
	ClickManager clickManager;
	[SerializeField]
	ArenaManager arenaManager;

	public delegate void AttackDelegate(CharacterObject targetter, CharacterObject target);
	public event AttackDelegate set_new_target_event;

	void Start() {
		clickManager.activate_choosing_attack_target_event += start_attack;
		clickManager.set_attack_target_event += set_target;
		clickManager.conclude_attack_event += conclude_attack;

		clickManager.deactivate_choosing_attack_target_event += cancel_attack;

		toggle_screen(false);
	}

	#region attack mechanics
		IEnumerator attack_counterattack() {
			normal_attack();

			if (char_attacked.is_dead()) {
				char_attacked.column.kill_slot(char_attacked);
				yield break;
			}			

			yield return new WaitForSeconds(0.3f);

			counter_attack();

			if (char_attacked.is_dead()) {
				char_attacked.column.kill_slot(char_attacked);
				yield break;
			}
		}

		void normal_attack() {
			char_attacking.use_action();
			char_attacking.attack_motion();

			char_attacked.take_hit(calculate_damage(char_attacked, char_attacking));
		}

		void counter_attack() {
			var aux = char_attacking;
			char_attacking = char_attacked;
			char_attacked = aux;

			char_attacking.use_action();
			char_attacking.counter_attack_motion();

			char_attacked.take_hit(calculate_damage(char_attacked, char_attacking));
		}

		#region player
			//an attacker has been selected
			void start_attack(CharacterObject charObj) {
				char_attacking = charObj;
				char_attacked = arenaManager.get_default_attack_target(charObj);
				set_target(char_attacked);
				toggle_screen(true);
			}

			void cancel_attack(CharacterObject charObj) {
				toggle_screen(false);
			}

			void conclude_attack() {
				StartCoroutine(attack_counterattack());
				toggle_screen(false);
			}

			//to be encapsulated in another class
			int calculate_damage(CharacterObject attacker, CharacterObject attacked) {
				return attacker.data.FOR - attacked.data.DEF;
			}
		#endregion
		
		#region enemy
			public void enemy_attack(CharacterObject enemy) {
				char_attacking = enemy;
				char_attacked = arenaManager.get_default_attack_target(enemy);
				
				StartCoroutine(attack_counterattack());
			}
		#endregion
	#endregion

	#region attack info
		//a target has been selected, now we can show info
		void set_target(CharacterObject charObj) {
			if (!is_target_valid(charObj)) {
				return;
			}

			char_attacked = charObj;
			textRight.text = get_char_info(char_attacking);
			textLeft.text = get_char_info(char_attacked);

			if (set_new_target_event != null) {
				set_new_target_event(char_attacking, char_attacked);
			}
		}

		string get_char_info(CharacterObject charObj) {
			string aux = "";
			aux += "FOR: " + charObj.data.FOR;
			aux += "\nAGI: " + charObj.data.AGI;
			return aux;
		}

		void toggle_screen(bool value) {
			attackScreen.SetActive(value);
		}

		public bool is_target_valid(CharacterObject target) {
			return arenaManager.get_attack_targets(char_attacking).Contains(target);
		}
	#endregion
}
