using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackManager : MonoBehaviour {
	[HeaderAttribute("References")]
	public GameObject attackScreen;
	public Text textLeft, textRight;
	[HideInInspector]
	public CharacterObject char_attacker,
							char_defender;

	[SerializeField]
	ClickManager clickManager;
	[SerializeField]
	ArenaManager arenaManager;
	[SerializeField]
	BuffManager buffManager;
	[SerializeField]
	SkillManagerDeluxe skillManager;
	[SerializeField]
	ViolenceCalculator calculator;

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
			yield return StartCoroutine(simple_attack());

			if (char_defender.is_dead()) {
				char_defender.column.kill_slot(char_defender);
				yield break;
			}			

			yield return new WaitForSeconds(0.3f);

			yield return StartCoroutine(simple_counter());

			if (char_defender.is_dead()) {
				char_defender.column.kill_slot(char_defender);
				yield break;
			}
		}

		IEnumerator simple_attack() {
			char_attacker.use_action();

			yield return StartCoroutine(char_attacker.attack_motion());

			Damage dmg = calculator.effective_damage(
				char_attacker,
				char_defender,
				AttackModule.NORMAL_ATTACK
			);

			// buffManager.on_attack_buffs(
			// 	char_attacking,
			// 	char_attacked,
			// 	AttackModule.NORMAL_ATTACK)

			PassiveSkillManager passive = skillManager.passiveManager;
			var on_attack = StartCoroutine(passive.on_attack(char_attacker, char_defender, dmg));
			yield return on_attack;

			/*
				update attacker, defender and damage after on_attack
				char_attacker = skillManager.passiveManager.attacker;			
			
			 */

			char_defender.take_hit(dmg.amount);
			
			yield break;
		}

		IEnumerator simple_counter() {
			var aux = char_attacker;
			char_attacker = char_defender;
			char_defender = aux;

			char_attacker.use_action();
			yield return StartCoroutine(char_attacker.counter_attack_motion());

			Damage dmg = calculator.effective_damage(
				char_attacker,
				char_defender,
				AttackModule.COUNTER_ATTACK);

			char_defender.take_hit(dmg.amount);

			//TODO: CHANGE TO FIT SIMPLE_ATTACK MODEL

			yield break;
		}

		IEnumerator block(CharacterObject blocker) {
			yield return StartCoroutine(blocker.block_attack_motion_start(char_defender));

			yield break;
		}

		#region player
			//an attacker has been selected
			void start_attack(CharacterObject charObj) {
				char_attacker = charObj;
				char_defender = arenaManager.get_default_attack_target(charObj);
				set_target(char_defender);
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
			public IEnumerator enemy_attack(CharacterObject enemy) {
				char_attacker = enemy;
				char_defender = arenaManager.get_default_attack_target(enemy);
				
				yield return StartCoroutine(attack_counterattack());
			}
		#endregion
	#endregion

	#region attack info
		//a target has been selected, now we can show info
		void set_target(CharacterObject charObj) {
			if (!is_target_valid(charObj)) {
				return;
			}

			char_defender = charObj;
			textRight.text = get_char_info(false);
			textLeft.text = get_char_info(true);

			if (set_new_target_event != null) {
				set_new_target_event(char_attacker, char_defender);
			}
		}

		string get_char_info(bool counter) {
			AttackModule mod = AttackModule.NORMAL_ATTACK;
			if (counter) mod = AttackModule.COUNTER_ATTACK; 

			string aux = "";
			aux += "\nDMG: <color=gray>" + calculator.theoretical_damage(counter? char_attacker : char_defender,
															counter? char_defender : char_attacker,
															mod) + "</color>";
			aux += "\nACC: <color=gray>" + calculator.accuracy_prob(counter? char_attacker : char_defender,
															counter? char_defender : char_attacker,
															mod) + "%</color>";
			aux += "\n\n";
			if (counter) aux += "<color=red>COUNTER!</color>";
			else aux += "<color=red><<<<</color>";
			
			return aux;
		}

		void toggle_screen(bool value) {
			attackScreen.SetActive(value);
		}

		public bool is_target_valid(CharacterObject target) {
			return arenaManager.get_attack_targets(char_attacker).Contains(target);
		}
	#endregion
}
