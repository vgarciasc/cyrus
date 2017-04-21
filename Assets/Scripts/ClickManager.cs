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
	[SerializeField]
	SwapManager swapManager;
	[SerializeField]
	EnemyManager enemyManager;

	//delegates
	public delegate void AttackDelegate(CharacterObject charObj);
	public event AttackDelegate activate_choosing_attack_target_event,
								deactivate_choosing_attack_target_event,
								set_attack_target_event;
	public delegate void SwapDelegate(CharacterObject charObj);
	public event SwapDelegate activate_choosing_swap_target_event,
								deactivate_choosing_swap_target_event,
								set_swap_target_event;
	public delegate void AttribDelegate(CharacterObject charObj);
	public event AttribDelegate activate_choosing_attrib_target_event,
								deactivate_choosing_attrib_target_event,
								set_attrib_target_event;
	public delegate void VoidDelegate();
	public event VoidDelegate deactivate_all_lanes,
							conclude_attack_event,
							conclude_swap_event;

	//variables
	enum States {NOTHING, CHOOSING_ACTION, DISPLAYING_ATTACK_INFO,
				DISPLAYING_CHAR_ATTRIBUTES, SWAPPING_ALLIES, IS_ATTACKING,
				ENEMY_TURN};
	States currentState;

	void Start() {
		enemyManager.end_enemy_attack_event += end_enemy_turn;
	}

	public void click_character(CharacterObject charObj) {
		switch (currentState) {
			case States.CHOOSING_ACTION:
				charObj.column.toggle_all_lanes(false);
				charObj.lane.toggle(true);
				break;

			case States.NOTHING:
				currentState = States.CHOOSING_ACTION;
				charObj.column.toggle_all_lanes(false);
				charObj.lane.toggle(true);
				break;

			case States.DISPLAYING_ATTACK_INFO:
				if (set_attack_target_event != null) {
					set_attack_target_event(charObj);
				}
				break;

			case States.DISPLAYING_CHAR_ATTRIBUTES:
				if (set_attrib_target_event != null) {
					set_attrib_target_event(charObj);
				}
				break;

			case States.SWAPPING_ALLIES:
				if (swapManager.is_swap_valid(charObj)) {
					currentState = States.NOTHING;
					if (deactivate_choosing_swap_target_event != null) {
						deactivate_choosing_swap_target_event(charObj);
					}
					if (conclude_swap_event != null) {
						conclude_swap_event();
					}
				}
				break;
		}
	}

	public void click_attack_button(CharacterObject charObj) {
		switch (currentState) {
			case States.CHOOSING_ACTION:
				if (charObj.has_actions()) {
					currentState = States.DISPLAYING_ATTACK_INFO;
					if (deactivate_all_lanes != null) {
						deactivate_all_lanes();
					}				
					if (activate_choosing_attack_target_event != null) {
						activate_choosing_attack_target_event(charObj);
					}
				}
				break;
		}
	}

	public void click_swap_button(CharacterObject charObj) {
		switch (currentState) {
			case States.CHOOSING_ACTION:
				if (charObj.has_actions()) {
					currentState = States.SWAPPING_ALLIES;
					if (deactivate_all_lanes != null) {
						deactivate_all_lanes();
					}				
					if (activate_choosing_swap_target_event != null) {
						activate_choosing_swap_target_event(charObj);
					}
				}
				break;
		}
	}

	public void click_attrib_button(CharacterObject charObj) {
		switch (currentState) {
			case States.CHOOSING_ACTION:
				currentState = States.DISPLAYING_CHAR_ATTRIBUTES;
				if (activate_choosing_attrib_target_event != null) {
					activate_choosing_attrib_target_event(charObj);
				}
				if (deactivate_all_lanes != null) {
					deactivate_all_lanes();
				}				
				break;
		}
	}

	public void click_confirmation_button() {
		switch (currentState) {
			case States.DISPLAYING_ATTACK_INFO:
				currentState = States.NOTHING;
				if (conclude_attack_event != null) {
					conclude_attack_event();
				}
				break;
		}
	}

	public void click_endturn_button() {
		switch (currentState) {
			//just end the turn
			default:
				currentState = States.ENEMY_TURN;
				arenaManager.refresh_character_actions();
				if (deactivate_all_lanes != null) {
					deactivate_all_lanes();
				}	
				break;	
		}
	}

	public void click_cancel_button(CharacterObject charObj) {
		switch (currentState) {
			case States.CHOOSING_ACTION:
				currentState = States.NOTHING;
				charObj.lane.toggle(false);
				break;

			case States.DISPLAYING_ATTACK_INFO:
				currentState = States.CHOOSING_ACTION;
				charObj.lane.toggle_lane(true);
				if (deactivate_choosing_attack_target_event != null) {
					deactivate_choosing_attack_target_event(charObj);
				}
				break;

			case States.DISPLAYING_CHAR_ATTRIBUTES:
				currentState = States.CHOOSING_ACTION;
				charObj.lane.toggle_lane(true);
				if (deactivate_choosing_attrib_target_event != null) {
					deactivate_choosing_attrib_target_event(charObj);
				}
				break;

			case States.SWAPPING_ALLIES:
				currentState = States.NOTHING;
				charObj.lane.toggle(false);
				if (deactivate_choosing_swap_target_event != null) {
					deactivate_choosing_swap_target_event(charObj);
				}
				break;
		}
	}

	public void end_enemy_turn() {
		switch (currentState) {
			case States.ENEMY_TURN:
				currentState = States.NOTHING;

				break;
		}
	}
}
