using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetKind {
	LAST_DEFENDER, //target is last character to receive an attack
	LAST_ATTACKER, //target is last character to attack
	ADJACENT_TO_CASTER, //target is all characters adjacent to skill caster
	CASTER //target is the skill caster
};

public class TargetManager : MonoBehaviour {
	public static TargetManager getTargetManager() {
		return (TargetManager) HushPuppy.safeFindComponent("GameController", "TargetManager");
	}

	List<CharacterObject> current_targets = new List<CharacterObject>();
	CharacterObject selected_target = null;
	CharacterObject last_selected_target = null;

	[SerializeField]
	ArenaManager arenaManager;
	[SerializeField]
	ClickManager clickManager;
	[SerializeField]
	GameObject confirmationScreen;

	[HideInInspector]
	public States current_state = States.INITIAL_STATE;
	public enum States {
		INITIAL_STATE,
		SHOW_TARGETS,
		SHOW_CONFIRMATION
	}

	public CharacterObject GetTarget() {
		return last_selected_target;
	}

	void update_targets(CharacterObject user, ElementActive elem, List<CharacterObject> targets) {

		List<CharacterObject> output = new List<CharacterObject>();
		CharacterObject tgt_1;
		CharacterObject tgt_2;

		switch (elem.target) {
			case TargetActive.ADJACENT_ANY:
				if (elem.targetIndex[0] == -1) {
					tgt_1 = user;
				}
				else {
					tgt_1 = targets[elem.targetIndex[0]];
				}

				output.AddRange(user.column.get_adjacent_characters(tgt_1));
				break;

			case TargetActive.ADJACENT_SWAPPABLE:
				if (elem.targetIndex[0] == -1) {
					tgt_1 = user;
				}
				else {
					tgt_1 = targets[elem.targetIndex[0]];
				}

				output.AddRange(arenaManager.get_swap_targets(tgt_1));
				break;

			case TargetActive.ENEMIES_LANE_ANY:
				if (elem.targetIndex[0] == -1) {
					tgt_1 = user;
				}
				else {
					tgt_1 = targets[elem.targetIndex[0]];
				}

				output.AddRange(arenaManager.get_attack_targets(tgt_1));
				break;

			case TargetActive.ENEMIES_LANE_SWAPPABLE:
				if (elem.targetIndex[0] == -1) {
					tgt_1 = user;
				}
				else {
					tgt_1 = targets[elem.targetIndex[0]];
				}

				var aux = arenaManager.get_attack_targets(tgt_1);
				for (int i = 0; i < aux.Count; i++) {
					if (aux[i].status.getSwappable()) {
						output.Add(aux[i]);
					}
				}
				break;
		}

		if (output.Count == 0) {
			//no targets available
			//what can i do, i'm just an elf
		}
		current_targets = output;
	}

	public void enter_targetting(CharacterObject user, ElementActive elem, List<CharacterObject> targets) {
		update_targets(user, elem, targets);

		switch (current_state) {
			case States.INITIAL_STATE:
				current_state = States.SHOW_TARGETS;
				
				highlight_targets(true);
				
				break;
		}
	}

	public void click_character(CharacterObject clicked) {
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

				last_selected_target = selected_target;
				selected_target = null;

				highlight_targets(false);
				toggle_confirmation(false);
				
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
		confirmationScreen.SetActive(value);
	}
}
