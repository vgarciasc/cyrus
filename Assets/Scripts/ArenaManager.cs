using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArenaManager : MonoBehaviour {

	public EncounterData defaultEncounter;

	[SerializeField]
	ClickManager clickManager;
	[SerializeField]
	AttackManager attackManager;

	[Header("Slot Columns")]
	[SerializeField]
	SlotColumn left;
	[SerializeField]
	SlotColumn right;

	public static ArenaManager getArenaManager() {
		return (ArenaManager) HushPuppy.safeFindComponent("ArenaManager", "ArenaManager");
	}

	void Start() {
		StartCoroutine (test_killing_slots());
		init_delegates();
	}

	IEnumerator test_killing_slots() {
		// right.get_charobj_by_index(0).take_hit(20);
		yield return new WaitForSeconds (1f);
		// left.get_charobj_by_slotID(3).take_hit(700);
		// yield return new WaitForSeconds (1f);
		// left.get_charobj_by_slotID(2).take_hit(700);
		// yield return new WaitForSeconds (1f);
		// left.get_charobj_by_slotID(1).take_hit(700);
	}

	void init_delegates() {
		clickManager.activate_choosing_attack_target_event += show_attack_targets;
		clickManager.deactivate_choosing_attack_target_event += unshow_attack_targets;

		clickManager.activate_choosing_swap_target_event += show_swap_targets;
		clickManager.deactivate_choosing_swap_target_event += unshow_swap_targets;

		clickManager.conclude_attack_event += unshow_swap_targets;

		attackManager.set_new_target_event += set_specific_attack_targets;
	}

	#region attack targets
		void show_attack_targets(CharacterObject targeter) {
			foreach (CharacterObject co in get_attack_targets(targeter)) {
				if (co == get_default_attack_target(targeter)) {
					co.target.toggle_targethi(true);
				}
				else {
					co.target.toggle_targetlow(true);
				}
			}
		}

		void set_specific_attack_targets(CharacterObject targeter, CharacterObject target) {
			unshow_attack_targets(targeter);
			foreach (CharacterObject co in get_attack_targets(targeter)) {
				if (co == target) {
					co.target.toggle_targethi(true);
				}
				else {
					co.target.toggle_targetlow(true);
				}
			}
		}
		
		void unshow_attack_targets(CharacterObject targeter) {
			foreach (CharacterObject co in left.charObj) {
				co.target.toggle_targets(false);
			}

			foreach (CharacterObject co in right.charObj) {
				co.target.toggle_targets(false);
			}
		}
	#endregion

	#region swap targets
		void show_swap_targets(CharacterObject targeter) {	
			foreach (CharacterObject co in get_swap_targets(targeter)) {
				co.target.toggle_targetlow(true);
			}
		}
		
		void unshow_swap_targets(CharacterObject targeter) {
			unshow_swap_targets();
		}

		void unshow_swap_targets() {
			foreach (CharacterObject co in left.charObj) {
				co.target.toggle_targets(false);
			}

			foreach (CharacterObject co in right.charObj) {
				co.target.toggle_targets(false);
			}
		}
	#endregion

	#region getters
		public SlotColumn get_player_column() {
			return right;
		}

		public SlotColumn get_enemy_column() {
			return left;
		}

		public SlotColumn get_other_column(SlotColumn current) {
			return current == left? right : left;
		}

		public List<CharacterObject> get_attack_targets(CharacterObject targeter) {
			List<CharacterObject> targets = new List<CharacterObject>();
			List<int> slotsOccupied = new List<int>();

			for (int i = 0; i < targeter.column.slots.Count; i++) {
				if (targeter.column.slots[i].charID == targeter.charID) {
					slotsOccupied.Add(i);
				}
			}

			SlotColumn opposite_column = get_other_column(targeter.column);

			foreach (int pos in slotsOccupied) {
				targets.Add(opposite_column.get_charobj_by_slotID(pos));
			}

			if (targets.Count == 0) {
				Debug.Log("Can't attack anyone.");
			}
			return targets;
		}

		public CharacterObject get_default_attack_target(CharacterObject targeter) {
			return get_attack_targets(targeter)[0];
		}

		public List<CharacterObject> get_swap_targets(CharacterObject targetter) {
			List<CharacterObject> targets = new List<CharacterObject>();
			
			switch (targetter.status.getSwapTargets()) {
				case AllyTarget.ADJACENT:
					targets.AddRange(targetter.column.get_adjacent_characters(targetter));
					break;
				case AllyTarget.EVERYONE:
					targets.AddRange(targetter.column.get_column_characters_except_targetter(targetter));
					break;
				case AllyTarget.NONE:
					break;
			}

			List<CharacterObject> aux = new List<CharacterObject>();

			for (int i = 0; i < targets.Count; i++) {
				if (targets[i].status.getSwappable()) {
					aux.Add(targets[i]);
				}
			}

			return aux;
		}
	#endregion

	#region actions
		public void refresh_character_actions() {
			foreach (CharacterObject co in left.charObj) {
				co.refresh_actions();
			}

			foreach (CharacterObject co in right.charObj) {
				co.refresh_actions();
			}
		}
	#endregion

	public CharacterObject get_char_by_targettable(Targettable tgt) {
		foreach (CharacterObject c in left.charObj) {
			if (c.target == tgt) {
				return c;
			}	
		}

		foreach (SlotBackground sb in left.slotsBackground) {
			if (sb.target == tgt) {
				return left.get_charobj_by_slotbg(sb);
			}
		}

		foreach (CharacterObject c in right.charObj) {
			if (c.target == tgt) {
				return c;
			}	
		}

		foreach (SlotBackground sb in right.slotsBackground) {
			if (sb.target == tgt) {
				return right.get_charobj_by_slotbg(sb);
			}
		}

		Debug.Log("Erro!");
		return null;
	}
}
