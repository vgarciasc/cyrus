using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour {
	public static ClickManager getClickManager() {
		return (ClickManager) HushPuppy.safeFindComponent("GameController", "ClickManager");
	}

	//references
	[HeaderAttribute("References")]
	[SerializeField]
	ArenaManager arenaManager;
	[SerializeField]
	AttackManager attackManager;

	//delegates
	public delegate void AttackDelegate(CharacterObject charObj);
	public event AttackDelegate start_attack_event,
								cancel_attack_event,
								set_attack_target_event;
	public delegate void VoidDelegate();
	public event VoidDelegate show_duel_info_event,
							conclude_attack_event;

	//variables
	bool attacking = false;

	public void click_character(CharacterObject charObj) {
		if (attacking) {
			if (set_attack_target_event != null) {
				set_attack_target_event(charObj);
			}

			if (show_duel_info_event != null) {
				show_duel_info_event();
			}

			// attack(charObj);
		}
	}

	public void click_attack_button(CharacterObject charObj) {
		if (!attacking) {
			if (start_attack_event != null) {
				start_attack_event(charObj);
			}

			attacking = true;
		}
		else {
			if (cancel_attack_event != null) {
				cancel_attack_event(charObj);
			}

			attacking = false;
		}
	}

	public void click_confirmation_button() {
		if (attacking) {
			attacking = false;
			if (conclude_attack_event != null) {
				conclude_attack_event();
			}
		}
	}
}
