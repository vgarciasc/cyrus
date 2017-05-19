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
	[SerializeField]
	TargetManager targetManager;
	[SerializeField]
	SkillManagerDeluxe skillManager;

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
							deactivate_all_cancels,
							deactivate_all_lanes_cancels,
							conclude_attack_event,
							conclude_swap_event,
							conclude_turn_event;

	//variables
	enum States {NOT_CLICKABLE, NOTHING, CHOOSING_ACTION, DISPLAYING_ATTACK_INFO,
				DISPLAYING_CHAR_ATTRIBUTES, SWAPPING_ALLIES, IS_ATTACKING,
				ENEMY_TURN, TARGET_MANAGER};
	States currentState;
	CharacterObject focused_character;

	void Start() {
		enemyManager.end_enemy_attack_event += end_enemy_turn;

		can_click(false);
	}

	public void can_click(bool value) {
		switch (currentState) {
			case States.NOT_CLICKABLE:
				if (value) {
					currentState = States.NOTHING;
				}
				break;
			default:
				if (!value) {
					currentState = States.NOT_CLICKABLE;
				}
				break;
		}
	}

	public void click_character(CharacterObject charObj) {
		focused_character = charObj;
		
		switch (currentState) {
			case States.CHOOSING_ACTION:
				if (deactivate_all_lanes != null) {
					deactivate_all_lanes();
				}	
				if (deactivate_all_cancels != null) {
					deactivate_all_cancels();
				}	
				charObj.lane.toggle(true);
				break;

			case States.NOTHING:
				currentState = States.CHOOSING_ACTION;
				charObj.lane.toggle(true);
				break;

			case States.DISPLAYING_ATTACK_INFO:
				if (set_attack_target_event != null) {
					set_attack_target_event(charObj);
				}
				break;

			case States.DISPLAYING_CHAR_ATTRIBUTES:
				arenaManager.get_other_column(charObj.column).toggle_all_lanes(false);
				charObj.column.toggle_all_lanes(false);
				charObj.lane.toggle_cancel(true);
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
			
			case States.TARGET_MANAGER:
				targetManager.click_character(charObj.target);
				break;
		}
	}

	public void click_attack_button(CharacterObject charObj) {
		switch (currentState) {
			case States.CHOOSING_ACTION:
				if (charObj.has_general_actions()) {
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
				if (charObj.has_swap_actions()) {
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

			case States.TARGET_MANAGER:
				targetManager.click_confirmation_button();
				break;
		}

		focused_character = null;		
	}

	public void click_endturn_button() {
		switch (currentState) {
			case States.DISPLAYING_CHAR_ATTRIBUTES:
				currentState = States.ENEMY_TURN;
				if (deactivate_all_lanes_cancels != null) {
					deactivate_all_lanes_cancels();
				}		
				if (deactivate_choosing_attrib_target_event != null) {
					deactivate_choosing_attrib_target_event(null);
				}
				if (conclude_turn_event != null) {
					conclude_turn_event();
				}
				break;

			default:
				currentState = States.ENEMY_TURN;
				if (deactivate_all_lanes_cancels != null) {
					deactivate_all_lanes_cancels();
				}	
				if (deactivate_choosing_attack_target_event != null) {
					deactivate_choosing_attack_target_event(null);
				}
				if (conclude_turn_event != null) {
					conclude_turn_event();
				}
				break;	
		}

		focused_character = null;		
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
				currentState = States.CHOOSING_ACTION;
				charObj.lane.toggle_lane(true);
				if (deactivate_choosing_swap_target_event != null) {
					deactivate_choosing_swap_target_event(charObj);
				}
				break;

			case States.TARGET_MANAGER:

				targetManager.click_cancel_button();

				break;				
		}

		focused_character = null;		
	}

	public void click_cancel_button() {
		switch (currentState) {
			case States.CHOOSING_ACTION:
				currentState = States.NOTHING;
				focused_character.lane.toggle_lane(false);
				break;

			case States.DISPLAYING_ATTACK_INFO:
				currentState = States.CHOOSING_ACTION;
				focused_character.lane.toggle_lane(true);
				if (deactivate_choosing_attack_target_event != null) {
					deactivate_choosing_attack_target_event(focused_character);
				}
				break;

			case States.DISPLAYING_CHAR_ATTRIBUTES:
				currentState = States.CHOOSING_ACTION;
				focused_character.lane.toggle_lane(true);
				if (deactivate_choosing_attrib_target_event != null) {
					deactivate_choosing_attrib_target_event(focused_character);
				}
				break;

			case States.SWAPPING_ALLIES:
				currentState = States.CHOOSING_ACTION;
				focused_character.lane.toggle_lane(true);
				if (deactivate_choosing_swap_target_event != null) {
					deactivate_choosing_swap_target_event(focused_character);
				}
				break;

			case States.TARGET_MANAGER:
				targetManager.click_cancel_button();
				break;				
		}

		focused_character = null;
	}

	public void click_skill_button(CharacterObject charObj, ActiveSkillData skill) {
		if (!ActiveSkillManager.getActiveSkillManager().can_cast(charObj, skill)) {
			return;
		}

		switch (currentState) {
			case States.CHOOSING_ACTION:
				currentState = States.TARGET_MANAGER;
				
				StartCoroutine(skillManager.activeManager.CastSkill(skill, charObj, null, 0));
				charObj.lane.toggle_lane(false);

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

	public void end_targetting(CharacterObject caster, bool was_cast_successful) {
		switch (currentState) {
			case States.TARGET_MANAGER:
				if (was_cast_successful) {
					//if is not casting and casting was successful, go to NOTHING
					currentState = States.NOTHING;	

					if (deactivate_all_cancels != null) {
						deactivate_all_cancels();
					}
					
					break;
				}
				else {
					//if is not casting and casting was not successful, go back to choosing actions
					currentState = States.CHOOSING_ACTION;

					caster.lane.toggle_lane(true);
					
					break;
				}
		}
	}
}
