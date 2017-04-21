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
			char_attacking.use_action();
			attack();
		}

		void attack() {
			// char_attacked.column.kill_slot(char_attacked);
			char_attacking.attack_motion();
			if (char_attacked.take_hit(calculate_damage(char_attacked, char_attacking))) {
				char_attacked.column.kill_slot(char_attacked);	
			}

			toggle_screen(false);
		}

		int calculate_damage(CharacterObject attacker, CharacterObject attacked) {
			return attacker.data.FOR - attacked.data.DEF;
		}
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
			if (value == false) {
				char_attacked = null;
			}
			attackScreen.SetActive(value);
		}

		public bool is_target_valid(CharacterObject target) {
			return arenaManager.get_attack_targets(char_attacking).Contains(target);
		}
	#endregion
}
