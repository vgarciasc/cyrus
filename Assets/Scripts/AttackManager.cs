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
		IEnumerator complete_attack(AttackBonus bonus) {
			yield return StartCoroutine(simple_attack(bonus));

			if (char_defender.is_dead()) {
				char_defender.column.kill_slot(char_defender);
				yield break;
			}			

			yield return new WaitForSeconds(0.3f);
			
			var aux = char_attacker;
			char_attacker = char_defender;
			char_defender = aux;

			yield return StartCoroutine(simple_counter(bonus));

			if (char_defender.is_dead()) {
				char_defender.column.kill_slot(char_defender);
				yield break;
			}
		}

		IEnumerator simple_attack(AttackBonus bonus) {
			if (char_attacker.is_dead() || char_defender.is_dead()) {
				yield break;
			}

			yield return block(AttackModule.NORMAL_ATTACK);

			char_attacker.use_action();

			yield return StartCoroutine(char_attacker.attack_motion());

			Damage dmg = calculator.effective_damage(
				char_attacker,
				char_defender,
				AttackModule.NORMAL_ATTACK,
				bonus
			);

			PassiveSkillManager passive = skillManager.passiveManager;
			var on_attack = StartCoroutine(passive.on_attack(char_attacker, char_defender, dmg));
			yield return on_attack;

			char_defender.take_hit(dmg.amount);

			if (dmg.amount == 0) {
				yield return StartCoroutine(char_defender.dodge_motion());
			}
			
			yield break;
		}

		IEnumerator simple_counter(AttackBonus bonus) {
			if (char_attacker.is_dead() || char_defender.is_dead()) {
				yield break;
			}

			// yield return block(AttackModule.COUNTER_ATTACK);

			Damage dmg = calculator.effective_damage(
				char_attacker,
				char_defender,
				AttackModule.COUNTER_ATTACK,
				bonus);

			char_attacker.use_action();
			var motion = StartCoroutine(char_attacker.counter_attack_motion());
			if (dmg.amount == 0) {
				StartCoroutine(char_defender.dodge_motion());
			} 
			
			yield return motion;
			
			PassiveSkillManager passive = skillManager.passiveManager;
			var on_attack = StartCoroutine(passive.on_counter_attack(char_attacker, char_defender, dmg));
			yield return on_attack;

			char_defender.take_hit(dmg.amount);

			//makes blocker go back to his place
			yield return block_end();

			yield break;
		}

		IEnumerator block(AttackModule mod) {
			PassiveSkillManager passive = skillManager.passiveManager;
			CharacterObject blocker = null;

			//CHECKING IF WILL BE BLOCKED
			for (int i = 0; i < char_defender.column.charObj.Count && blocker == null; i++) {
				float prob = passive.Get_Block_Probability(char_attacker, char_defender, char_defender.column.charObj[i], mod);
				if (ViolenceCalculator.pass_test(prob)) {
					blocker = char_defender.column.charObj[i];
				}
			}

			if (blocker == null || !blocker.has_actions()) {
				yield break;
			}
			//END_CHECKING_BLOCKING

			yield return blocker.block_attack_motion_start(char_defender);

			yield return new WaitForSeconds(0.3f);

			yield return passive.on_block(blocker, char_attacker, char_defender, mod);

			char_defender = blocker;
		}

		IEnumerator block_end() {
			for (int i = 0; i < char_attacker.column.charObj.Count; i++) {
				var ch = char_attacker.column.charObj[i];
				if (ch.is_blocking) {
					yield return ch.block_attack_motion_end();
				}
			}

			for (int i = 0; i < char_defender.column.charObj.Count; i++) {
				var ch = char_defender.column.charObj[i];
				if (ch.is_blocking) {
					yield return ch.block_attack_motion_end();
				}
			}
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
				StartCoroutine(complete_attack(null));
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
				
				yield return StartCoroutine(complete_attack(null));
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

	#region skill interfaces
		public IEnumerator complete_attack(CharacterObject attacker,
										CharacterObject defender,
										AttackBonus bonus) {
			
			this.char_attacker = attacker;
			this.char_defender = defender;
			
			yield return complete_attack(bonus);
		}

		public IEnumerator simple_attack(CharacterObject attacker,
										CharacterObject defender,
										AttackBonus bonus) {
			
			this.char_attacker = attacker;
			this.char_defender = defender;
			
			yield return simple_attack(bonus);
		}

		public IEnumerator simple_counter(CharacterObject attacker,
										CharacterObject defender,
										AttackBonus bonus) {
			
			this.char_attacker = attacker;
			this.char_defender = defender;
			
			yield return simple_counter(bonus);
		}
	#endregion
}
