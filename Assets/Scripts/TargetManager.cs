using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public enum TargetPassive {
	LAST_DEFENDER, //target is last character to receive an attack
	LAST_ATTACKER, //target is last character to attack
	ADJACENT_TO_CASTER, //target is all characters adjacent to skill caster
	CASTER //target is the skill caster
};
public enum TargetActive {
	ADJACENT_SWAPPABLE, //ADJACENT (TARGET 1): targets allies adjacent to TARGET 1 that are swappable
	ENEMIES_LANE_SWAPPABLE, //ENEMIES_LANE (TARGET 1): targets enemies in lane of TARGET 1
	ENEMIES_LANE_ANY, //ENEMIES_LANE (TARGET 1): targets enemies in lane of TARGET 1 that are swappable
	ADJACENT_ANY, //ADJACENT (TARGET 1): targets any allies adjacent to TARGET 1
	SELF, //SELF: targets self
	SLOT_SELF, //SLOT SELF: targets slot self
	SLOTS_CASTER_COLUMN, //SLOTS SELF COLUMN: targets slots in caster column
	SLOTS_OPPOSITE_COLUMN, //SLOTS ENEMY COLUMN: targets slots in enemy column
	CASTER_COLUMN, //CASTER COLUMN: targets characters in caster column
	OPPOSITE_COLUMN //OPPOSITE COLUMN: targets characters in opposite column to caster
};

public class TargetManager : MonoBehaviour {
	public static TargetManager getTargetManager() {
		return (TargetManager) HushPuppy.safeFindComponent("GameController", "TargetManager");
	}

	List<Targettable> current_targets = new List<Targettable>();
	Targettable selected_target = null;
	Targettable last_selected_target = null;

	[SerializeField]
	ArenaManager arenaManager;
	[SerializeField]
	ClickManager clickManager;
	[SerializeField]
	GameObject confirmation;
	[SerializeField]
	GameObject cancellation;
	[SerializeField]
	GameObject skillDescriptionPanel;
	[SerializeField]
	Text skillTitle;
	[SerializeField]
	Text skillDescription;

	bool selecting_slot = false;

	[HideInInspector]
	public States current_state = States.INITIAL_STATE;
	public enum States {
		INITIAL_STATE,
		SHOW_TARGETS,
		SHOW_CONFIRMATION
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.K)) {
			for (int i = 0; i < 4; i++) {
				// Debug.Log("slotID: " + i + "\nslotbg: " + 
				// ArenaManager.getArenaManager().get_player_column().get_slotbg_by_slotID(i));
			}
		}
	}

	public Targettable GetTarget() {
		return last_selected_target;
	}

	void update_targets(CharacterObject user, ElementActive elem, List<Targettable> targets) {
		List<Targettable> output = new List<Targettable>();
		Targettable tgt_1;
		Targettable tgt_2;
		
		if (elem.targetIndex.Count == 0) {
			Debug.Log("Skill is targetting but does not have targets.");
			Debug.Break();
		}

		if (elem.targetIndex[0] == -1) {
			tgt_1 = user.target;
		}
		else {
			tgt_1 = targets[elem.targetIndex[0]];
		}

		if (tgt_1.GetCharacter() == null) {
			Debug.Log("Aiming for a slot. If not desired outcome, please check.");
		}

		switch (elem.target) {
			case TargetActive.ADJACENT_ANY:
				foreach (CharacterObject co in user.column.get_adjacent_characters(tgt_1.GetCharacter())) {
					output.Add(co.target);
				}
				break;

			case TargetActive.ADJACENT_SWAPPABLE:
				foreach (CharacterObject co in arenaManager.get_swap_targets(tgt_1.GetCharacter())) {
					output.Add(co.target);
				}
				break;

			case TargetActive.ENEMIES_LANE_ANY:
				foreach (CharacterObject co in arenaManager.get_attack_targets(tgt_1.GetCharacter())) {
					output.Add(co.target);
				}
				break;

			case TargetActive.ENEMIES_LANE_SWAPPABLE:
				var aux = arenaManager.get_attack_targets(tgt_1.GetCharacter());
				foreach (CharacterObject co in aux) {
					if (co.status.getSwappable()) {
						output.Add(co.target);
					}
				}
				break;

			case TargetActive.SELF:
				output.Add(user.target);
				break;

			case TargetActive.SLOT_SELF:
				selecting_slot = true;
				output.Add(user.target);
				break;
			
			case TargetActive.SLOTS_CASTER_COLUMN:
				selecting_slot = true;
				foreach (CharacterObject co in user.column.charObj) {
					output.Add(co.target);
				}
				break;

			case TargetActive.SLOTS_OPPOSITE_COLUMN:
				selecting_slot = true;
				foreach (CharacterObject co in arenaManager.get_other_column(user.column).charObj) {
					output.Add(co.target);
				}
				break;

			case TargetActive.CASTER_COLUMN:
				foreach (CharacterObject co in user.column.charObj) {
					output.Add(co.target);
				}
				break;

			case TargetActive.OPPOSITE_COLUMN:
				foreach (CharacterObject co in arenaManager.get_other_column(user.column).charObj) {
					output.Add(co.target);
				}
				break;
		}

		if (output.Count == 0) {
			//no targets available
			//what can i do, i'm just an elf
			Debug.Log("No targets available for skill.");
		}
		current_targets = output;
	}

	public void enter_targetting(CharacterObject user, ElementActive elem, List<Targettable> targets) {
		selecting_slot = false;
		update_targets(user, elem, targets);

		switch (current_state) {
			case States.INITIAL_STATE:
				current_state = States.SHOW_TARGETS;
				
				user.lane.toggle_cancel(false);
				toggle_skill_panel(true);
				highlight_targets(true);
				toggle_cancellation(true);
				
				break;
		}

		//no choice
		// if (current_targets.Count == 1) {
		// 	click_character(current_targets[0]);
		// 	click_confirmation_button();
		// }
	}

	public void click_character(Targettable clicked) {
		if (!current_targets.Contains(clicked)) {
			return;
		}
		
		switch (current_state) {
			case States.SHOW_CONFIRMATION:
			case States.SHOW_TARGETS:
				current_state = States.SHOW_CONFIRMATION;
				
				last_selected_target = selected_target;
				selected_target = clicked;

				highlight_targets(true);
				toggle_confirmation(true);

				break;
		}
	}

	public void click_confirmation_button() {
		switch (current_state) {
			case States.SHOW_CONFIRMATION:
				current_state = States.INITIAL_STATE;

				if (selecting_slot) {
					var obj = selected_target.GetComponent<CharacterObject>();
					last_selected_target = selected_target.GetCharacter().column.get_slotbg_by_charobj(obj).target;
				} else {
					last_selected_target = selected_target;
				}

				selected_target = null;

				highlight_targets(false);
				toggle_confirmation(false);
				toggle_cancellation(false);
				toggle_skill_panel(false);
				
				break;
		}
	}

	public void click_cancel_button() {
		switch (current_state) {
			case States.SHOW_CONFIRMATION:
			case States.SHOW_TARGETS:
				last_selected_target = selected_target = null;

				current_state = States.INITIAL_STATE;

				highlight_targets(false);
				toggle_confirmation(false);
				toggle_cancellation(false);
				toggle_skill_panel(false);
				
				break;
		}
	}

	void highlight_targets(bool value) {
		if (value) {
			for (int i = 0; i < current_targets.Count; i++) {
				current_targets[i].toggle_targetlow(true);
				current_targets[i].toggle_targethi(false);
			}

			if (selected_target != null) {
				selected_target.toggle_targetlow(false);
				selected_target.toggle_targethi(true);
			}
		}
		else {
			for (int i = 0; i < current_targets.Count; i++) {
				current_targets[i].toggle_targets(false);
			}
		}
	}

	void toggle_confirmation(bool value) {
		confirmation.gameObject.SetActive(value);
	}

	void toggle_cancellation(bool value) {
		cancellation.gameObject.SetActive(value);
	}

	public void show_skill_panel(ActiveSkillData skl) {
		skillTitle.text = skl.title;
		skillDescription.text = skl.description;
	}

	void toggle_skill_panel(bool value) {
		skillDescriptionPanel.SetActive(value);
	}
}
